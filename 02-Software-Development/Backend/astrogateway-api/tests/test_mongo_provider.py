"""Real MongoDB integration checks. Use an explicitly selected local disposable test database."""
import hashlib
import os
from concurrent.futures import ThreadPoolExecutor
from uuid import uuid4
import pytest
from fastapi.testclient import TestClient
MongoClient = pytest.importorskip('pymongo').MongoClient

URL = os.environ.get('AG_TEST_MONGODB_URL')
pytestmark = pytest.mark.skipif(not URL, reason='Set AG_TEST_MONGODB_URL for real local MongoDB integration checks')

@pytest.fixture
def api():
    from mongo_app import create_mongo_app
    name = 'astrogateway_test_' + uuid4().hex
    with TestClient(create_mongo_app(URL, name)) as client:
        yield client, name
    with MongoClient(URL) as mongo:
        mongo.drop_database(name)

def signup(client, suffix='a'):
    result = client.post('/v1/auth/register', json={'email': f'{suffix}@example.invalid', 'password': 'Correct-Horse-Example-2048', 'display_name': 'Local Astronaut', 'username': 'astro_' + suffix})
    assert result.status_code == 201, result.text
    body = result.json()
    return body, {'Authorization': 'Bearer ' + body['access_token']}

def mission(client, headers, launch=None):
    result = client.post('/v1/mission-sessions', headers=headers, json={'launch_id': launch or str(uuid4()), 'mission_id': 'mission-red-horizon-poc'})
    assert result.status_code == 201, result.text
    return result.json()

def test_local_auth_persists_hashed_password_and_token(api):
    client, database = api
    body, h = signup(client)
    assert client.get('/v1/me', headers=h).json()['user_id'] == body['profile']['user_id']
    assert body['email_verified'] is False
    with MongoClient(URL) as mongo:
        user = mongo[database].users.find_one()
        assert user['password_hash'].startswith('$argon2id$') and 'password' not in user
        session = mongo[database].auth_sessions.find_one()
        assert session['_id'] == hashlib.sha256(body['access_token'].encode()).hexdigest()
        assert body['access_token'] not in str(session)
    result = client.post('/v1/auth/login', json={'identifier': 'ASTRO_A', 'password': 'Correct-Horse-Example-2048'})
    assert result.status_code == 200 and result.json()['profile']['user_id'] == body['profile']['user_id']
    assert 'HttpOnly' in result.headers['set-cookie'] and 'SameSite=strict' in result.headers['set-cookie']
    browser = client.post('/v1/auth/login', headers={'Origin': 'http://127.0.0.1:5173'}, json={'identifier': 'astro_a', 'password': 'Correct-Horse-Example-2048'})
    assert browser.status_code == 200 and 'access_token' not in browser.json()

def test_unique_accounts_validation_and_no_client_privileges(api):
    client, _ = api
    signup(client)
    assert client.post('/v1/auth/register', json={'email': 'A@example.invalid', 'password': 'Correct-Horse-Example-2048', 'display_name': 'Someone'}).status_code == 409
    invalid = client.post('/v1/auth/register', json={'email': 'invalid', 'password': 'short-secret', 'display_name': 'X', 'training_level': 10})
    assert invalid.status_code == 422 and 'short-secret' not in invalid.text
    assert client.post('/v1/auth/login', json={'identifier': {'$ne': None}, 'password': 'anything'}).status_code == 422
    assert client.post('/v1/auth/login', json={'identifier': 'a@example.invalid', 'password': 'incorrect'}).status_code == 401

@pytest.mark.parametrize('token', ['x'*43, 'a.b.c', '', 'sb_publishable_test'])
def test_unverified_tokens_rejected(api, token):
    client, _ = api
    assert client.get('/v1/me', headers={'Authorization': 'Bearer ' + token}).status_code == 401

def test_profile_logout_and_expiry_enforced(api):
    client, database = api
    body, h = signup(client)
    p = client.post('/v1/me/display-name', headers=h, json={'display_name': 'Updated Astronaut'})
    assert p.status_code == 200 and p.json()['user_id'] == body['profile']['user_id']
    assert client.post('/v1/me/display-name', headers=h, json={'display_name': 'Other', 'user_id': str(uuid4())}).status_code == 422
    assert client.post('/v1/auth/logout', headers=h, json={}).status_code == 200
    assert client.get('/v1/me', headers=h).status_code == 401
    fresh = client.post('/v1/auth/login', json={'identifier': 'a@example.invalid', 'password': 'Correct-Horse-Example-2048'}).json()
    from datetime import datetime, timezone, timedelta
    with MongoClient(URL) as mongo:
        mongo[database].auth_sessions.update_one({'_id': hashlib.sha256(fresh['access_token'].encode()).hexdigest()}, {'$set': {'expires_at': datetime.now(timezone.utc) - timedelta(seconds=1)}})
    assert client.get('/v1/me', headers={'Authorization': 'Bearer ' + fresh['access_token']}).status_code == 401

def test_mission_ownership_idempotency_and_commands(api):
    client, _ = api
    _, h = signup(client); _, other = signup(client, 'b')
    launch = str(uuid4()); first = mission(client, h, launch)
    assert mission(client, h, launch)['id'] == first['id']
    path = '/v1/mission-sessions/' + first['id']
    assert client.get(path, headers=other).status_code == 404
    assert client.post(path + '/complete', headers=other, json={'launch_id': launch, 'outcome': 'integration_check'}).status_code == 404
    command = {'command_id': str(uuid4()), 'expected_revision': 0, 'kind': 'board_rover', 'payload': {}}
    a = client.post(path + '/commands', headers=h, json=command)
    assert a.status_code == 200 and not a.json()['duplicate']
    assert client.post(path + '/commands', headers=h, json=command).json()['duplicate']
    assert client.post(path + '/commands', headers=h, json={**command, 'kind': 'exit_rover'}).status_code == 409
    assert client.get('/v1/reports/' + first['id'], headers=h).json()['score'] is None
    assert client.get('/v1/skills', headers=h).json()['levels'] is None

def test_concurrent_revision_has_one_winner(api):
    client, _ = api
    _, h = signup(client); row = mission(client, h)
    path = '/v1/mission-sessions/' + row['id'] + '/commands'
    def send(_): return client.post(path, headers=h, json={'command_id': str(uuid4()), 'expected_revision': 0, 'kind': 'board_rover', 'payload': {}}).status_code
    with ThreadPoolExecutor(max_workers=2) as pool: results = list(pool.map(send, range(2)))
    assert sorted(results) == [200, 409]

def test_sensor_records_retries_and_owner_denial(api):
    from test_sensor_records import rec
    client, _ = api
    _, h = signup(client); _, other = signup(client, 'b'); row = mission(client, h)
    path = '/v1/mission-sessions/' + row['id'] + '/sensor-records'
    record = rec()
    assert client.post(path, headers=other, json={'records': [record]}).status_code == 404
    result = client.post(path, headers=h, json={'records': [record]})
    assert result.status_code == 200 and result.json()['accepted'] == 1
    assert client.post(path, headers=h, json={'records': [record]}).json()['duplicates'] == 1
    assert client.post(path, headers=h, json={'records': [{**record, 'value': 4.0}]}).status_code == 409
    assert result.json()['records'][0]['analysis']['status'] == 'not_connected'

def test_browser_origin_and_rate_limits(api):
    client, _ = api
    assert client.post('/v1/auth/login', headers={'Origin': 'https://untrusted.invalid'}, json={'identifier': 'a', 'password': 'x'}).status_code == 403
    statuses = [client.post('/v1/auth/login', json={'identifier': 'missing', 'password': 'x'}).status_code for _ in range(13)]
    assert statuses[-1] == 429

def test_complete_is_idempotent_and_not_score(api):
    client, _ = api
    _, h = signup(client); row = mission(client, h)
    path = '/v1/mission-sessions/' + row['id'] + '/complete'
    payload = {'launch_id': row['launch_id'], 'outcome': 'integration_check'}
    first = client.post(path, headers=h, json=payload)
    assert first.status_code == 200 and first.json()['status'] == 'integration_check_complete'
    assert client.post(path, headers=h, json=payload).json()['completed_at'] == first.json()['completed_at']
    assert client.get('/v1/reports/' + row['id'], headers=h).json()['score'] is None

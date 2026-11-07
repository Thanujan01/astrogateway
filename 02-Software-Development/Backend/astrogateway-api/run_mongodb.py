"""Start the local MongoDB-backed account and mission API. No cloud configuration required."""
import uvicorn
if __name__ == '__main__':
    uvicorn.run('mongo_app:app', host='127.0.0.1', port=8000, access_log=False, log_level='warning')

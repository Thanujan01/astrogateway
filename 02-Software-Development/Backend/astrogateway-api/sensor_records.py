"""First Mission rover sensor records (local only, not deployed).
The Unity client submits SIMULATED or UNAVAILABLE observations; it can never submit an analysis verdict.
MODEL-ESTIMATED and BACKEND-ANALYZED are reserved for the server. No scientific model is connected, so every
record's analysis is returned as not_connected with no result, model or confidence."""
import json
import math
import re
from datetime import datetime, timedelta, timezone
from pathlib import Path
from typing import Literal
from uuid import UUID
from fastapi import Depends, HTTPException
from pydantic import BaseModel, ConfigDict, Field, field_validator, model_validator
from rules.catalog import MissionCatalog
from rules.config import load_config

CATALOG = json.loads((Path(__file__).resolve().parent / 'rules' / 'sensor_catalog_v1.json').read_text(encoding='utf-8'))
SENSORS = {s['sensor_id']: s for s in CATALOG['sensors']}
INSTRUMENTS = {i['instrument_id']: i for i in CATALOG['instruments']}
LIMITS = CATALOG['limits']
CANONICAL_MISSION = CATALOG['mission']['mission_id']
ANALYSIS_INTERFACE = CATALOG['analysis_interface']['version']
MISSIONS = MissionCatalog(load_config())
TOKEN = re.compile(r'^[a-z][a-z0-9_.-]{1,63}$')


class SensorLocation(BaseModel):
    model_config = ConfigDict(extra='forbid')
    x_m: float = Field(ge=-200, le=200)
    y_m: float = Field(ge=-50, le=200)
    z_m: float = Field(ge=-200, le=200)
    frame: Literal['scene_local_m']


class SensorRecordIn(BaseModel):
    model_config = ConfigDict(extra='forbid')
    schema_version: Literal[1]
    event_id: UUID
    observation_id: UUID
    mission_id: str = Field(max_length=64)
    mission_version: Literal[1]
    build_variant: str = Field(max_length=48)
    sensor_id: str = Field(max_length=64)
    sensor_type: Literal['environmental', 'terrain', 'scientific', 'navigation']
    instrument_id: str = Field(max_length=64)
    station_id: str | None = Field(default=None, max_length=16)
    sim_time_s: float = Field(ge=0, le=86400)
    sim_tick: int = Field(ge=0, le=1_000_000_000)
    captured_at: datetime
    location: SensorLocation
    value: float | None = None
    value_text: str | None = Field(default=None, max_length=64)
    unit: str = Field(max_length=16)
    # Only these two may come from a client. A client-made MODEL-ESTIMATED / BACKEND-ANALYZED claim fails here.
    provenance: Literal['SIMULATED', 'UNAVAILABLE']
    method: str = Field(max_length=64)
    sim_config_version: Literal['fm-sensors-1']

    @model_validator(mode='before')
    @classmethod
    def non_finite(cls, data):
        # NaN/Infinity would make the 422 error body itself unserialisable (500). Replace them with a marker first.
        def clean(v):
            if isinstance(v, float) and not math.isfinite(v): return 'non-finite'
            if isinstance(v, dict): return {k: clean(x) for k, x in v.items()}
            return v
        return clean(data) if isinstance(data, dict) else data

    @field_validator('build_variant', 'method')
    @classmethod
    def token(cls, v):
        if not TOKEN.match(v): raise ValueError('Invalid identifier.')
        return v

    @field_validator('captured_at')
    @classmethod
    def clock(cls, v):
        if v.tzinfo is None: raise ValueError('captured_at needs a timezone.')
        now = datetime.now(timezone.utc)
        if v > now + timedelta(seconds=LIMITS['clock_skew_s']) or v < now - timedelta(days=LIMITS['max_age_days']):
            raise ValueError('captured_at outside the accepted window.')
        return v

    @model_validator(mode='after')
    def against_catalog(self):
        try:
            resolved = MISSIONS.resolve(self.mission_id)
        except KeyError:
            raise ValueError('Unknown mission id.') from None
        if resolved.canonical_id != CANONICAL_MISSION: raise ValueError('Mission is not the First Mission.')
        sensor = SENSORS.get(self.sensor_id)
        instrument = INSTRUMENTS.get(self.instrument_id)
        if sensor is None or instrument is None: raise ValueError('Unknown sensor or instrument.')
        if self.sensor_id not in instrument['sensors']: raise ValueError('Sensor does not belong to this instrument.')
        if sensor['sensor_type'] != self.sensor_type or sensor['unit'] != self.unit: raise ValueError('Sensor type or unit mismatch.')
        if self.station_id is not None and not re.match(r'^site_[0-9]{1,3}$', self.station_id): raise ValueError('Invalid station id.')
        if self.provenance == 'UNAVAILABLE' or not sensor['implemented'] or not instrument['available']:
            # Not-modelled sensors must be reported as unavailable, never with a number.
            if self.provenance != 'UNAVAILABLE' or self.value is not None or self.value_text is not None:
                raise ValueError('Sensor is not implemented; only an UNAVAILABLE record without a value is accepted.')
            return self
        if self.method != sensor['method']: raise ValueError('Method does not match the sensor catalog.')
        kind = sensor['kind']
        if kind == 'numeric':
            if self.value is None or self.value_text is not None or not math.isfinite(self.value) or not sensor['min'] <= self.value <= sensor['max']:
                raise ValueError('Numeric value missing or out of range.')
        elif kind == 'category':
            if self.value is not None or self.value_text not in sensor['values']: raise ValueError('Category value not allowed.')
        elif kind == 'identifier':
            if self.value is not None or self.value_text is None or not re.match(sensor['pattern'], self.value_text): raise ValueError('Identifier invalid.')
            if self.sensor_id == 'sci.station_id' and self.value_text != self.station_id: raise ValueError('Station id mismatch.')
        elif kind == 'location':
            if self.value is not None or self.value_text is not None: raise ValueError('Location sensors carry no value.')
        return self


class SensorBatchIn(BaseModel):
    model_config = ConfigDict(extra='forbid')
    records: list[SensorRecordIn] = Field(min_length=1, max_length=LIMITS['max_batch'])

    @model_validator(mode='after')
    def unique(self):
        if len({r.event_id for r in self.records}) != len(self.records): raise ValueError('Duplicate event id in batch.')
        return self


class AnalysisStatus(BaseModel):
    interface_version: Literal['ag.science-analysis.v0'] = 'ag.science-analysis.v0'
    status: Literal['not_connected'] = 'not_connected'
    model_version: None = None
    result: None = None
    confidence: None = None
    reason: str = 'No scientific analysis model is connected.'


class SensorRecordAck(BaseModel):
    event_id: UUID
    duplicate: bool
    stored_at: datetime
    provenance: Literal['SIMULATED', 'UNAVAILABLE']
    analysis: AnalysisStatus


class SensorBatchAck(BaseModel):
    session_id: UUID
    accepted: int
    duplicates: int
    records: list[SensorRecordAck]


def record_fingerprint(record: SensorRecordIn) -> str:
    """Stable comparison of a resubmitted record (retry) against the stored one."""
    return json.dumps(record.model_dump(mode='json'), sort_keys=True, separators=(',', ':'))


def register(app, authorized_user, repository):
    @app.post('/v1/mission-sessions/{session_id}/sensor-records', response_model=SensorBatchAck)
    async def submit_sensor_records(session_id: UUID, payload: SensorBatchIn, user=Depends(authorized_user), db=Depends(repository)):
        rows = await db.submit_sensor_records(user, session_id, payload.records)
        if not isinstance(rows, list) or len(rows) != len(payload.records):
            raise HTTPException(502, 'Sensor record validation failed.')
        acks = []
        for record, row in zip(payload.records, rows):
            if not isinstance(row, dict) or str(row.get('event_id')) != str(record.event_id) or str(row.get('session_id')) != str(session_id):
                raise HTTPException(502, 'Sensor record validation failed.')
            acks.append(SensorRecordAck(event_id=record.event_id, duplicate=bool(row['duplicate']), stored_at=row['stored_at'],
                                        provenance=record.provenance, analysis=AnalysisStatus()))
        return SensorBatchAck(session_id=session_id, accepted=sum(not a.duplicate for a in acks), duplicates=sum(a.duplicate for a in acks), records=acks)

    @app.get('/v1/science/analysis-status', response_model=AnalysisStatus)
    async def analysis_status(user=Depends(authorized_user)):
        return AnalysisStatus()

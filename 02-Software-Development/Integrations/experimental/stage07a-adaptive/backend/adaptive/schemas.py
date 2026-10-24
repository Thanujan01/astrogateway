"""Bounded, versioned telemetry; no owner, final score or evidence-trust input."""
from typing import Annotated,Literal,Union
from uuid import UUID
from pydantic import BaseModel,ConfigDict,Field,model_validator,StrictBool
class Strict(BaseModel):
 model_config=ConfigDict(extra='forbid',allow_inf_nan=False)
class Event(Strict):
 event_id:UUID
 elapsed_s:float=Field(ge=0,le=7200)
class Movement(Event):
 kind:Literal['movement']
 x_m:float=Field(ge=-125,le=125)
 z_m:float=Field(ge=-125,le=125)
 speed_mps:float=Field(ge=0,le=4)
class Reaction(Event):
 kind:Literal['reaction']
 reaction_s:float=Field(ge=.1,le=60)
class Resource(Event):
 kind:Literal['resource']
 oxygen_used_l:float=Field(ge=0,le=1000)
 water_used_l:float=Field(ge=0,le=20)
 power_used_wh:float=Field(ge=0,le=5000)
class Equipment(Event):
 kind:Literal['equipment']
 equipment:Literal['airlock','solar_array','rover','water_recycler','sampler']
 correct:StrictBool
class Emergency(Event):
 kind:Literal['emergency']
 incident:Literal['dust','power_loss','oxygen_leak','rover_fault']
 resolved:StrictBool
 response_s:float=Field(ge=.1,le=600)
class Mistake(Event):
 kind:Literal['mistake']
 category:Literal['navigation','survival','decision_making','equipment','emergency']
 retries:int=Field(ge=0,le=5)
class Completion(Event):
 kind:Literal['completion']
 completed:StrictBool
TelemetryEvent=Annotated[Union[Movement,Reaction,Resource,Equipment,Emergency,Mistake,Completion],Field(discriminator='kind')]
class Batch(Strict):
 schema_version:Literal[1]=1
 mission_session_id:UUID
 events:list[TelemetryEvent]=Field(min_length=1,max_length=128)
 @model_validator(mode='after')
 def ordered(self):
  if len({e.event_id for e in self.events})!=len(self.events):raise ValueError('Duplicate event IDs')
  if any(b.elapsed_s<=a.elapsed_s for a,b in zip(self.events,self.events[1:])):raise ValueError('Events must increase in time')
  if sum(e.kind=='completion' for e in self.events)>1 or any(e.kind=='completion' for e in self.events[:-1]):raise ValueError('Completion must be last')
  return self
class Recommendation(Strict):
 previous:Literal['Easy','Medium','Hard']='Easy'
class Debrief(Strict):
 mission_session_id:UUID
 question:Literal['summary','navigation','survival','decision_making','equipment','emergency']='summary'

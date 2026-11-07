"""Compile and exercise the actual device-local save model without an Editor license."""
import argparse
import os
from pathlib import Path
import subprocess
import tempfile

ROOT = Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--mono-root', type=Path, default=Path(os.environ.get('UNITY_MONO_ROOT', '/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/Resources/Scripting/MonoBleedingEdge')))
args = parser.parse_args()
source = ROOT / '01-Game-Development/MissionRedHorizon/Assets/AstroGateway/FirstMission/Scripts/Missions/MissionProgressState.cs'
runner = '''using System;
using AstroGateway.FirstMission.Missions;
class Program {
static int count;
static readonly int[] steps = {3,2,4,4,3,2};
static MissionProgressState Normalize(MissionProgressState s) => s.Normalize(steps,3);
static void Check(bool ok, string name) { if (!ok) throw new Exception(name); count++; }
static void Main() {
var fresh=Normalize(new MissionProgressState());
Check(!fresh.finished && fresh.mission==0 && fresh.o2==40, "New player starts at mission one");
var partial=Normalize(new MissionProgressState {mission=2,step=3,repairProgress=.45f,activeSeconds=132,retries=2});
Check(partial.step==3 && partial.repairProgress==.45f && partial.activeSeconds==132 && partial.retries==2,"Partial repair and debrief survive normalization");
var complete=Normalize(new MissionProgressState {mission=6,finished=true});
Check(complete.finished && complete.step==0,"Completed legacy save remains complete");
Check(!Normalize(new MissionProgressState {mission=2,finished=true}).finished,"Stale completion flag cannot skip remaining missions");
foreach (int index in new[]{-1,7,int.MaxValue}) {
var s=Normalize(new MissionProgressState {mission=index,finished=true,repaired=true});
Check(s.mission==0 && !s.finished && !s.repaired,"Invalid mission index restarts guidance");
}
Check(Normalize(new MissionProgressState {mission=2,step=-1}).step==0,"Negative step repaired");
Check(Normalize(new MissionProgressState {mission=2,step=4}).step==0,"Past-end step repaired");
Check(Normalize(new MissionProgressState {checkpoints=99}).checkpoints==3,"Checkpoint cap");
Check(Normalize(new MissionProgressState {checkpoints=-1}).checkpoints==0,"Negative checkpoint repaired");
foreach (float invalid in new[]{float.NaN,float.PositiveInfinity,float.NegativeInfinity}) {
var s=Normalize(new MissionProgressState {charge=invalid,repairProgress=invalid,o2=invalid,stormEta=invalid,activeSeconds=invalid});
Check(s.charge==0 && s.repairProgress==0 && s.o2==40 && s.stormEta==200 && s.activeSeconds==0,"Nonfinite save values cannot poison gameplay");
}
var bounds=Normalize(new MissionProgressState {charge=2,repairProgress=-2,o2=200,stormEta=900,activeSeconds=-1,retries=-1});
Check(bounds.charge==1 && bounds.repairProgress==0 && bounds.o2==100 && bounds.stormEta==200 && bounds.activeSeconds==0 && bounds.retries==0,"Physical bounds and counters");
Check(Normalize(new MissionProgressState {repaired=true}).repairProgress==1,"Legacy repaired generator restores full bar");
Check(Normalize(new MissionProgressState {o2=0}).o2==30,"Resume gives oxygen retry reserve");
Check(Normalize(new MissionProgressState {stormStarted=true,stormEta=0}).stormEta==150,"Resume gives storm retry window");
Check(Normalize(new MissionProgressState {mission=6,stormStarted=true,stormEta=0}).stormEta==0,"Completed storm save does not restart countdown");
var failed=Normalize(new MissionProgressState {failed=true,o2=0,stormStarted=true,stormEta=0,failReason="CAUGHT IN THE STORM"});
Check(failed.failed && failed.o2==0 && failed.stormEta==0,"Failed run is preserved until explicit retry");
bool rejected=false; try { new MissionProgressState().Normalize(new[]{0},3); } catch(ArgumentException) { rejected=true; }
Check(rejected,"Empty mission definition is rejected");
Console.WriteLine("PASS: "+count+" save-state cases using actual C# source; no Unity runtime/device claim");
}}
'''
with tempfile.TemporaryDirectory(prefix='ag-progress-') as temp:
    p = Path(temp)
    (p / 'Program.cs').write_text(runner)
    exe = p / 'validation.exe'
    subprocess.run([str(args.mono_root / 'bin/mono'), str(args.mono_root / 'lib/mono/4.5/mcs.exe'), '-out:' + str(exe), str(source), str(p / 'Program.cs')], check=True)
    subprocess.run([str(args.mono_root / 'bin/mono'), str(exe)], check=True)

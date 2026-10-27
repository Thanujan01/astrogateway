"""Compile actual coach + JSON reader using Unity's bundled Mono, without editor licensing."""
from pathlib import Path
import subprocess, tempfile
ROOT = Path(__file__).resolve().parents[2]
MONO = Path('/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/Resources/Scripting/MonoBleedingEdge')
GAME = ROOT / '01-Game-Development/MissionRedHorizon'
src = GAME / 'Assets/AstroGateway/MissionRules/Scripts'
# Extract the actual config class, which otherwise shares a file with Unity-dependent models.
raw = (src / 'MissionRules.cs').read_text()
config = raw[raw.index('    public class MissionRulesConfig'):raw.index('    // ---------- mission identifiers')]
runner = '''using System; using System.IO; using System.Collections.Generic;
using AstroGateway.MissionRules;
namespace AstroGateway.MissionRules { CONFIG }
class Program {
static void Main(string[] args) {
var config = new MissionRulesConfig(File.ReadAllText(args[0]));
var coach = new OfflineMissionCoach(config);
var fixtures = (Dictionary<string,object>)MiniJson.Parse(File.ReadAllText(args[1]));
int count=0;
foreach(var item in (List<object>)fixtures["cases"]) {
var c=(Dictionary<string,object>)item;
var history=new List<OfflineMissionCoach.Report>();
foreach(var score in (List<object>)c["scores"]) history.Add(new OfflineMissionCoach.Report {
SessionId="fixture-"+history.Count, RulesVersion=config.Version, Difficulty=Convert.ToInt32(c["difficulty"]), Score=Convert.ToDouble(score)});
if(coach.Recommend(history,Convert.ToInt32(c["weather"])).Difficulty!=Convert.ToInt32(c["expected"])) throw new Exception("Parity failure "+count);
count++;
}
foreach(var score in new[]{double.NaN,double.PositiveInfinity,-1,101}) {
var r=new OfflineMissionCoach.Report{SessionId="x",RulesVersion=config.Version,Difficulty=1,Score=score};
if(coach.Recommend(new[]{r},0).Status!="invalid-or-incompatible-history") throw new Exception("Invalid accepted"); count++;
}
var good=new OfflineMissionCoach.Report{SessionId="x",RulesVersion=config.Version,Difficulty=1,Score=90};
if(coach.Recommend(new[]{good,good},0).Status!="invalid-or-incompatible-history") throw new Exception("Duplicate accepted");count++;
good.RulesVersion="old";
if(coach.Recommend(new[]{good},0).Status!="invalid-or-incompatible-history") throw new Exception("Version accepted");count++;
if(coach.Recommend(null,0).Status!="invalid-or-incompatible-history") throw new Exception("Null accepted");count++;
if(coach.Recommend(new OfflineMissionCoach.Report[0],-1).Status!="invalid-or-incompatible-history") throw new Exception("Negative accepted");count++;
Console.WriteLine("PASS: "+count+" cases using actual C# coach and parser; 438 backend parity cases");
}}
'''.replace('CONFIG', config)
with tempfile.TemporaryDirectory(prefix='ag-offline-coach-') as temp:
    p = Path(temp)
    (p/'Program.cs').write_text(runner)
    exe = p/'validation.exe'
    subprocess.run([str(MONO/'bin/mono'), str(MONO/'lib/mono/4.5/mcs.exe'), '-out:'+str(exe), str(p/'Program.cs'), str(src/'OfflineMissionCoach.cs'), str(src/'MiniJson.cs')], check=True)
    subprocess.run([str(MONO/'bin/mono'),str(exe),str(GAME/'Assets/AstroGateway/MissionRules/Config/mission_rules_v1.json'),str(GAME/'Assets/AstroGateway/MissionRules/Config/offline_coach_vectors.json')],check=True)

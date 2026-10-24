from pathlib import Path
p=Path('work/stage04/MarsEnvironmentBuilder.cs');s=p.read_text()
s=s.replace('else if(c=="save")','else if(c=="refine")Refine();else if(c=="save")')
s=s.replace('if(layered)rr=radius*(1+(r%2==0?.05f:-.05f));verts.Add(new Vector3(Mathf.Cos(angle)*rr+(y-.5f)*.12f,y,Mathf.Sin(angle)*rr*.8f));uv.Add(new Vector2((float)s/sides,y));','if(layered){float fracture=.9f+.11f*Mathf.Sin(angle*3+seed)+.07f*Mathf.Cos(angle*7);rr=(1-.28f*y)*fracture+.025f*Mathf.Sin(y*37+angle*2);y=y*(.95f+.05f*Mathf.Sin(angle*2+seed));}verts.Add(new Vector3(Mathf.Cos(angle)*rr+(y-.5f)*.12f,y,Mathf.Sin(angle)*rr*.8f));uv.Add(new Vector2((float)s/sides,y));')
method='''
 public static void Refine(){Open();mats=AssetDatabase.FindAssets("t:Material",new[]{F+"/Materials"}).Select(g=>AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(g))).ToDictionary(m=>m.name,m=>m);
 // Owned Stage 04 meshes only. All source greybox references remain intact.
 for(int i=0;i<3;i++){string oldName="LayeredDelta_LOD"+i;var old=AssetDatabase.LoadAssetAtPath<Mesh>(F+"/Rocks/"+oldName+".asset");var refined=Rock("DeltaFractured_LOD"+i,i==0?22:i==1?14:8,i==0?12:i==1?6:3,63,true);EditorUtility.CopySerialized(refined,old);old.name=oldName;EditorUtility.SetDirty(old);}
 var layered=Texture("Delta_FracturedStrata_512",512,(u,v)=>{float w=v*53+Mathf.PerlinNoise(u*5+1,v*2+1)*1.5f;float band=Mathf.Sin(w);float seam=Mathf.Pow(Mathf.Clamp01(-band),7)*.22f;return Color.Lerp(Hex("#746453"),Hex("#A48B6B"),.32f+Noise(u,v)*.55f-seam);});mats["Mars_Rock_Layered"].SetTexture("_BaseMap",layered);mats["Mars_Rock_Layered"].SetTexture("_BumpMap",AssetDatabase.LoadAssetAtPath<Texture2D>(F+"/Materials/Regolith_MicroNormal_256.png"));mats["Mars_Rock_Layered"].SetFloat("_BumpScale",.3f);mats["Mars_Rock_Layered"].EnableKeyword("_NORMALMAP");EditorUtility.SetDirty(mats["Mars_Rock_Layered"]);
 foreach(var t in zones.Find("D_LayeredDeltaTerrain").GetComponentsInChildren<Transform>()){if(t.name.StartsWith("DeltaTerrace_")){int j=int.Parse(t.name.Substring(t.name.Length-1));t.position=At(-29+j*14,110+j%2*3,-.65f);t.localScale=new Vector3(12+j%2*2,3.5f+j%3*.6f,7.5f);t.localRotation=Quaternion.Euler(0,15+j*27,0);}if(t.name=="WeatheredDeltaToe")t.position-=Vector3.up*.22f;}
 var details=Group("07_FunctionalExteriorDetails",root);
 // A compact original utility kit: recessed cabinet faces, bolts, radiator fins, supported tanks and dust skirts.
 var h=Group("HabitatExternalKit",details);for(int i=0;i<6;i++){float x=-70.5f+i*2.0f;Box("RecessedServiceAccess",At(x,-64.075f,1.42f),new Vector3(1.45f,.85f,.035f),"Habitat_CompositeDark",h);Box("ServiceCover",At(x,-64.10f,1.44f),new Vector3(1.32f,.69f,.03f),"Habitat_OffWhite",h);foreach(float sign in new[]{-1f,1f})Box("PanelFastener",At(x+sign*.56f,-64.13f,1.44f),new Vector3(.045f,.045f,.035f),"Metal_Industrial",h);}
 for(int i=0;i<8;i++)Box("AirlockCoolingLouvre",At(-55.5f+i*.25f,-62.035f,2.35f),new Vector3(.10f,.36f,.06f),"Metal_Industrial",h);
 var hv=Cylinder("AtmosphereServiceAccumulator",At(-71.2f,-55.2f,1.1f),.45f,2.2f,"Habitat_OffWhite",h);Conduit(h,"AccumulatorConnection",At(-71.2f,-55.2f,.25f),At(-68,-55.2f,.25f),At(-68,-56,.25f));
 var p=Group("PowerExternalKit",details);for(int i=0;i<4;i++){float x=24+i*2;Box("BatteryLowerDustApron",At(x,-42.14f,.38f),new Vector3(1.7f,.24f,.05f),"Mars_Regolith_Dark",p);for(int j=0;j<5;j++)Box("ServicePanelVent",At(x,-42.15f,1.95f+j*.065f),new Vector3(.75f,.024f,.035f),"Habitat_CompositeDark",p);}
 // Clustered low stones beside (not across) approaches, including foreground at hero views.
 var small=AssetDatabase.LoadAssetAtPath<Mesh>(F+"/Rocks/AngularBasalt_LOD2.asset");var stone=Group("ApproachStoneClusters",details);var random=new System.Random(407);foreach(var center in new[]{new Vector2(-49,-76),new Vector2(-76,-68),new Vector2(-48,-34),new Vector2(11,-64),new Vector2(37,-63),new Vector2(-14,85),new Vector2(20,88)})for(int i=0;i<16;i++){var q=center+new Vector2((float)random.NextDouble()*6-3,(float)random.NextDouble()*4-2);if(RouteDistance(q)<3.1f)continue;float scale=.08f+(float)random.NextDouble()*.3f;MeshGO("DustExposedStone",small,mats["Mars_Rock_Basaltic"],stone,At(q.x,q.y,-.02f),new Vector3(scale,scale*.55f,scale));}
 Combine(h,"Props");Combine(p,"Props");Combine(stone,"Props");
 var arrival=cameras.Find("View_A_Arrival");arrival.position=At(-42,-69,1.7f);arrival.LookAt(At(-55,-42,1.3f));arrival.GetComponent<Camera>().fieldOfView=64;
 var delta=cameras.Find("View_D_Delta");delta.position=At(-12,84,1.7f);delta.LookAt(At(0,110,1.5f));
 var solar=cameras.Find("View_C_SolarPower");solar.position=At(40,-72,1.7f);solar.LookAt(At(60,-47,1.4f));
 AssetDatabase.SaveAssets();EditorSceneManager.SaveScene(SceneManager.GetActiveScene());Validate();Capture();Debug.Log("ASTROGATEWAY_ART_REFINED");
 }
'''
s=s.replace(' static Scene Open(){',method+'\n static Scene Open(){');p.write_text(s)

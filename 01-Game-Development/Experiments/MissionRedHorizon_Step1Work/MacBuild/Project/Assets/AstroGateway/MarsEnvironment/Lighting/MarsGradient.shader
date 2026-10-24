Shader "AstroGateway/MarsGradient" {Properties {_Top("Zenith",Color)=(.3,.23,.19,1) _Horizon("Horizon",Color)=(.64,.43,.3,1)} SubShader {Tags {"Queue"="Background" "RenderType"="Background" "RenderPipeline"="UniversalPipeline"} Pass {ZWrite Off Cull Off HLSLPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
float4 _Top,_Horizon;struct Attributes{float3 positionOS:POSITION;};struct Varyings{float4 positionCS:SV_POSITION;float3 dir:TEXCOORD0;};Varyings vert(Attributes i){Varyings o;o.positionCS=TransformObjectToHClip(i.positionOS);o.dir=i.positionOS;return o;}half4 frag(Varyings i):SV_Target{return lerp(_Horizon,_Top,saturate(normalize(i.dir).y*.8+.2));}
ENDHLSL } } }

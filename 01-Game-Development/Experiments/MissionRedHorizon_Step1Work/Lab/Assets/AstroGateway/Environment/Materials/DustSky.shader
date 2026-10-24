Shader "AstroGateway/DustSky" {
 Properties { _Zenith("Zenith", Color)=(0.47,0.43,0.38,1) _Horizon("Dust horizon",Color)=(0.76,0.64,0.52,1) }
 SubShader { Tags {"Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" "RenderPipeline"="UniversalPipeline"} Cull Off ZWrite Off
 Pass { HLSLPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
 struct A {float4 positionOS:POSITION;}; struct V {float4 positionCS:SV_POSITION;float3 direction:TEXCOORD0;};
 CBUFFER_START(UnityPerMaterial) half4 _Zenith; half4 _Horizon; CBUFFER_END
 V vert(A a){V o;o.positionCS=TransformObjectToHClip(a.positionOS.xyz);o.direction=a.positionOS.xyz;return o;}
 half4 frag(V i):SV_Target{float y=normalize(i.direction).y;float f=pow(saturate(y),.45);return half4(lerp(_Horizon.rgb,_Zenith.rgb,f),1);}
 ENDHLSL }
 }
}

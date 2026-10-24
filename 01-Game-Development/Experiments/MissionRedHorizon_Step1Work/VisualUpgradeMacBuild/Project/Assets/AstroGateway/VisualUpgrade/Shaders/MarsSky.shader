// Mars sky for Mars_VisualUpgrade_v1 (procedural, temporary art): butterscotch zenith-to-horizon gradient, a dusty
// horizon haze band, a small sun disk with the cool forward-scatter halo seen in Mars rover imagery, and a faint
// ground colour below the horizon. _SunDir is set from the scene's directional light by VisualUpgradeAtmosphere.
Shader "AstroGateway/VisualUpgrade/MarsSky"
{
    Properties
    {
        _Top("Zenith", Color) = (0.36, 0.25, 0.18, 1)
        _Horizon("Horizon", Color) = (0.80, 0.58, 0.40, 1)
        _Haze("Horizon haze", Color) = (0.86, 0.66, 0.48, 1)
        _Ground("Below horizon", Color) = (0.42, 0.28, 0.19, 1)
        _Halo("Sun halo", Color) = (0.62, 0.70, 0.78, 1)
        _SunColor("Sun disk", Color) = (1.4, 1.3, 1.15, 1)
        _SunDir("Sun direction (to sun)", Vector) = (0.4, 0.35, 0.6, 0)
        _SunSize("Sun size", Range(0.990, 0.9999)) = 0.9993
        _HaloPower("Halo tightness", Range(1, 64)) = 10
        _HaloStrength("Halo strength", Range(0, 2)) = 0.55
        _HazeHeight("Haze height", Range(0.01, 0.5)) = 0.14
        _Exposure("Exposure", Range(0.2, 3)) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Background" "RenderType" = "Background" "RenderPipeline" = "UniversalPipeline" "PreviewType" = "Skybox" }
        Pass
        {
            ZWrite Off Cull Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _Top, _Horizon, _Haze, _Ground, _Halo, _SunColor;
                float4 _SunDir;
                float _SunSize, _HaloPower, _HaloStrength, _HazeHeight, _Exposure;
            CBUFFER_END

            struct Attributes { float3 positionOS : POSITION; };
            struct Varyings { float4 positionCS : SV_POSITION; float3 dir : TEXCOORD0; };

            Varyings Vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS);
                o.dir = v.positionOS;
                return o;
            }

            half4 Frag(Varyings i) : SV_Target
            {
                float3 d = normalize(i.dir);
                float3 s = normalize(_SunDir.xyz);
                float h = d.y;
                half3 sky = lerp(_Horizon.rgb, _Top.rgb, pow(saturate(h), 0.55));
                sky = lerp(sky, _Haze.rgb, exp(-max(h, 0) / _HazeHeight) * 0.85);
                half3 below = lerp(_Haze.rgb, _Ground.rgb, saturate(-h * 6));
                half3 c = h >= 0 ? sky : below;
                float cosA = dot(d, s);
                float halo = pow(saturate(cosA), _HaloPower) * _HaloStrength;
                c = lerp(c, _Halo.rgb, saturate(halo) * saturate(h * 8 + 0.3));
                c += _SunColor.rgb * pow(saturate(cosA), 900) * 0.25;
                c = lerp(c, _SunColor.rgb, smoothstep(_SunSize, _SunSize + 0.0003, cosA));
                return half4(c * _Exposure, 1);
            }
            ENDHLSL
        }
    }
}

// Mars ground for Mars_VisualUpgrade_v1 (procedural, temporary art). World-space XZ mapping so the existing
// Terrain250m mesh and the new far-terrain mesh blend seamlessly. Two scales of the same regolith texture hide
// tiling, a second rocky texture is blended in by noise and slope, and low-frequency noise adds dune-scale
// colour and roughness variation. Lit by URP (main + additional lights, shadows, SSAO, fog).
Shader "AstroGateway/VisualUpgrade/MarsTerrain"
{
    Properties
    {
        [MainTexture] _BaseMap("Regolith albedo", 2D) = "white" {}
        [Normal] _BumpMap("Regolith normal", 2D) = "bump" {}
        _RockMap("Rock albedo", 2D) = "white" {}
        [Normal] _RockNormal("Rock normal", 2D) = "bump" {}
        _MacroMap("Macro variation", 2D) = "gray" {}
        _TileNear("Near tile size (m)", Float) = 2.5
        _TileFar("Far tile size (m)", Float) = 17
        _RockTile("Rock tile size (m)", Float) = 4
        _MacroTile("Macro tile size (m)", Float) = 180
        _Tint("Regolith tint", Color) = (0.78, 0.47, 0.30, 1)
        _DarkTint("Dark patch tint", Color) = (0.42, 0.25, 0.17, 1)
        _RockTint("Rock tint", Color) = (0.50, 0.36, 0.28, 1)
        _RockAmount("Rock amount", Range(0, 1)) = 0.35
        _SlopeRock("Slope to rock", Range(0, 4)) = 2.2
        _NormalStrength("Normal strength", Range(0, 2)) = 1
        _Smoothness("Smoothness", Range(0, 1)) = 0.12
        _FarBlendStart("Far blend start (m)", Float) = 12
        _FarBlendEnd("Far blend end (m)", Float) = 60
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" }
        LOD 300

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT _SHADOWS_SOFT_LOW _SHADOWS_SOFT_MEDIUM _SHADOWS_SOFT_HIGH
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile _ _CLUSTER_LIGHT_LOOP
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            TEXTURE2D(_BumpMap); SAMPLER(sampler_BumpMap);
            TEXTURE2D(_RockMap); SAMPLER(sampler_RockMap);
            TEXTURE2D(_RockNormal); SAMPLER(sampler_RockNormal);
            TEXTURE2D(_MacroMap); SAMPLER(sampler_MacroMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float _TileNear, _TileFar, _RockTile, _MacroTile;
                half4 _Tint, _DarkTint, _RockTint;
                half _RockAmount, _SlopeRock, _NormalStrength, _Smoothness;
                float _FarBlendStart, _FarBlendEnd;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                half fogFactor : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float Hash(float2 p) { return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453); }
            float Noise(float2 p)
            {
                float2 i = floor(p), f = frac(p);
                float2 u = f * f * (3 - 2 * f);
                return lerp(lerp(Hash(i), Hash(i + float2(1, 0)), u.x), lerp(Hash(i + float2(0, 1)), Hash(i + float2(1, 1)), u.x), u.y);
            }
            float Fbm(float2 p) { return Noise(p) * .55 + Noise(p * 2.03 + 17.1) * .28 + Noise(p * 4.07 + 41.3) * .17; }

            Varyings Vert(Attributes v)
            {
                Varyings o = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                VertexPositionInputs p = GetVertexPositionInputs(v.positionOS.xyz);
                o.positionCS = p.positionCS;
                o.positionWS = p.positionWS;
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.fogFactor = ComputeFogFactor(p.positionCS.z);
                return o;
            }

            half4 Frag(Varyings i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                float2 w = i.positionWS.xz;
                float dist = distance(GetCameraPositionWS(), i.positionWS);
                float far = saturate((dist - _FarBlendStart) / max(1, _FarBlendEnd - _FarBlendStart));

                // Regolith at two scales: near detail fades into a larger tile with distance (hides repetition).
                half3 aNear = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, w / _TileNear).rgb;
                half3 aFar = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, w / _TileFar + .37).rgb;
                half3 regolith = lerp(aNear, lerp(aNear, aFar, .65), far);
                half3 nNear = UnpackNormalScale(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, w / _TileNear), _NormalStrength);
                half3 nFar = UnpackNormalScale(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, w / _TileFar + .37), _NormalStrength * .6);
                half3 nRegolith = normalize(lerp(nNear, nFar, far * .7));

                // Macro variation: a painted macro map plus fBm noise for dune-scale light/dark patches.
                half macro = SAMPLE_TEXTURE2D(_MacroMap, sampler_MacroMap, w / _MacroTile).r;
                float n = Fbm(w / 38.0);
                float patches = saturate((n - .42) * 3.2);
                half3 tint = lerp(_Tint.rgb, _DarkTint.rgb, saturate(patches * .5 + (0.5 - macro) * .45));

                // Rock exposure by noise and slope.
                float3 N = normalize(i.normalWS);
                float slope = 1 - saturate(N.y);
                float rockMask = saturate((Fbm(w / 21.0 + 9.7) - (1 - _RockAmount)) * 4 + slope * _SlopeRock * 4);
                half3 rock = SAMPLE_TEXTURE2D(_RockMap, sampler_RockMap, w / _RockTile).rgb * _RockTint.rgb * 1.6;
                half3 nRock = UnpackNormalScale(SAMPLE_TEXTURE2D(_RockNormal, sampler_RockNormal, w / _RockTile), _NormalStrength);

                half3 albedo = lerp(regolith * tint * 1.55, rock, rockMask);
                albedo *= lerp(.92, 1.08, Noise(w / 5.3)); // small-scale brightness breakup
                half3 nTS = normalize(lerp(nRegolith, nRock, rockMask));

                // Planar XZ tangent frame.
                float3 T = normalize(float3(1, 0, 0) - N * N.x);
                float3 B = normalize(cross(T, N)); // +Z when N is up, matching uv.y = world z
                float3 normalWS = normalize(nTS.x * T + nTS.y * B + nTS.z * N);

                InputData d = (InputData)0;
                d.positionWS = i.positionWS;
                d.positionCS = i.positionCS;
                d.normalWS = normalWS;
                d.viewDirectionWS = GetWorldSpaceNormalizeViewDir(i.positionWS);
                d.shadowCoord = TransformWorldToShadowCoord(i.positionWS);
                d.fogCoord = i.fogFactor;
                d.bakedGI = SampleSH(normalWS);
                d.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(i.positionCS);
                d.shadowMask = half4(1, 1, 1, 1);

                SurfaceData s = (SurfaceData)0;
                s.albedo = saturate(albedo);
                s.metallic = 0;
                s.specular = half3(0, 0, 0);
                s.smoothness = _Smoothness * lerp(1, 1.6, rockMask);
                s.occlusion = lerp(1, .82, rockMask);
                s.normalTS = nTS;
                s.alpha = 1;

                half4 c = UniversalFragmentPBR(d, s);
                c.rgb = MixFog(c.rgb, d.fogCoord);
                return half4(c.rgb, 1);
            }
            ENDHLSL
        }

        UsePass "Universal Render Pipeline/Lit/SHADOWCASTER"
        UsePass "Universal Render Pipeline/Lit/DEPTHONLY"
        UsePass "Universal Render Pipeline/Lit/DEPTHNORMALS"
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}

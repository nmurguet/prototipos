// Full-screen heat-distortion post-process.
// Used by HeatDistortionFeature.cs (URP Renderer Feature).
// AtmosphericEntry.cs sets the global properties each frame:
//   _HeatIntensity  — 0-1 normalised entry intensity
//   _HeatShipPos    — xy = ship viewport UV (0-1), zw = velocity direction in viewport space
Shader "Hidden/HeatDistortion"
{
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        ZWrite Off
        ZTest  Always
        Cull   Off

        Pass
        {
            Name "HeatDistortion"

            HLSLPROGRAM
            #pragma vertex   Vert
            #pragma fragment Frag

            // Blit.hlsl provides: Vert, Varyings (with texcoord), _BlitTexture, sampler_LinearClamp
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            // Set globally by AtmosphericEntry.cs
            float  _HeatIntensity;
            float4 _HeatShipPos; // xy = ship UV pos, zw = velocity dir in UV space

            // ---- value noise helpers ----

            float2 Hash2(float2 p)
            {
                p = float2(dot(p, float2(127.1, 311.7)),
                           dot(p, float2(269.5, 183.3)));
                return frac(sin(p) * 43758.5453);
            }

            float VNoise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);   // smooth Hermite
                float a = Hash2(i             ).x;
                float b = Hash2(i + float2(1, 0)).x;
                float c = Hash2(i + float2(0, 1)).x;
                float d = Hash2(i + float2(1, 1)).x;
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }

            // ---- fragment ----

            float4 Frag(Varyings IN) : SV_Target
            {
                float2 uv     = IN.texcoord.xy;
                float2 shipUV = _HeatShipPos.xy;
                float2 velDir = _HeatShipPos.zw;

                // Bias the heat zone ahead of the ship in its direction of travel.
                float2 aheadUV = shipUV + velDir * 0.05;
                float  dist    = length(uv - aheadUV);

                // Intensity zone: 0.22 of viewport radius, shaped by entry heat.
                float zone = saturate(1.0 - dist / 0.22) * _HeatIntensity;

                // Early-out: no distortion outside the zone.
                if (zone < 0.002)
                    return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

                float t = _Time.y;

                // Two octaves of animated noise for a natural shimmer.
                float2 n1 = float2(VNoise(uv * 9.0 + float2(t * 0.80, t * 0.35)) - 0.5,
                                   VNoise(uv * 9.0 + float2(t * 0.40, t * 0.90)) - 0.5);
                float2 n2 = float2(VNoise(uv * 4.5 + float2(t * 0.25, t * 0.55)) - 0.5,
                                   VNoise(uv * 4.5 + float2(t * 0.65, t * 0.15)) - 0.5);

                float2 distortion = (n1 * 0.65 + n2 * 0.35) * zone * 0.022;

                return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + distortion);
            }
            ENDHLSL
        }
    }
}

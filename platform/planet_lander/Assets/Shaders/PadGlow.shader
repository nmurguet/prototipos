// Additive glow for landing pad indicator lights.
// Additive blending means the light "adds" to whatever is behind it,
// producing a bloom-like effect without a post-process pass.
// LandingPad.cs updates _GlowColor and _GlowIntensity each frame on the
// per-pad material instance, so this shader intentionally skips the
// SRP-Batcher CBUFFER (incompatible with per-frame material.SetFloat).
Shader "Custom/PadGlow"
{
    Properties
    {
        _GlowColor    ("Glow Color",  Color)        = (0.30, 1.00, 0.42, 1.0)
        _GlowIntensity("Intensity",   Float)        = 1.5
    }

    SubShader
    {
        Tags
        {
            "Queue"          = "Transparent"
            "RenderType"     = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        // Additive blending: dst = dst + src*srcAlpha  →  lights always brighten the scene.
        Blend One One
        ZWrite Off
        Cull   Off

        Pass
        {
            Name "PadGlow"

            HLSLPROGRAM
            #pragma vertex   Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 posOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings   { float4 posCS : SV_POSITION; float2 uv : TEXCOORD0; };

            // No CBUFFER_START — properties are updated per-frame via material.SetFloat/SetColor.
            float4 _GlowColor;
            float  _GlowIntensity;

            Varyings Vert(Attributes IN)
            {
                Varyings o;
                o.posCS = TransformObjectToHClip(IN.posOS.xyz);
                o.uv    = IN.uv;
                return o;
            }

            float4 Frag(Varyings IN) : SV_Target
            {
                // LineRenderer UV: x goes 0→1 along length, y goes 0→1 across width.
                // For zero-length lines (dot-lights), use both axes together so the
                // falloff is radially symmetric around the centre of the billboard.
                float2 centered = IN.uv - 0.5;
                float  dist     = length(centered) * 2.0;   // 0=centre, 1=corner

                // Sharp inner core  +  wide soft halo (both add together).
                float core = pow(saturate(1.0 - dist), 2.0);        // tight bright spot
                float halo = pow(saturate(1.0 - dist), 0.35) * 0.4; // diffuse glow radius

                float brightness = (core + halo) * _GlowIntensity;
                // Alpha=1 with additive blend: the blend equation handles transparency.
                return float4(_GlowColor.rgb * brightness, 1.0);
            }
            ENDHLSL
        }
    }
}

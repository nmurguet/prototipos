Shader "Custom/PlanetAtmosphere"
{
    Properties
    {
        _AtmoColor   ("Atmosphere Color", Color) = (0.3, 0.6, 1.0, 0.6)
        // Where the planet surface sits in UV space (planet.radius / planet.atmosphereRadius).
        // Set from Planet.cs at build time.
        _InnerRadius ("Inner Radius", Float) = 0.5
        // Scales overall opacity — driven by planet.atmosphereDrag.
        _Intensity   ("Intensity",    Float) = 1.0
    }

    SubShader
    {
        // Draw behind the planet body (sortingOrder = -1 on the MeshRenderer).
        Tags
        {
            "Queue"          = "Transparent"
            "RenderType"     = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "PlanetAtmosphere"

            HLSLPROGRAM
            #pragma vertex   Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 posOS : POSITION;
                float2 uv    : TEXCOORD0;
            };

            struct Varyings
            {
                float4 posCS : SV_POSITION;
                float2 uv    : TEXCOORD0;
            };

            // SRP-Batcher compatible block (properties set once at planet build time).
            CBUFFER_START(UnityPerMaterial)
                float4 _AtmoColor;
                float  _InnerRadius;
                float  _Intensity;
            CBUFFER_END

            Varyings Vert(Attributes IN)
            {
                Varyings o;
                o.posCS = TransformObjectToHClip(IN.posOS.xyz);
                o.uv    = IN.uv;
                return o;
            }

            float4 Frag(Varyings IN) : SV_Target
            {
                // dist: 0 = planet centre, 1 = edge of atmosphere quad.
                float dist = length(IN.uv - 0.5) * 2.0;

                // Fully transparent inside the planet body.
                if (dist < _InnerRadius) return float4(0, 0, 0, 0);

                // t: 0 = planet surface, 1 = outer atmosphere edge.
                float t = saturate((dist - _InnerRadius) / (1.0 - _InnerRadius));

                // Wide atmospheric scatter — exponential falloff from the surface outward.
                float scatter = exp(-t * 3.5);

                // Narrow bright limb right above the surface (like a glow halo at the horizon).
                float limb = exp(-t * 22.0) * 0.75;

                float alpha = (scatter + limb) * _Intensity * _AtmoColor.a;
                return float4(_AtmoColor.rgb, saturate(alpha));
            }
            ENDHLSL
        }
    }
}

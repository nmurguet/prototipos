using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

/// URP Renderer Feature that applies full-screen heat distortion during atmospheric entry.
///
/// SETUP (one-time):
///   1. Select your URP Renderer asset (Project Settings → Graphics → Scriptable Renderer).
///   2. In its Inspector click "Add Renderer Feature" and choose "Heat Distortion Feature".
///   3. Make sure "Opaque Texture" (aka _CameraOpaqueTexture) is enabled on the URP Asset.
///
/// AtmosphericEntry.cs drives the effect by setting two global shader properties each frame:
///   _HeatIntensity  — 0-1 normalised entry intensity  (0 = no effect, skip pass entirely)
///   _HeatShipPos    — float4 (shipViewportUV.xy, velocityDirViewport.zw)
public class HeatDistortionFeature : ScriptableRendererFeature
{
    HeatDistortionPass _pass;
    Material           _mat;

    static readonly int HeatIntensityId = Shader.PropertyToID("_HeatIntensity");

    // ------------------------------------------------------------------ ScriptableRendererFeature

    public override void Create()
    {
        var shader = Shader.Find("Hidden/HeatDistortion");
        if (shader == null)
        {
            Debug.LogWarning("[HeatDistortionFeature] Shader 'Hidden/HeatDistortion' not found. " +
                             "Make sure HeatDistortion.shader is inside Assets/.");
            return;
        }
        _mat  = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
        _pass = new HeatDistortionPass(_mat);
        _pass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (_pass == null) return;
        // Skip if intensity is essentially zero — avoids the blit cost entirely.
        if (Shader.GetGlobalFloat(HeatIntensityId) < 0.005f) return;
        renderer.EnqueuePass(_pass);
    }

    protected override void Dispose(bool disposing) => CoreUtils.Destroy(_mat);

    // ------------------------------------------------------------------ inner pass

    class HeatDistortionPass : ScriptableRenderPass
    {
        readonly Material _mat;
        static readonly int HeatIntensityId = Shader.PropertyToID("_HeatIntensity");

        public HeatDistortionPass(Material mat) { _mat = mat; }

        // RenderGraph allocates one of these per frame; no manual pooling needed.
        class PassData
        {
            public TextureHandle src;
            public Material      mat;
        }

        public override void RecordRenderGraph(RenderGraph rg, ContextContainer frameData)
        {
            if (_mat == null) return;
            if (Shader.GetGlobalFloat(HeatIntensityId) < 0.005f) return;

            var res = frameData.Get<UniversalResourceData>();

            // Can't blit when rendering directly to the backbuffer (no intermediate RT).
            if (res.isActiveTargetBackBuffer) return;

            TextureHandle src = res.activeColorTexture;

            // Create a temporary texture with the same format as the camera target.
            var camData = frameData.Get<UniversalCameraData>();
            var rtDesc  = camData.cameraTargetDescriptor;
            rtDesc.depthBufferBits = 0;
            rtDesc.msaaSamples     = 1;

            TextureHandle tmp = rg.CreateTexture(new TextureDesc(rtDesc.width, rtDesc.height)
            {
                colorFormat = rtDesc.graphicsFormat,
                filterMode  = FilterMode.Bilinear,
                name        = "_HeatDistortionTemp",
            });

            // --- Pass A: apply heat distortion shader  src → tmp ---
            using (var builder = rg.AddRasterRenderPass<PassData>("HeatDistort_Apply", out var pd))
            {
                pd.src = src;
                pd.mat = _mat;
                builder.UseTexture(src);                  // declare read
                builder.SetRenderAttachment(tmp, 0);       // declare write
                builder.AllowPassCulling(false);
                builder.SetRenderFunc(static (PassData d, RasterGraphContext ctx) =>
                    Blitter.BlitTexture(ctx.cmd, d.src, new Vector4(1, 1, 0, 0), d.mat, 0));
            }

            // --- Pass B: copy tmp back to the active color texture ---
            using (var builder = rg.AddRasterRenderPass<PassData>("HeatDistort_Copy", out var pd))
            {
                pd.src = tmp;
                pd.mat = null;
                builder.UseTexture(tmp);                   // declare read
                builder.SetRenderAttachment(src, 0);       // declare write
                builder.AllowPassCulling(false);
                builder.SetRenderFunc(static (PassData d, RasterGraphContext ctx) =>
                    Blitter.BlitTexture(ctx.cmd, d.src, new Vector4(1, 1, 0, 0), 0, false));
            }
        }
    }
}

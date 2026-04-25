using UnityEngine;

public class ParallaxStars : MonoBehaviour
{
    struct Layer { public Vector2[] anchors; public float parallax; public float size; }

    static readonly (float f, int n, float s)[] Cfg =
    {
        (0.04f, 180, 1.0f),
        (0.16f, 180, 1.4f),
        (0.38f,  80, 2.0f),
    };

    const float Spread = 200000f;

    Layer[]  _layers;
    Material _mat;
    Camera   _cam;

    void Awake()
    {
        _mat = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default")
                         ?? Shader.Find("Sprites/Default"));
        _cam = Camera.main;

        var rng = new System.Random(12345);
        _layers = new Layer[Cfg.Length];
        for (int li = 0; li < Cfg.Length; li++)
        {
            var (f, n, s) = Cfg[li];
            var anchors = new Vector2[n];
            for (int i = 0; i < n; i++)
                anchors[i] = new Vector2(
                    (float)(rng.NextDouble() * 2 - 1) * Spread,
                    (float)(rng.NextDouble() * 2 - 1) * Spread);
            _layers[li] = new Layer { anchors = anchors, parallax = f, size = s };
        }
    }

    void OnRenderObject()
    {
        if (_mat == null || _cam == null || Camera.current != _cam) return;

        _mat.SetPass(0);
        GL.PushMatrix();
        GL.LoadProjectionMatrix(_cam.projectionMatrix);
        GL.modelview = _cam.worldToCameraMatrix;

        Vector2 camPos = _cam.transform.position;
        GL.Begin(GL.QUADS);
        foreach (var layer in _layers)
        {
            float s = layer.size;
            for (int i = 0; i < layer.anchors.Length; i++)
            {
                // draw_pos = anchor * parallax + cam_pos * (1 - parallax)
                Vector2 draw = layer.anchors[i] * layer.parallax + camPos * (1f - layer.parallax);

                // Tile so stars always cover the visible area
                float tile = Spread * 2f;
                draw.x = Mathf.Repeat(draw.x - camPos.x + Spread, tile) + camPos.x - Spread;
                draw.y = Mathf.Repeat(draw.y - camPos.y + Spread, tile) + camPos.y - Spread;

                float bright = 0.4f + 0.6f * (i % 3) / 2f;
                GL.Color(new Color(bright, bright, bright + 0.1f, 1f));
                GL.Vertex3(draw.x - s, draw.y - s, 0);
                GL.Vertex3(draw.x + s, draw.y - s, 0);
                GL.Vertex3(draw.x + s, draw.y + s, 0);
                GL.Vertex3(draw.x - s, draw.y + s, 0);
            }
        }
        GL.End();
        GL.PopMatrix();
    }
}

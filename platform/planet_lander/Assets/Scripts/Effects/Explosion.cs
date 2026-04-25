using UnityEngine;
using System.Collections.Generic;

public class Explosion : MonoBehaviour
{
    struct Ring  { public float radius, maxR, life, maxLife; public Color color; }
    struct Shard { public Vector2 pos, vel; public float life; }

    readonly List<Ring>  _rings  = new List<Ring>();
    readonly List<Shard> _shards = new List<Shard>();
    Material _mat;
    float    _flashLife;

    public void Initialize(Vector2 pos, bool fatal)
    {
        transform.position = pos;
        _mat       = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default")
                               ?? Shader.Find("Sprites/Default"));
        _flashLife = fatal ? 0.35f : 0.18f;

        int rings = fatal ? 3 : 2;
        for (int i = 0; i < rings; i++)
        {
            float maxR = fatal ? Random.Range(180f, 380f) : Random.Range(80f, 160f);
            float life = fatal ? 0.70f : 0.45f;
            _rings.Add(new Ring { radius=0, maxR=maxR, life=life, maxLife=life,
                color = i == 0 ? new Color(1f,0.9f,0.4f) : new Color(1f,0.4f,0.1f) });
        }

        int n = fatal ? 14 : 8;
        for (int i = 0; i < n; i++)
        {
            float a = Random.Range(0f, Mathf.PI * 2f);
            float s = Random.Range(80f, fatal ? 400f : 220f);
            float l = Random.Range(0.4f, fatal ? 1.4f : 0.8f);
            _shards.Add(new Shard {
                pos  = pos,
                vel  = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * s,
                life = l,
            });
        }

        Destroy(gameObject, 1.8f);
    }

    void Update()
    {
        float dt = Time.deltaTime;
        _flashLife -= dt;

        for (int i = _rings.Count - 1; i >= 0; i--)
        {
            var r = _rings[i];
            r.life  -= dt;
            r.radius = (1f - r.life / r.maxLife) * r.maxR;
            if (r.life <= 0f) { _rings.RemoveAt(i); continue; }
            _rings[i] = r;
        }

        for (int i = _shards.Count - 1; i >= 0; i--)
        {
            var s = _shards[i];
            s.life -= dt;
            s.pos  += s.vel * dt;
            s.vel  *= 0.94f;
            if (s.life <= 0f) { _shards.RemoveAt(i); continue; }
            _shards[i] = s;
        }
    }

    void OnRenderObject()
    {
        if (_mat == null || Camera.current != Camera.main || Camera.main == null) return;
        _mat.SetPass(0);
        GL.PushMatrix();
        GL.LoadProjectionMatrix(Camera.main.projectionMatrix);
        GL.modelview = Camera.main.worldToCameraMatrix;

        // Flash quad
        if (_flashLife > 0f)
        {
            float a = _flashLife / (_flashLife > 0.18f ? 0.35f : 0.18f);
            float s = (_rings.Count >= 3 ? 200f : 80f) * a;
            GL.Begin(GL.QUADS);
            GL.Color(new Color(1f, 0.95f, 0.7f, a * 0.8f));
            Vector3 c = transform.position;
            GL.Vertex(c + new Vector3(-s,-s,0)); GL.Vertex(c + new Vector3(s,-s,0));
            GL.Vertex(c + new Vector3(s, s,0)); GL.Vertex(c + new Vector3(-s, s,0));
            GL.End();
        }

        // Rings
        const int segs = 32;
        GL.Begin(GL.LINES);
        foreach (var r in _rings)
        {
            float t = 1f - r.life / r.maxLife;
            GL.Color(new Color(r.color.r, r.color.g, r.color.b, (1f - t * t) * 0.9f));
            Vector3 p = transform.position;
            for (int i = 0; i < segs; i++)
            {
                float a0 = i       * Mathf.PI * 2f / segs;
                float a1 = (i + 1) * Mathf.PI * 2f / segs;
                GL.Vertex(p + new Vector3(Mathf.Cos(a0)*r.radius, Mathf.Sin(a0)*r.radius, 0));
                GL.Vertex(p + new Vector3(Mathf.Cos(a1)*r.radius, Mathf.Sin(a1)*r.radius, 0));
            }
        }
        GL.End();

        // Debris trails
        GL.Begin(GL.LINES);
        foreach (var s in _shards)
        {
            GL.Color(new Color(1f, 0.6f, 0.2f, s.life * s.life));
            Vector3 head = s.pos;
            Vector3 tail = head - new Vector3(s.vel.x * 0.06f, s.vel.y * 0.06f, 0f);
            GL.Vertex(head); GL.Vertex(tail);
        }
        GL.End();

        GL.PopMatrix();
    }
}

using UnityEngine;
using System.Collections.Generic;

// Custom smoke — no Unity ParticleSystem
public class EngineSmoke : MonoBehaviour
{
    struct Particle
    {
        public Vector2 pos, vel;
        public float   life, maxLife, size;
    }

    const int   Max       = 120;
    const float Rate      = 36f;
    const float Spread    = 7f;

    readonly List<Particle> _particles = new List<Particle>(Max);
    Material _mat;
    float    _accum;
    bool     _emitting;

    static readonly Vector2 NozzleLocal = new Vector2(0f, -13f);

    void Awake()
    {
        _mat = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default")
                         ?? Shader.Find("Sprites/Default"));
    }

    public void Emit(bool on) => _emitting = on;

    void FixedUpdate()
    {
        var ship = GetComponent<ShipController>();

        if (_emitting && ship.Fuel > 0f)
        {
            _accum += Rate * Time.fixedDeltaTime;
            while (_accum >= 1f && _particles.Count < Max)
            {
                _accum -= 1f;
                SpawnParticle(ship);
            }
            if (_accum >= 1f) _accum = 0f;
        }
        else _accum = 0f;

        for (int i = _particles.Count - 1; i >= 0; i--)
        {
            var p = _particles[i];
            p.life -= Time.fixedDeltaTime;
            p.pos  += p.vel * Time.fixedDeltaTime;
            p.vel  *= 0.96f;
            if (p.life <= 0f) { _particles.RemoveAt(i); continue; }
            _particles[i] = p;
        }
    }

    void SpawnParticle(ShipController ship)
    {
        Vector2 nozzle = (Vector2)transform.position
                       + (Vector2)(transform.rotation * NozzleLocal);

        float baseAngle = Mathf.Atan2(-transform.up.y, -transform.up.x);
        float spread    = Random.Range(-Spread, Spread) * Mathf.Deg2Rad;
        float speed     = Random.Range(60f, 160f);
        float a         = baseAngle + spread;
        float life      = Random.Range(0.7f, 1.1f);

        _particles.Add(new Particle {
            pos     = nozzle,
            vel     = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * speed + ship.Rb.linearVelocity * 0.4f,
            life    = life,
            maxLife = life,
            size    = Random.Range(2.5f, 5f),
        });
    }

    void OnRenderObject()
    {
        if (_particles.Count == 0 || Camera.current != Camera.main || Camera.main == null) return;

        _mat.SetPass(0);
        GL.PushMatrix();
        GL.LoadProjectionMatrix(Camera.main.projectionMatrix);
        GL.modelview = Camera.main.worldToCameraMatrix;

        GL.Begin(GL.QUADS);
        foreach (var p in _particles)
        {
            float t = p.life / p.maxLife;
            float s = p.size * (1f + (1f - t) * 0.8f);
            GL.Color(new Color(0.7f, 0.7f, 0.7f, t * t * 0.55f));
            Vector3 pos = p.pos;
            GL.Vertex(pos + new Vector3(-s, -s, 0));
            GL.Vertex(pos + new Vector3( s, -s, 0));
            GL.Vertex(pos + new Vector3( s,  s, 0));
            GL.Vertex(pos + new Vector3(-s,  s, 0));
        }
        GL.End();
        GL.PopMatrix();
    }
}

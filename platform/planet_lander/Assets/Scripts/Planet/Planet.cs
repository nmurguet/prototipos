using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(PolygonCollider2D))]
public class Planet : MonoBehaviour
{
    [SerializeField] PlanetConfig _config;   // serializado → persiste en escena pre-built
    public PlanetConfig Config => _config;

    const int TerrainPoints = 128;
    float[] _radii;
    float[] _phases;

    // Live-tunable in Inspector (override Config values at runtime)
    [Header("Gravity (live)")]
    [SerializeField] float gravityOverride     = -1f;   // -1 = use Config
    [SerializeField] float gravityCapOverride  = 260f;
    [SerializeField] float dragOverride        = -1f;   // -1 = use Config
    [SerializeField] float dragScaleOverride   = 0.35f;

    float SurfaceGravity => gravityOverride  >= 0f ? gravityOverride  : Config.surfaceGravity;
    float AtmoDrag       => dragOverride     >= 0f ? dragOverride     : Config.atmosphereDrag;

    // ------------------------------------------------------------------ init

    void Awake()
    {
        // Escena pre-built: _config está serializado pero _radii no → regenerar
        if (_radii == null && _config != null)
            GenerateTerrain();
    }

    public void Initialize(PlanetConfig cfg)
    {
        _config = cfg;
        transform.position = cfg.position;
        gameObject.name    = cfg.name;

        GenerateTerrain();
        BuildMesh();
        BuildCollider();
        BuildAtmosphere();

        GameState.Instance?.RegisterPlanet(this);
    }

    void OnDestroy() => GameState.Instance?.UnregisterPlanet(this);

    // ------------------------------------------------------------------ terrain

    void GenerateTerrain()
    {
        int seed = 0;
        foreach (char c in Config.name) seed = seed * 31 + c;
        var rng = new System.Random(seed);

        var harmonics = PlanetConfig.Styles[Config.terrainStyle];
        _phases = new float[harmonics.Length];
        for (int i = 0; i < _phases.Length; i++)
            _phases[i] = (float)(rng.NextDouble() * Mathf.PI * 2);

        _radii = new float[TerrainPoints];
        for (int i = 0; i < TerrainPoints; i++)
            _radii[i] = ComputeRadius(i * Mathf.PI * 2f / TerrainPoints);
    }

    float ComputeRadius(float angle)
    {
        var harmonics = PlanetConfig.Styles[Config.terrainStyle];
        float offset = 0f;
        for (int j = 0; j < harmonics.Length; j++)
            offset += Mathf.Sin(harmonics[j].frequency * angle + _phases[j]) * harmonics[j].amplitude;
        return Config.radius * (1f + Config.roughness * offset);
    }

    public float GetSurfaceRadiusAt(float angle)
    {
        angle = ((angle % (Mathf.PI * 2f)) + Mathf.PI * 2f) % (Mathf.PI * 2f);
        float t  = angle / (Mathf.PI * 2f) * TerrainPoints;
        int   i0 = (int)t % TerrainPoints;
        int   i1 = (i0 + 1) % TerrainPoints;
        return Mathf.Lerp(_radii[i0], _radii[i1], t - Mathf.Floor(t));
    }

    public Vector2 GetSurfacePoint(float angle)
    {
        float r = GetSurfaceRadiusAt(angle);
        return (Vector2)transform.position + new Vector2(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r);
    }

    // ------------------------------------------------------------------ mesh

    void BuildMesh()
    {
        var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default")
                  ?? Shader.Find("Sprites/Default");
        var mat = new Material(shader) { color = Config.color };
        GetComponent<MeshRenderer>().material = mat;

        var mesh  = new Mesh { name = Config.name };
        var verts = new Vector3[TerrainPoints + 1];
        verts[0]  = Vector3.zero;
        for (int i = 0; i < TerrainPoints; i++)
        {
            float a  = i * Mathf.PI * 2f / TerrainPoints;
            verts[i + 1] = new Vector3(Mathf.Cos(a) * _radii[i], Mathf.Sin(a) * _radii[i], 0f);
        }
        var tris = new int[TerrainPoints * 3];
        for (int i = 0; i < TerrainPoints; i++)
        {
            tris[i * 3]     = 0;
            tris[i * 3 + 1] = i + 1;
            tris[i * 3 + 2] = (i + 1) % TerrainPoints + 1;
        }
        mesh.vertices  = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;

        // Outline — thin line, centered on surface; collider will be offset outward by half-width
        float lineWidth = Mathf.Clamp(Config.radius * 0.004f, 3f, 14f);
        var lr  = gameObject.AddComponent<LineRenderer>();
        var outC = Config.color * 1.3f; outC.a = 1f;
        lr.useWorldSpace = false;
        lr.loop          = true;
        lr.positionCount = TerrainPoints;
        lr.startColor    = lr.endColor   = outC;
        lr.startWidth    = lr.endWidth   = lineWidth;
        lr.material      = new Material(shader);
        lr.sortingOrder  = 1;
        for (int i = 0; i < TerrainPoints; i++)
        {
            float a = i * Mathf.PI * 2f / TerrainPoints;
            lr.SetPosition(i, new Vector3(Mathf.Cos(a) * _radii[i], Mathf.Sin(a) * _radii[i], 0f));
        }

        _outlineHalfWidth = lineWidth * 0.5f;
    }

    float _outlineHalfWidth;

    void BuildCollider()
    {
        // Push collider outward by half the line width so the outer edge of the
        // visual line aligns exactly with the physics boundary.
        float offset = _outlineHalfWidth;
        var path = new Vector2[TerrainPoints];
        for (int i = 0; i < TerrainPoints; i++)
        {
            float a = i * Mathf.PI * 2f / TerrainPoints;
            float r = _radii[i] + offset;
            path[i] = new Vector2(Mathf.Cos(a) * r, Mathf.Sin(a) * r);
        }
        GetComponent<PolygonCollider2D>().SetPath(0, path);
    }

    void BuildAtmosphere()
    {
        var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default")
                  ?? Shader.Find("Sprites/Default");
        var go = new GameObject("Atmosphere");
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;

        const int segs = 64;
        var lr   = go.AddComponent<LineRenderer>();
        var c    = Config.color;
        lr.useWorldSpace = false;
        lr.loop          = true;
        lr.positionCount = segs;
        lr.startColor    = lr.endColor  = new Color(c.r, c.g, c.b, 0.10f);
        lr.startWidth    = lr.endWidth  = Config.atmosphereRadius * 0.06f;
        lr.material      = new Material(shader);
        lr.sortingOrder  = -1;
        for (int i = 0; i < segs; i++)
        {
            float a = i * Mathf.PI * 2f / segs;
            lr.SetPosition(i, new Vector3(Mathf.Cos(a) * Config.atmosphereRadius,
                                          Mathf.Sin(a) * Config.atmosphereRadius, 0f));
        }
    }

    // ------------------------------------------------------------------ flat-zone detection

    public int[] FindFlatZones(int count)
    {
        float circumference = 2f * Mathf.PI * Config.radius;
        int   winSize       = Mathf.Max(4, Mathf.RoundToInt(54f / circumference * TerrainPoints));

        var variance = new float[TerrainPoints];
        for (int i = 0; i < TerrainPoints; i++)
        {
            float mean = 0f;
            for (int j = 0; j < winSize; j++) mean += _radii[(i + j) % TerrainPoints];
            mean /= winSize;
            float v = 0f;
            for (int j = 0; j < winSize; j++) { float d = _radii[(i + j) % TerrainPoints] - mean; v += d * d; }
            variance[i] = v / winSize;
        }

        var  result  = new List<int>();
        var  used    = new bool[TerrainPoints];
        int  spacing = Mathf.Max(winSize * 2, TerrainPoints / (count + 1));

        for (int n = 0; n < count; n++)
        {
            float best = float.MaxValue; int bestIdx = -1;
            for (int i = 0; i < TerrainPoints; i++)
                if (!used[i] && variance[i] < best) { best = variance[i]; bestIdx = i; }
            if (bestIdx < 0) break;
            result.Add(bestIdx);
            for (int d = -spacing; d <= spacing; d++)
                used[((bestIdx + d) % TerrainPoints + TerrainPoints) % TerrainPoints] = true;
        }
        return result.ToArray();
    }

    // ------------------------------------------------------------------ gravity + drag

    public void ApplyGravityTo(Rigidbody2D rb)
    {
        Vector2 dir  = (Vector2)transform.position - rb.position;
        float   dist = dir.magnitude;
        if (dist < 0.001f || dist > 6f * Config.radius) return;

        float gm    = SurfaceGravity * Config.radius * Config.radius;
        float accel = Mathf.Min(gm / (dist * dist), gravityCapOverride);
        rb.AddForce(dir.normalized * accel * rb.mass);

        if (dist < Config.atmosphereRadius && dist > Config.radius)
        {
            float t          = Mathf.Clamp01(1f - (dist - Config.radius) / (Config.atmosphereRadius - Config.radius));
            float dragFactor = AtmoDrag * t * t * dragScaleOverride;
            rb.linearVelocity *= Mathf.Max(0f, 1f - dragFactor * Time.fixedDeltaTime);
        }
    }
}

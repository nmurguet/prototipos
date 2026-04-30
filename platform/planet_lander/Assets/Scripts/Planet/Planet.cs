using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(PolygonCollider2D))]
public class Planet : MonoBehaviour
{
    [SerializeField] PlanetConfig _config;   // serializado → persiste en escena pre-built
    public PlanetConfig Config => _config;

    const int TerrainPoints = 128;

    // Serialized so that flattened terrain from SceneBuilder is preserved in
    // pre-built scenes. Awake() only regenerates if null (fresh runtime build).
    [SerializeField] float[] _radii;
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
        // Push collider outward by the full line width so the ship stops visibly
        // outside the terrain line, preventing any visual overlap even under
        // physics interpenetration.
        float offset = _outlineHalfWidth * 2f; // = full lineWidth
        var path = new Vector2[TerrainPoints];
        for (int i = 0; i < TerrainPoints; i++)
        {
            float a = i * Mathf.PI * 2f / TerrainPoints;
            float r = _radii[i] + offset;
            path[i] = new Vector2(Mathf.Cos(a) * r, Mathf.Sin(a) * r);
        }
        var col = GetComponent<PolygonCollider2D>();
        col.SetPath(0, path);
        col.sharedMaterial = new PhysicsMaterial2D("PlanetMat") { bounciness = 0f, friction = 0.5f };
    }

    void BuildAtmosphere()
    {
        // Remove stale child if rebuilding (e.g. called from SceneBuilder).
        var old = transform.Find("Atmosphere");
        if (old != null) Destroy(old.gameObject);

        // No atmosphere at all (Ferro, Shadow, etc. are very thin — still show something).
        // Use custom shader when available, fall back to LineRenderer ring.
        var shader = Shader.Find("Custom/PlanetAtmosphere");
        if (shader != null)
            BuildAtmosphereShader(shader);
        else
            BuildAtmosphereFallback();
    }

    void BuildAtmosphereShader(Shader shader)
    {
        var go = new GameObject("Atmosphere");
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;

        float r = Config.atmosphereRadius;

        // Flat quad of size 2r × 2r — the shader uses UV distance from centre for shaping.
        var mesh = new Mesh { name = "AtmoQuad" };
        mesh.vertices  = new Vector3[] {
            new(-r, -r, 0), new(r, -r, 0), new(r, r, 0), new(-r, r, 0) };
        mesh.uv        = new Vector2[] {
            new(0, 0), new(1, 0), new(1, 1), new(0, 1) };
        mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        mesh.RecalculateNormals();
        go.AddComponent<MeshFilter>().mesh = mesh;

        var mr = go.AddComponent<MeshRenderer>();
        mr.sortingOrder          = -1;   // behind planet body and outline
        mr.shadowCastingMode     = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.receiveShadows        = false;

        // Tint the atmosphere toward a brighter, slightly sky-shifted version of the planet colour.
        Color  c       = Config.color;
        Color  atmoCol = new Color(
            Mathf.Lerp(c.r, 0.45f, 0.28f),
            Mathf.Lerp(c.g, 0.70f, 0.28f),
            Mathf.Lerp(c.b, 1.00f, 0.28f),
            0.70f);

        var mat = new Material(shader);
        // Planet surface sits at (radius / atmosphereRadius) in the shader's 0-1 dist space.
        mat.SetColor("_AtmoColor",   atmoCol);
        mat.SetFloat("_InnerRadius", Config.radius / Config.atmosphereRadius);
        mat.SetFloat("_Intensity",   Mathf.Clamp(Config.atmosphereDrag * 0.9f, 0.25f, 2.0f));
        mr.sharedMaterial = mat;
    }

    // LineRenderer ring fallback (used when Custom/PlanetAtmosphere shader is missing).
    void BuildAtmosphereFallback()
    {
        var go = new GameObject("Atmosphere");
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;

        var fallbackShader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default")
                          ?? Shader.Find("Sprites/Default");
        const int segs = 64;
        var lr  = go.AddComponent<LineRenderer>();
        var c   = Config.color;
        lr.useWorldSpace = false;
        lr.loop          = true;
        lr.positionCount = segs;
        lr.startColor    = lr.endColor = new Color(c.r, c.g, c.b, 0.10f);
        lr.startWidth    = lr.endWidth = Config.atmosphereRadius * 0.06f;
        lr.material      = new Material(fallbackShader);
        lr.sortingOrder  = -1;
        for (int i = 0; i < segs; i++)
        {
            float a = i * Mathf.PI * 2f / segs;
            lr.SetPosition(i, new Vector3(Mathf.Cos(a) * Config.atmosphereRadius,
                                          Mathf.Sin(a) * Config.atmosphereRadius, 0f));
        }
    }

    // ------------------------------------------------------------------ flat-zone detection + flattening

    // Keep in sync with LandingPad.PadWidth.
    const float PadWorldWidth = 140f;

    public int[] FindFlatZones(int count)
    {
        // Window covers the full pad arc width plus a margin on each side.
        float padArcFraction = PadWorldWidth / (2f * Mathf.PI * Config.radius);
        int   winSize        = Mathf.Max(4, Mathf.CeilToInt(padArcFraction * TerrainPoints) + 2);

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

            // Return the CENTRE of the flat window, not the start.
            // Previously this returned bestIdx (the window start), placing the pad
            // at the edge of the flat zone instead of the middle.
            int centre = (bestIdx + winSize / 2) % TerrainPoints;
            result.Add(centre);

            for (int d = -spacing; d <= spacing; d++)
                used[((bestIdx + d) % TerrainPoints + TerrainPoints) % TerrainPoints] = true;
        }
        return result.ToArray();
    }

    /// <summary>
    /// Smoothly flattens _radii at each pad centre so the landing platform
    /// always sits on perfectly level terrain. Call this after FindFlatZones
    /// and before building the pad GameObjects.
    /// Also rebuilds mesh + collider so the visual matches.
    /// </summary>
    public void FlattenForPads(int[] padCentres)
    {
        if (padCentres == null || padCentres.Length == 0) return;

        // How many terrain indices the pad arc covers on each side (+1 blend zone)
        float halfArcFrac = (PadWorldWidth * 0.5f) / (2f * Mathf.PI * Config.radius);
        int   halfIdx     = Mathf.Max(2, Mathf.CeilToInt(halfArcFrac * TerrainPoints) + 1);
        int   blendExtra  = Mathf.Max(2, halfIdx);   // smooth transition beyond flat zone

        foreach (int centre in padCentres)
        {
            // Average the radii inside the flat zone
            int   total = halfIdx * 2 + 1;
            float sum   = 0f;
            for (int d = -halfIdx; d <= halfIdx; d++)
                sum += _radii[((centre + d) % TerrainPoints + TerrainPoints) % TerrainPoints];
            float avg = sum / total;

            // Flat zone: lerp fully to avg
            for (int d = -halfIdx; d <= halfIdx; d++)
            {
                int idx = ((centre + d) % TerrainPoints + TerrainPoints) % TerrainPoints;
                _radii[idx] = avg;
            }

            // Blend zone on each side: smooth transition to original terrain
            for (int d = 1; d <= blendExtra; d++)
            {
                float t = 1f - Mathf.SmoothStep(0f, 1f, (float)d / (blendExtra + 1));
                int idxL = ((centre - halfIdx - d) % TerrainPoints + TerrainPoints) % TerrainPoints;
                int idxR = ((centre + halfIdx + d) % TerrainPoints + TerrainPoints) % TerrainPoints;
                _radii[idxL] = Mathf.Lerp(_radii[idxL], avg, t);
                _radii[idxR] = Mathf.Lerp(_radii[idxR], avg, t);
            }
        }

        RefreshGeometry();
    }

    // Rebuilds mesh vertices, outline LR positions, and collider from current _radii.
    // Safe to call multiple times — does not create new components.
    void RefreshGeometry()
    {
        // Mesh vertices (topology/triangles unchanged)
        var mesh = GetComponent<MeshFilter>()?.sharedMesh;
        if (mesh != null)
        {
            var verts = new Vector3[TerrainPoints + 1];
            verts[0] = Vector3.zero;
            for (int i = 0; i < TerrainPoints; i++)
            {
                float a = i * Mathf.PI * 2f / TerrainPoints;
                verts[i + 1] = new Vector3(Mathf.Cos(a) * _radii[i], Mathf.Sin(a) * _radii[i], 0f);
            }
            mesh.vertices = verts;
            mesh.RecalculateNormals();
        }

        // Outline LineRenderer (on this GO, not a child)
        var lr = GetComponent<LineRenderer>();
        if (lr != null)
        {
            for (int i = 0; i < TerrainPoints; i++)
            {
                float a = i * Mathf.PI * 2f / TerrainPoints;
                lr.SetPosition(i, new Vector3(Mathf.Cos(a) * _radii[i], Mathf.Sin(a) * _radii[i], 0f));
            }
        }

        // Collider (reuse BuildCollider which already handles offset)
        BuildCollider();
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

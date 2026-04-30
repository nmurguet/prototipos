using UnityEngine;

/// Engine exhaust smoke.
/// All key parameters are exposed in the Inspector for live tuning.
public class EngineSmoke : MonoBehaviour
{
    // ---- Inspector tunables ----

    [Header("Emission")]
    [Tooltip("Particles emitted per second while thrusting.")]
    [SerializeField] float emitRate         = 36f;
    [Tooltip("Half-angle of the exhaust cone (degrees). 0 = perfectly straight back.")]
    [SerializeField] float spreadAngleDeg   = 7f;
    [Tooltip("Minimum particle exit speed (world units / s).")]
    [SerializeField] float speedMin         = 60f;
    [Tooltip("Maximum particle exit speed.")]
    [SerializeField] float speedMax         = 160f;
    [Tooltip("How much of the ship's own velocity transfers to each particle (0-1).")]
    [SerializeField] float shipVelInfluence = 0.40f;

    [Header("Particle Life")]
    [SerializeField] float lifetimeMin      = 0.65f;
    [SerializeField] float lifetimeMax      = 1.00f;

    [Header("Particle Size")]
    [Tooltip("Size at spawn (random range).")]
    [SerializeField] float sizeMin          = 2.5f;
    [SerializeField] float sizeMax          = 5.5f;
    [Tooltip("Size multiplier at end of life (smoke billows out as it ages).")]
    [SerializeField] float sizeGrowthEnd    = 2.5f;

    [Header("Visual")]
    [Tooltip("Base colour of the smoke. Alpha is ignored here; opacity is driven by the curve below.")]
    [SerializeField] Color smokeColor       = new Color(0.72f, 0.72f, 0.72f);
    [Tooltip("Peak opacity of each smoke puff (0-1).")]
    [SerializeField] float maxAlpha         = 0.55f;
    [SerializeField] int   maxParticles     = 150;

    [Header("Collision")]
    [Tooltip("Particles collide with planet surfaces (2D).")]
    [SerializeField] bool  enableCollision  = true;
    [Tooltip("Speed fraction kept after a collision (0 = stop, 1 = no damping).")]
    [SerializeField] float collisionDampen  = 0.30f;
    [Tooltip("Fraction of remaining lifetime lost on each collision.")]
    [SerializeField] float collisionLifeLoss = 0.20f;

    // ---- private ----

    ParticleSystem _ps;
    ShipController _ship;
    bool           _emitting;
    float          _accum;
    bool           _initialized;

    static readonly Vector2 NozzleLocal = new Vector2(0f, -13f);

    // ------------------------------------------------------------------ lifecycle

    void Awake()
    {
        _ship = GetComponent<ShipController>();
        BuildSystem();
    }

    void BuildSystem()
    {
        // Remove old child if rebuilding
        var old = transform.Find("EngineSmokePS");
        if (old != null) Destroy(old.gameObject);

        var go = new GameObject("EngineSmokePS");
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;

        _ps = go.AddComponent<ParticleSystem>();
        var rdr = go.GetComponent<ParticleSystemRenderer>();

        rdr.material     = MakeAlphaMat();
        rdr.sortingOrder = 3;
        rdr.renderMode   = ParticleSystemRenderMode.Billboard;

        ApplyModules();
        _ps.Play();
        _initialized = true;
    }

    void ApplyModules()
    {
        var main = _ps.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startSpeed      = 0f;
        main.startLifetime   = new ParticleSystem.MinMaxCurve(lifetimeMin, lifetimeMax);
        main.startSize       = new ParticleSystem.MinMaxCurve(sizeMin, sizeMax);
        main.maxParticles    = maxParticles;
        main.loop            = false;
        main.playOnAwake     = false;

        var em = _ps.emission;
        em.enabled = false;   // we emit manually via EmitParams

        // Color over lifetime: base colour with fade-in then fade-out
        var col = _ps.colorOverLifetime;
        col.enabled = true;
        var g = new Gradient();
        g.SetKeys(
            new[] {
                new GradientColorKey(smokeColor, 0f),
                new GradientColorKey(smokeColor * 0.75f, 1f),
            },
            new[] {
                new GradientAlphaKey(0f,        0.00f),
                new GradientAlphaKey(maxAlpha,  0.15f),
                new GradientAlphaKey(maxAlpha * 0.75f, 0.60f),
                new GradientAlphaKey(0f,        1.00f),
            });
        col.color = new ParticleSystem.MinMaxGradient(g);

        // Size over lifetime: starts at 50 %, grows to sizeGrowthEnd ×
        var sz = _ps.sizeOverLifetime;
        sz.enabled = true;
        sz.size = new ParticleSystem.MinMaxCurve(1f,
            new AnimationCurve(new Keyframe(0f, 0.5f), new Keyframe(1f, sizeGrowthEnd)));

        // 2D world collision
        var collision = _ps.collision;
        collision.enabled      = enableCollision;
        collision.type         = ParticleSystemCollisionType.World;
        collision.mode         = ParticleSystemCollisionMode.Collision2D;
        collision.bounce       = new ParticleSystem.MinMaxCurve(0f);
        collision.dampen       = new ParticleSystem.MinMaxCurve(collisionDampen);
        collision.lifetimeLoss = new ParticleSystem.MinMaxCurve(collisionLifeLoss);
        collision.collidesWith = -1;   // all layers
    }

    // Live-update modules when a value changes in the Inspector
#if UNITY_EDITOR
    void OnValidate()
    {
        if (!_initialized || _ps == null) return;
        ApplyModules();
    }
#endif

    // ------------------------------------------------------------------ public API

    public void Emit(bool on) => _emitting = on;

    // ------------------------------------------------------------------ FixedUpdate

    void FixedUpdate()
    {
        if (_ship == null || !_emitting || _ship.Fuel <= 0f) { _accum = 0f; return; }

        _accum += emitRate * Time.fixedDeltaTime;
        while (_accum >= 1f)
        {
            _accum -= 1f;
            EmitOne();
        }
    }

    void EmitOne()
    {
        Vector2 nozzle  = (Vector2)transform.position
                        + (Vector2)(transform.rotation * NozzleLocal);
        Vector2 baseDir = -(Vector2)transform.up;
        float   baseAng = Mathf.Atan2(baseDir.y, baseDir.x);
        float   ang     = baseAng + Random.Range(-spreadAngleDeg, spreadAngleDeg) * Mathf.Deg2Rad;
        float   speed   = Random.Range(speedMin, speedMax);

        var ep = new ParticleSystem.EmitParams();
        ep.position      = nozzle;
        ep.velocity      = new Vector3(Mathf.Cos(ang), Mathf.Sin(ang)) * speed
                         + (Vector3)(Vector2)(_ship.Rb.linearVelocity * shipVelInfluence);
        ep.startLifetime = Random.Range(lifetimeMin, lifetimeMax);
        ep.startSize     = Random.Range(sizeMin, sizeMax);
        ep.startColor    = smokeColor;
        _ps.Emit(ep, 1);
    }

    // ------------------------------------------------------------------ material

    static Material MakeAlphaMat()
    {
        var shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        var mat = new Material(shader);
        SetTransparent(mat, additive: false);
        return mat;
    }

    internal static void SetTransparent(Material mat, bool additive)
    {
        // URP Particles/Unlit surface setup
        mat.SetFloat("_Surface", 1f);           // Transparent
        mat.SetFloat("_Blend",   additive ? 2f : 0f);  // 2=Additive, 0=Alpha
        mat.SetFloat("_ZWrite",  0f);
        mat.SetInt("_SrcBlend",
            (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", additive
            ? (int)UnityEngine.Rendering.BlendMode.One
            : (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        if (additive) mat.EnableKeyword("_BLENDMODE_ADDITIVE");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }
}

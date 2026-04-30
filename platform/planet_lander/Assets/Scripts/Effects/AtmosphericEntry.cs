using UnityEngine;

/// Atmospheric re-entry: bow-shock arc + plasma trail + ablation smoke.
/// All parameters exposed in the Inspector. Particles collide with planets (2D).
public class AtmosphericEntry : MonoBehaviour
{
    // ------------------------------------------------------------------ Inspector

    [Header("Intensity Thresholds  (intensity = speed × atmoDensity × planetDrag)")]
    [Tooltip("Entry intensity at which ablation smoke begins.")]
    [SerializeField] float smokeThreshold  =  80f;
    [Tooltip("Entry intensity at which plasma and bow-shock appear.")]
    [SerializeField] float plasmaThreshold = 220f;
    [Tooltip("Intensity considered 'maximum' — effects are fully saturated here.")]
    [SerializeField] float maxIntensity    = 700f;

    [Header("Height Cutoff")]
    [Tooltip("Effects fade to zero below this fraction of atmosphere height.\n"
           + "0 = surface, 1 = top of atmosphere.\n"
           + "e.g. 0.25 means the effect is gone when the ship is in the lowest "
           + "25 % of the atmosphere (near the ground).")]
    [Range(0f, 1f)]
    [SerializeField] float surfaceFadeFraction = 0.25f;

    [Header("Plasma Particles")]
    [Tooltip("Max emission rate (particles / second at full intensity).")]
    [SerializeField] float plasmaRateMax       = 90f;
    [Tooltip("Spread cone half-angle (degrees).")]
    [SerializeField] float plasmaSpreadDeg     = 45f;
    [Tooltip("Particle speed as a fraction of the ship's speed (range).")]
    [SerializeField] float plasmaSpeedMin      = 0.20f;
    [SerializeField] float plasmaSpeedMax      = 0.65f;
    [SerializeField] float plasmaLifetimeMin   = 0.18f;
    [SerializeField] float plasmaLifetimeMax   = 0.45f;
    [SerializeField] float plasmaSizeMin       = 2.5f;
    [SerializeField] float plasmaSizeMax       = 7.0f;
    [SerializeField] int   plasmaMaxParticles  = 200;

    [Header("Ablation Smoke Particles")]
    [SerializeField] float smokeRateMax        = 45f;
    [Tooltip("Spread cone half-angle (degrees).")]
    [SerializeField] float smokeSpreadDeg      = 60f;
    [SerializeField] float smokeSpeedMin       = 0.05f;
    [SerializeField] float smokeSpeedMax       = 0.28f;
    [SerializeField] float smokeLifetimeMin    = 0.55f;
    [SerializeField] float smokeLifetimeMax    = 1.10f;
    [SerializeField] float smokeSizeMin        = 8f;
    [SerializeField] float smokeSizeMax        = 20f;
    [Tooltip("Peak opacity of each smoke puff.")]
    [SerializeField] float smokeMaxAlpha       = 0.50f;
    [SerializeField] int   smokeMaxParticles   = 150;

    [Header("Collision")]
    [SerializeField] bool  enableCollision     = true;
    [SerializeField] float collisionDampen     = 0.40f;
    [SerializeField] float collisionLifeLoss   = 0.20f;

    [Header("Bow Shock")]
    [SerializeField] float bowShockArcDeg      = 70f;
    [SerializeField] float bowShockForwardDist = 18f;
    [SerializeField] float bowShockRadiusMin   = 14f;
    [SerializeField] float bowShockRadiusMax   = 34f;
    [SerializeField] float bowShockWidthMin    = 2.5f;
    [SerializeField] float bowShockWidthMax    = 9.0f;

    // ------------------------------------------------------------------ Private

    ParticleSystem _plasma;
    ParticleSystem _smoke;
    LineRenderer   _bowShock;
    ShipController _ship;

    float _intensity;
    float _plasmaAccum;
    float _smokeAccum;
    bool  _initialized;

    // ------------------------------------------------------------------ Lifecycle

    void Awake()
    {
        _ship = GetComponent<ShipController>();
        BuildSystems();
    }

    void BuildSystems()
    {
        DestroyChild("PlasmaTrailPS");
        DestroyChild("SmokeTrailPS");
        DestroyChild("BowShockGO");

        var partShader = Shader.Find("Universal Render Pipeline/Particles/Unlit")
                      ?? Shader.Find("Sprites/Default");

        _plasma   = BuildPS("PlasmaTrailPS",  partShader, additive: true,
                            plasmaMaxParticles, BuildPlasmaModules);
        _smoke    = BuildPS("SmokeTrailPS",   partShader, additive: false,
                            smokeMaxParticles,  BuildSmokeModules);
        _bowShock = BuildBowShock();

        _initialized = true;
    }

    void DestroyChild(string n) { var t = transform.Find(n); if (t) Destroy(t.gameObject); }

    // ------------------------------------------------------------------ OnValidate

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!_initialized) return;
        if (_plasma != null) { BuildPlasmaModules(_plasma); }
        if (_smoke  != null) { BuildSmokeModules(_smoke);   }
    }
#endif

    // ------------------------------------------------------------------ FixedUpdate

    void FixedUpdate()
    {
        if (_ship == null || _ship.IsDead) { _intensity = 0f; return; }

        _intensity = ComputeIntensity();
        float norm = Mathf.Clamp01(_intensity / maxIntensity);

        Vector2 vel    = _ship.Rb.linearVelocity;
        float   speed  = vel.magnitude;
        Vector2 velDir = speed > 0.01f ? vel / speed : Vector2.up;
        Vector2 right  = new Vector2(-velDir.y, velDir.x);

        // Plasma
        if (_intensity > plasmaThreshold)
        {
            float frac = Mathf.Clamp01((_intensity - plasmaThreshold)
                                       / (maxIntensity - plasmaThreshold));
            _plasmaAccum += Mathf.Lerp(5f, plasmaRateMax, frac) * Time.fixedDeltaTime;
            while (_plasmaAccum >= 1f)
            {
                _plasmaAccum -= 1f;
                EmitPlasma(velDir, right, speed);
            }
        }
        else _plasmaAccum = 0f;

        // Smoke
        if (_intensity > smokeThreshold)
        {
            float frac = Mathf.Clamp01((_intensity - smokeThreshold)
                                       / (maxIntensity - smokeThreshold));
            _smokeAccum += Mathf.Lerp(4f, smokeRateMax, frac) * Time.fixedDeltaTime;
            while (_smokeAccum >= 1f)
            {
                _smokeAccum -= 1f;
                EmitSmoke(velDir, right, speed);
            }
        }
        else _smokeAccum = 0f;
    }

    // ------------------------------------------------------------------ Update (visual)

    // Global property IDs — cached to avoid repeated string lookups.
    static readonly int HeatIntensityId = Shader.PropertyToID("_HeatIntensity");
    static readonly int HeatShipPosId   = Shader.PropertyToID("_HeatShipPos");

    void Update()
    {
        UpdateBowShock();
        UpdateHeatShader();
    }

    /// Pushes current entry intensity and ship screen position into global shader
    /// properties so HeatDistortionFeature can read them without any direct reference.
    void UpdateHeatShader()
    {
        float norm = Mathf.Clamp01(_intensity / maxIntensity);
        Shader.SetGlobalFloat(HeatIntensityId, norm);

        if (norm > 0.005f && _ship != null && Camera.main != null)
        {
            // Ship position in viewport space (0-1).
            Vector3 sp = Camera.main.WorldToViewportPoint(_ship.transform.position);

            // Velocity direction in viewport space — used to bias distortion ahead of ship.
            Vector2 vel    = _ship.Rb.linearVelocity;
            Vector2 velDir = vel.sqrMagnitude > 1f ? vel.normalized : Vector2.up;
            Vector3 sp2    = Camera.main.WorldToViewportPoint(
                                 _ship.transform.position + (Vector3)(velDir * 400f));
            Vector2 screenVel = new Vector2(sp2.x - sp.x, sp2.y - sp.y);
            if (screenVel.sqrMagnitude > 0.0001f) screenVel.Normalize();

            Shader.SetGlobalVector(HeatShipPosId,
                new Vector4(sp.x, sp.y, screenVel.x, screenVel.y));
        }
    }

    // ------------------------------------------------------------------ Emission

    void EmitPlasma(Vector2 velDir, Vector2 right, float speed)
    {
        float ang   = Mathf.Atan2(-velDir.y, -velDir.x)
                    + Random.Range(-plasmaSpreadDeg, plasmaSpreadDeg) * Mathf.Deg2Rad;
        float pSpd  = Random.Range(plasmaSpeedMin, plasmaSpeedMax) * speed;
        Vector2 pos = (Vector2)transform.position
                    + velDir * Random.Range(-8f,  5f)
                    + right  * Random.Range(-7f,  7f);

        var ep = new ParticleSystem.EmitParams();
        ep.position      = pos;
        ep.velocity      = new Vector3(Mathf.Cos(ang), Mathf.Sin(ang)) * pSpd;
        ep.startLifetime = Random.Range(plasmaLifetimeMin, plasmaLifetimeMax);
        ep.startSize     = Random.Range(plasmaSizeMin, plasmaSizeMax);
        ep.startColor    = new Color(1f, Random.Range(0.85f, 1f), Random.Range(0.7f, 1f), 1f);
        _plasma.Emit(ep, 1);
    }

    void EmitSmoke(Vector2 velDir, Vector2 right, float speed)
    {
        float ang   = Mathf.Atan2(-velDir.y, -velDir.x)
                    + Random.Range(-smokeSpreadDeg, smokeSpreadDeg) * Mathf.Deg2Rad;
        float pSpd  = Random.Range(smokeSpeedMin, smokeSpeedMax) * speed;
        Vector2 pos = (Vector2)transform.position
                    + velDir * Random.Range(-12f, 3f)
                    + right  * Random.Range(-9f,  9f);

        float g  = Random.Range(0.12f, 0.30f);
        var ep = new ParticleSystem.EmitParams();
        ep.position      = pos;
        ep.velocity      = new Vector3(Mathf.Cos(ang), Mathf.Sin(ang)) * pSpd;
        ep.startLifetime = Random.Range(smokeLifetimeMin, smokeLifetimeMax);
        ep.startSize     = Random.Range(smokeSizeMin, smokeSizeMax);
        ep.startColor    = new Color(g, g * 0.75f, g * 0.55f, 0.55f);
        _smoke.Emit(ep, 1);
    }

    // ------------------------------------------------------------------ Bow Shock

    void UpdateBowShock()
    {
        if (_intensity < plasmaThreshold || _ship == null)
        { _bowShock.enabled = false; return; }

        _bowShock.enabled = true;
        float norm   = Mathf.Clamp01(_intensity / maxIntensity);
        Vector2 vel  = _ship.Rb.linearVelocity;
        Vector2 fwd  = vel.sqrMagnitude > 0.01f ? vel.normalized : Vector2.up;
        Vector2 ctr  = (Vector2)transform.position
                     + fwd * Mathf.Lerp(bowShockForwardDist * 0.7f, bowShockForwardDist, norm);

        float arcR  = Mathf.Lerp(bowShockRadiusMin, bowShockRadiusMax, norm);
        _bowShock.startWidth = _bowShock.endWidth
            = Mathf.Lerp(bowShockWidthMin, bowShockWidthMax, norm);

        float orange = Mathf.Lerp(0.45f, 0.80f, norm);
        float alpha  = Mathf.Lerp(0.50f, 0.92f, norm);
        var grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(1f, orange * 0.55f, 0.05f), 0.0f),
                new GradientColorKey(new Color(1f, orange,         0.10f), 0.5f),
                new GradientColorKey(new Color(1f, orange * 0.55f, 0.05f), 1.0f),
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(alpha * 0.35f, 0.0f),
                new GradientAlphaKey(alpha,         0.5f),
                new GradientAlphaKey(alpha * 0.35f, 1.0f),
            });
        _bowShock.colorGradient = grad;

        const int Segs = 28;
        float baseAngle = Mathf.Atan2(fwd.y, fwd.x);
        float halfRad   = bowShockArcDeg * Mathf.Deg2Rad;
        for (int i = 0; i < Segs; i++)
        {
            float t = (float)i / (Segs - 1);
            float a = baseAngle + Mathf.Lerp(-halfRad, halfRad, t);
            _bowShock.SetPosition(i,
                new Vector3(ctr.x + Mathf.Cos(a) * arcR,
                            ctr.y + Mathf.Sin(a) * arcR, 0f));
        }
    }

    // ------------------------------------------------------------------ Builders

    ParticleSystem BuildPS(string goName, Shader shader, bool additive,
                           int maxParts, System.Action<ParticleSystem> applyModules)
    {
        var go = new GameObject(goName);
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;

        var ps  = go.AddComponent<ParticleSystem>();
        var rdr = go.GetComponent<ParticleSystemRenderer>();

        var mat = new Material(shader);
        EngineSmoke.SetTransparent(mat, additive);
        rdr.material     = mat;
        rdr.sortingOrder = additive ? 9 : 7;
        rdr.renderMode   = ParticleSystemRenderMode.Billboard;

        var main = ps.main;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startSpeed      = 0f;
        main.maxParticles    = maxParts;
        main.loop            = false;
        main.playOnAwake     = false;

        var em = ps.emission;
        em.enabled = false;

        applyModules(ps);
        ps.Play();
        return ps;
    }

    void BuildPlasmaModules(ParticleSystem ps)
    {
        var col = ps.colorOverLifetime;
        col.enabled = true;
        var g = new Gradient();
        g.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(1.0f, 0.95f, 0.80f), 0.00f),
                new GradientColorKey(new Color(1.0f, 0.55f, 0.10f), 0.45f),
                new GradientColorKey(new Color(0.8f, 0.10f, 0.00f), 1.00f),
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.00f),
                new GradientAlphaKey(0.7f, 0.50f),
                new GradientAlphaKey(0.0f, 1.00f),
            });
        col.color = new ParticleSystem.MinMaxGradient(g);

        var sz = ps.sizeOverLifetime;
        sz.enabled = true;
        sz.size = new ParticleSystem.MinMaxCurve(1f,
            new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 0.2f)));

        ApplyCollision(ps);
    }

    void BuildSmokeModules(ParticleSystem ps)
    {
        var col = ps.colorOverLifetime;
        col.enabled = true;
        var g = new Gradient();
        g.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(0.35f, 0.22f, 0.14f), 0.0f),
                new GradientColorKey(new Color(0.12f, 0.08f, 0.05f), 1.0f),
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0f,           0.00f),
                new GradientAlphaKey(smokeMaxAlpha, 0.20f),
                new GradientAlphaKey(smokeMaxAlpha * 0.7f, 0.60f),
                new GradientAlphaKey(0f,           1.00f),
            });
        col.color = new ParticleSystem.MinMaxGradient(g);

        var sz = ps.sizeOverLifetime;
        sz.enabled = true;
        sz.size = new ParticleSystem.MinMaxCurve(1f,
            new AnimationCurve(new Keyframe(0f, 0.4f), new Keyframe(1f, 3.0f)));

        ApplyCollision(ps);
    }

    void ApplyCollision(ParticleSystem ps)
    {
        var c = ps.collision;
        c.enabled      = enableCollision;
        c.type         = ParticleSystemCollisionType.World;
        c.mode         = ParticleSystemCollisionMode.Collision2D;
        c.bounce       = new ParticleSystem.MinMaxCurve(0f);
        c.dampen       = new ParticleSystem.MinMaxCurve(collisionDampen);
        c.lifetimeLoss = new ParticleSystem.MinMaxCurve(collisionLifeLoss);
        c.collidesWith = -1;   // all physics layers
    }

    LineRenderer BuildBowShock()
    {
        var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default")
                  ?? Shader.Find("Sprites/Default");
        var go = new GameObject("BowShockGO");
        go.transform.SetParent(transform);

        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.loop          = false;
        lr.positionCount = 28;
        lr.material      = new Material(shader);
        lr.sortingOrder  = 10;
        lr.enabled       = false;
        return lr;
    }

    // ------------------------------------------------------------------ Intensity

    float ComputeIntensity()
    {
        var planet = GameState.Instance?.GetNearestPlanet(_ship.Rb.position);
        if (planet == null) return 0f;

        Vector2 toShip = _ship.Rb.position - (Vector2)planet.transform.position;
        float   dist   = toShip.magnitude;
        if (dist > planet.Config.atmosphereRadius) return 0f;

        float surfR = planet.GetSurfaceRadiusAt(Mathf.Atan2(toShip.y, toShip.x));
        if (dist <= surfR) return 0f;

        // heightFrac: 0 = at surface, 1 = top of atmosphere
        float atmoHeight = planet.Config.atmosphereRadius - surfR;
        float heightFrac = (dist - surfR) / atmoHeight;

        // Fade to zero smoothly in the lowest surfaceFadeFraction of the atmosphere.
        // Above that band the factor is 1 (full effect).
        float heightFactor = surfaceFadeFraction > 0f
            ? Mathf.Clamp01(heightFrac / surfaceFadeFraction)
            : 1f;

        // depth: how deep inside the atmosphere (0 = top, 1 = surface)
        float depth   = 1f - heightFrac;
        float density = depth * depth * planet.Config.atmosphereDrag;

        return _ship.Rb.linearVelocity.magnitude * density * heightFactor;
    }
}

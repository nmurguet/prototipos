using UnityEngine;
using UnityEngine.InputSystem;

/// Predicts the ship's ballistic trajectory (gravity only, no thrust).
/// Toggle with Tab. Runs a forward simulation every frame when active.
public class TrajectoryPredictor : MonoBehaviour
{
    [Header("Simulation")]
    [Tooltip("Number of simulation steps to run forward.")]
    [SerializeField] int   steps = 400;

    [Tooltip("Time delta per simulation step (seconds). "
           + "steps * simDt = total lookahead time.")]
    [SerializeField] float simDt = 0.08f;   // 400 * 0.08 = 32 s lookahead

    [Tooltip("Draw every Nth simulated point. Higher = fewer line segments but faster.")]
    [SerializeField] int   drawEvery = 2;

    LineRenderer   _trail;
    LineRenderer   _impactDot;
    bool           _active;
    ShipController _ship;

    // ------------------------------------------------------------------ init

    void Awake()
    {
        _ship = GetComponent<ShipController>();

        var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default")
                  ?? Shader.Find("Sprites/Default");

        // ── trajectory trail ──
        var trailGO = new GameObject("TrajTrail");
        trailGO.transform.SetParent(transform);
        _trail                = trailGO.AddComponent<LineRenderer>();
        _trail.useWorldSpace  = true;
        _trail.positionCount  = 0;
        _trail.startWidth     = 3f;
        _trail.endWidth       = 0.5f;
        _trail.material       = new Material(shader);
        _trail.sortingOrder   = 10;
        _trail.enabled        = false;

        // Cyan → transparent gradient
        var grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[]  {
                new GradientColorKey(new Color(0.30f, 0.88f, 1.00f), 0f),
                new GradientColorKey(new Color(0.30f, 0.88f, 1.00f), 1f),
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0.90f, 0.00f),
                new GradientAlphaKey(0.00f, 1.00f),
            });
        _trail.colorGradient = grad;

        // ── impact marker (large dot = 2 identical positions with thick width) ──
        var dotGO = new GameObject("TrajImpact");
        dotGO.transform.SetParent(transform);
        _impactDot               = dotGO.AddComponent<LineRenderer>();
        _impactDot.useWorldSpace = true;
        _impactDot.positionCount = 2;
        _impactDot.startWidth    = _impactDot.endWidth = 12f;
        _impactDot.material      = new Material(shader);
        _impactDot.sortingOrder  = 11;
        _impactDot.enabled       = false;
    }

    // ------------------------------------------------------------------ toggle

    void Update()
    {
        if (Keyboard.current?.tabKey.wasPressedThisFrame == true)
            _active = !_active;

        bool show = _active && !_ship.IsDead && !_ship.IsLanded;

        if (!show)
        {
            _trail.enabled     = false;
            _impactDot.enabled = false;
            return;
        }

        _trail.enabled = true;
        RunSimulation();
    }

    // ------------------------------------------------------------------ simulation

    void RunSimulation()
    {
        var planets = GameState.Instance?.Planets;
        if (planets == null || planets.Count == 0)
        {
            _trail.enabled = false;
            return;
        }

        Vector2 pos = _ship.Rb.position;
        Vector2 vel = _ship.Rb.linearVelocity;

        // Pre-allocate max possible draw points
        int maxPts = Mathf.CeilToInt((float)steps / drawEvery) + 1;
        var pts    = new Vector3[maxPts];
        int count  = 0;

        bool    impact   = false;
        Vector3 impactPt = Vector3.zero;

        for (int i = 0; i < steps; i++)
        {
            // Record this point every Nth step
            if (i % drawEvery == 0 && count < maxPts)
                pts[count++] = new Vector3(pos.x, pos.y, 0f);

            // Accumulate gravity and drag from every planet
            foreach (var planet in planets)
            {
                Vector2 dir  = (Vector2)planet.transform.position - pos;
                float   dist = dir.magnitude;
                if (dist < 0.001f) continue;

                // Gravity (matches Planet.ApplyGravityTo exactly)
                if (dist <= 6f * planet.Config.radius)
                {
                    float gm    = planet.Config.surfaceGravity
                                * planet.Config.radius * planet.Config.radius;
                    float accel = Mathf.Min(gm / (dist * dist), 260f);
                    vel += dir.normalized * accel * simDt;
                }

                // Atmosphere drag
                if (dist < planet.Config.atmosphereRadius && dist > planet.Config.radius)
                {
                    float t    = Mathf.Clamp01(
                                    1f - (dist - planet.Config.radius)
                                       / (planet.Config.atmosphereRadius - planet.Config.radius));
                    float drag = planet.Config.atmosphereDrag * t * t * 0.35f;
                    vel       *= Mathf.Max(0f, 1f - drag * simDt);
                }

                // Surface impact check
                float angle = Mathf.Atan2(pos.y - planet.transform.position.y,
                                          pos.x - planet.transform.position.x);
                if (dist <= planet.GetSurfaceRadiusAt(angle))
                {
                    impactPt = new Vector3(pos.x, pos.y, 0f);
                    impact   = true;
                    break;
                }
            }

            if (impact) break;

            pos += vel * simDt;
        }

        // Upload to LineRenderer
        _trail.positionCount = count;
        for (int i = 0; i < count; i++) _trail.SetPosition(i, pts[i]);

        // Impact marker
        if (impact)
        {
            _impactDot.enabled = true;
            _impactDot.SetPosition(0, impactPt);
            _impactDot.SetPosition(1, impactPt);
            _impactDot.startColor = _impactDot.endColor = new Color(1f, 0.22f, 0.08f, 0.95f);
        }
        else
        {
            _impactDot.enabled = false;
        }
    }
}

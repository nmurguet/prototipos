using UnityEngine;

public class LandingPad : MonoBehaviour
{
    public Planet OwnerPlanet { get; private set; }
    public bool   IsUsed      { get; private set; }

    // ---- tunables ----
    const float PadWidth   = 140f;  // total platform width in world units
    const float LiftHeight =  10f;  // platform sits this many units above terrain
    const float LegDepth   =  28f;  // support legs extend this far into terrain
    const int   Segments   =   9;   // polyline points along the curved surface
    const float SurfWidth  =   5f;  // platform line thickness
    const float LegWidth   =   3f;
    const float LightSize  =   9f;

    int _terrainIndex;

    LineRenderer _surface;
    LineRenderer _legL, _legR;
    LineRenderer _lightL, _lightR;

    // Glow shader material shared by both lights on this pad.
    // null when the shader is unavailable (falls back to colour animation).
    Material _lightMat;

    static readonly int GlowColorId     = Shader.PropertyToID("_GlowColor");
    static readonly int GlowIntensityId  = Shader.PropertyToID("_GlowIntensity");

    // Proximity zone (world units) for the intensity boost — matches CameraController.
    const float GlowProximityRadius = 1100f;

    float _pulse;

    // ------------------------------------------------------------------ init

    public void Initialize(Planet planet, int terrainIndex)
    {
        OwnerPlanet   = planet;
        _terrainIndex = terrainIndex;

        // Parent to planet so the pad moves/rotates with it (future-proof)
        transform.SetParent(planet.transform);

        // Position GO at the centre surface point — the BoxCollider2D is built
        // in local space, so this determines where the trigger zone sits.
        float   centerAngle = terrainIndex * Mathf.PI * 2f / 128f;
        Vector2 surf        = planet.GetSurfacePoint(centerAngle);
        Vector2 norm        = (surf - (Vector2)planet.transform.position).normalized;

        transform.position = surf + norm * LiftHeight;
        float rot = Mathf.Atan2(norm.y, norm.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, rot);

        BuildVisual();
        BuildTrigger();
    }

    // ------------------------------------------------------------------ visual

    void BuildVisual()
    {
        var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default")
                  ?? Shader.Find("Sprites/Default");

        float centerAngle = _terrainIndex * Mathf.PI * 2f / 128f;
        float halfArc     = (PadWidth * 0.5f) / OwnerPlanet.Config.radius; // radians

        // -- curved surface platform --
        _surface = MakeLR("Surface", Segments, SurfWidth, 4, shader);
        for (int i = 0; i < Segments; i++)
        {
            float   t     = (float)i / (Segments - 1);
            float   angle = centerAngle + Mathf.Lerp(-halfArc, halfArc, t);
            Vector2 s     = OwnerPlanet.GetSurfacePoint(angle);
            Vector2 n     = (s - (Vector2)OwnerPlanet.transform.position).normalized;
            _surface.SetPosition(i, (Vector3)(s + n * LiftHeight));
        }

        // -- support legs at each end --
        Vector2 ptL, normL, ptR, normR;
        SampleEnd(-halfArc, centerAngle, out ptL, out normL);
        SampleEnd(+halfArc, centerAngle, out ptR, out normR);

        _legL = MakeLR("LegL", 2, LegWidth, 3, shader);
        _legL.SetPosition(0, (Vector3)(ptL + normL * LiftHeight));
        _legL.SetPosition(1, (Vector3)(ptL - normL * LegDepth));

        _legR = MakeLR("LegR", 2, LegWidth, 3, shader);
        _legR.SetPosition(0, (Vector3)(ptR + normR * LiftHeight));
        _legR.SetPosition(1, (Vector3)(ptR - normR * LegDepth));

        // -- indicator lights at ends --
        // Use the custom glow shader when available; fall back to Sprite-Unlit-Default.
        var glowShader  = Shader.Find("Custom/PadGlow");
        var lightShader = glowShader != null ? glowShader : shader;

        _lightL = MakeLR("LightL", 2, LightSize, 5, lightShader);
        _lightR = MakeLR("LightR", 2, LightSize, 5, lightShader);
        Vector3 posL = (Vector3)(ptL + normL * (LiftHeight + 4f));
        Vector3 posR = (Vector3)(ptR + normR * (LiftHeight + 4f));
        _lightL.SetPosition(0, posL); _lightL.SetPosition(1, posL);
        _lightR.SetPosition(0, posR); _lightR.SetPosition(1, posR);

        if (glowShader != null)
        {
            // One material instance per pad, shared by both lights — updated every frame.
            _lightMat = new Material(glowShader);
            _lightMat.SetColor(GlowColorId,    new Color(0.30f, 1.00f, 0.42f, 1f));
            _lightMat.SetFloat(GlowIntensityId, 1.5f);
            _lightL.sharedMaterial = _lightMat;
            _lightR.sharedMaterial = _lightMat;
        }

        SetColor(false);
    }

    void SampleEnd(float arcOffset, float center, out Vector2 pt, out Vector2 norm)
    {
        float   angle = center + arcOffset;
        pt   = OwnerPlanet.GetSurfacePoint(angle);
        norm = (pt - (Vector2)OwnerPlanet.transform.position).normalized;
    }

    LineRenderer MakeLR(string n, int pts, float w, int order, Shader shader)
    {
        var go = new GameObject(n);
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;   // positions are world-space, not affected by GO transform
        lr.positionCount = pts;
        lr.startWidth    = lr.endWidth = w;
        lr.material      = new Material(shader);
        lr.sortingOrder  = order;
        return lr;
    }

    void SetColor(bool used)
    {
        Color surfaceCol = used ? new Color(0.45f, 0.45f, 0.45f)
                                : new Color(0.20f, 0.88f, 0.32f);
        Color legCol     = new Color(0.50f, 0.52f, 0.55f);
        Color lightCol   = used ? new Color(0.35f, 0.35f, 0.35f)
                                : new Color(0.30f, 1.00f, 0.42f);

        if (_surface != null) { _surface.startColor = _surface.endColor = surfaceCol; }
        if (_legL    != null) { _legL.startColor = _legL.endColor = legCol;
                                _legR.startColor = _legR.endColor = legCol; }
        if (_lightL  != null) { _lightL.startColor = _lightL.endColor = lightCol;
                                 _lightR.startColor = _lightR.endColor = lightCol; }
    }

    // ------------------------------------------------------------------ trigger

    void BuildTrigger()
    {
        var col       = gameObject.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        // Local space: the GO is already oriented along the surface tangent,
        // so X = along platform, Y = outward from planet.
        col.size   = new Vector2(PadWidth, LiftHeight * 4f);
        col.offset = new Vector2(0f, 0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        other.GetComponent<ShipController>()?.OnLandingPadEnter(this);
    }

    // ------------------------------------------------------------------ animate

    void Update()
    {
        if (IsUsed || _lightL == null) return;

        _pulse += Time.deltaTime * 2.5f;
        float b = Mathf.Sin(_pulse) * 0.35f + 0.65f;   // 0.30 .. 1.0

        if (_lightMat != null)
        {
            // Proximity boost: intensity ramps up as the ship approaches this pad.
            float proximityBoost = 1f;
            var   ship           = LevelManager.Instance?.Ship;
            if (ship != null)
            {
                float dist = Vector2.Distance(
                    (Vector2)transform.position, ship.Rb.position);
                float t  = 1f - Mathf.Clamp01(dist / GlowProximityRadius);
                proximityBoost = 1f + t * t * 3.5f;   // 1× at edge → 4.5× at centre
            }

            _lightMat.SetColor(GlowColorId,
                new Color(0.30f * b, 1.00f * b, 0.42f * b, 1f));
            _lightMat.SetFloat(GlowIntensityId, 1.5f * b * proximityBoost);
        }
        else
        {
            // Fallback: plain colour animation (no glow shader).
            var c = new Color(0.30f * b, 1.00f * b, 0.42f * b);
            _lightL.startColor = _lightL.endColor = c;
            _lightR.startColor = _lightR.endColor = c;
        }
    }

    // ------------------------------------------------------------------ landing

    // Returns the world position where the ship centre should sit when landed,
    // given the ship's bottom offset in local space (NozzY magnitude = 13).
    /// World-space centre of the platform surface (used by CameraController for proximity zoom).
    public Vector2 GetWorldCenter() => (Vector2)transform.position;

    public Vector2 GetLandingPosition(float shipBottomLocal)
    {
        float   angle = _terrainIndex * Mathf.PI * 2f / 128f;
        Vector2 surf  = OwnerPlanet.GetSurfacePoint(angle);
        Vector2 norm  = (surf - (Vector2)OwnerPlanet.transform.position).normalized;
        // Platform surface is LiftHeight above terrain; ship centre sits
        // shipBottomLocal units further out so its bottom rests on the platform.
        return surf + norm * (LiftHeight + shipBottomLocal);
    }

    public Quaternion GetLandingRotation()
    {
        float   angle = _terrainIndex * Mathf.PI * 2f / 128f;
        Vector2 surf  = OwnerPlanet.GetSurfacePoint(angle);
        Vector2 norm  = (surf - (Vector2)OwnerPlanet.transform.position).normalized;
        float   rot   = Mathf.Atan2(norm.y, norm.x) * Mathf.Rad2Deg - 90f;
        return Quaternion.Euler(0f, 0f, rot);
    }

    public void OnLanded(ShipController ship, bool clean)
    {
        if (!IsUsed)
        {
            if (clean)
            {
                ship.AddFuel(65f);
                ship.AddHull(18f);
                GameState.Instance?.AddScore(150);
                HUDManager.Instance?.ShowLandingAlert("PERFECT LANDING  +150", true);
            }
            else
            {
                ship.AddFuel(35f);
                GameState.Instance?.AddScore(60);
                HUDManager.Instance?.ShowLandingAlert("HARD LANDING  +60", false);
            }

            GameState.Instance?.IncrementPadsLanded();
            IsUsed = true;
            SetColor(true);
        }
        else
        {
            // Revisit: small fuel bonus, no score, no alert.
            ship.AddFuel(10f);
            HUDManager.Instance?.ShowLandingAlert("ALREADY VISITED", false);
        }
    }
}

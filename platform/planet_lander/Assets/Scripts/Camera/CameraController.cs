using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    Camera         _cam;
    ShipController _ship;

    // ------------------------------------------------------------------ Inspector

    [Header("Altitude-based zoom")]
    [Tooltip("Orthographic size when close to surface")]
    [SerializeField] float orthoClose   = 800f;
    [Tooltip("Orthographic size when far from any surface")]
    [SerializeField] float orthoFar     = 18000f;
    [Tooltip("Altitude at which the camera starts zooming out")]
    [SerializeField] float zoomStartAlt = 2500f;
    [Tooltip("Altitude at which the camera reaches maximum zoom-out")]
    [SerializeField] float zoomFarAlt   = 25000f;

    [Header("Landing pad proximity zoom")]
    [Tooltip("Orthographic size used when centred on a landing pad")]
    [SerializeField] float orthoPadZoom       = 480f;
    [Tooltip("Distance from pad centre at which proximity zoom begins (world units)")]
    [SerializeField] float padProximityRadius = 1100f;
    [Tooltip("How sharply the proximity blend ramps up (1 = linear, 3 = cubic ease-in)")]
    [SerializeField] float padBlendPow        = 2f;

    [Header("Smoothing")]
    [Tooltip("Unused — position snap is instant (Rigidbody2D.Interpolate handles visual smoothness)")]
    [SerializeField] float smoothPos  = 6f;
    [Tooltip("How fast the camera zooms for altitude-based changes (higher = snappier)")]
    [SerializeField] float smoothZoom = 3f;
    [Tooltip("How fast the camera snaps into / out of pad proximity zoom")]
    [SerializeField] float smoothPadZoom = 4f;

    // ------------------------------------------------------------------ state

    float        _ortho;
    float        _padBlend;           // 0 = no pad nearby, 1 = fully inside pad zone
    LandingPad[] _pads;

    // ------------------------------------------------------------------ lifecycle

    void Awake()
    {
        Instance = this;
        _cam     = GetComponent<Camera>();
        _cam.orthographic     = true;
        _cam.orthographicSize = orthoFar;
        _cam.backgroundColor  = new Color(0.02f, 0.02f, 0.06f);
        _cam.clearFlags       = CameraClearFlags.SolidColor;
        _cam.nearClipPlane    = -10000f;
        _cam.farClipPlane     =  10000f;
        _ortho = orthoFar;
    }

    public void SetShip(ShipController ship)
    {
        _ship = ship;
        // Invalidate pad cache whenever the level changes / restarts.
        _pads = null;
    }

    // ------------------------------------------------------------------ LateUpdate

    void LateUpdate()
    {
        if (_ship == null || !_ship.gameObject.activeSelf) return;

        // Snap position directly to ship (Rigidbody interpolation handles visual smoothness).
        transform.position = new Vector3(
            _ship.transform.position.x,
            _ship.transform.position.y,
            -100f);

        // --- altitude target ---
        float alt     = GameState.Instance?.GetAltitudeAt(_ship.Rb.position) ?? float.MaxValue;
        float altSize = AltOrtho(alt);

        // --- pad proximity blend ---
        float targetBlend = ComputePadBlend();
        _padBlend = Mathf.Lerp(_padBlend, targetBlend, smoothPadZoom * Time.deltaTime);

        // Interpolate between altitude zoom and pad zoom.
        float tgtSize = Mathf.Lerp(altSize, orthoPadZoom, _padBlend);

        // Smooth the final size (altitude changes are gradual; pad transitions use smoothPadZoom).
        float lerpSpeed = Mathf.Lerp(smoothZoom, smoothPadZoom, _padBlend);
        _ortho = Mathf.Lerp(_ortho, tgtSize, lerpSpeed * Time.deltaTime);
        _cam.orthographicSize = _ortho;
    }

    // ------------------------------------------------------------------ helpers

    float AltOrtho(float alt)
    {
        if (alt <= zoomStartAlt) return orthoClose;
        if (alt >= zoomFarAlt)   return orthoFar;
        float t = (alt - zoomStartAlt) / (zoomFarAlt - zoomStartAlt);
        return Mathf.Lerp(orthoClose, orthoFar, Mathf.Sqrt(t));
    }

    /// Returns a 0-1 blend weight: 0 = no pad near, 1 = ship is right on top of a pad.
    float ComputePadBlend()
    {
        if (_ship == null) return 0f;

        // Lazy-initialise pad cache.
        if (_pads == null)
            _pads = FindObjectsByType<LandingPad>(FindObjectsSortMode.None);

        if (_pads.Length == 0) return 0f;

        Vector2 shipPos    = _ship.Rb.position;
        float   closestSqr = float.MaxValue;

        foreach (var pad in _pads)
        {
            if (pad == null) continue;
            float dsq = (pad.GetWorldCenter() - shipPos).sqrMagnitude;
            if (dsq < closestSqr) closestSqr = dsq;
        }

        float radiusSqr = padProximityRadius * padProximityRadius;
        if (closestSqr >= radiusSqr) return 0f;

        // t=0 at edge of zone, t=1 at centre.
        float t = 1f - Mathf.Sqrt(closestSqr) / padProximityRadius;
        return Mathf.Pow(Mathf.Clamp01(t), padBlendPow);
    }

    public float CurrentOrtho => _ortho;
}

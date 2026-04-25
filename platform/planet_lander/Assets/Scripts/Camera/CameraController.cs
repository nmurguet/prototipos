using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    Camera           _cam;
    ShipController   _ship;

    [Header("Zoom range")]
    [Tooltip("Orthographic size when close to surface (min zoom-in)")]
    [SerializeField] float orthoClose   = 800f;
    [Tooltip("Orthographic size when far from surface (max zoom-out)")]
    [SerializeField] float orthoFar     = 18000f;
    [Tooltip("Altitude at which the camera starts zooming out")]
    [SerializeField] float zoomStartAlt = 2500f;
    [Tooltip("Altitude at which the camera reaches maximum zoom-out")]
    [SerializeField] float zoomFarAlt   = 25000f;

    [Header("Smoothing")]
    [Tooltip("Unused — position snap is now instant to avoid jitter (Rigidbody2D.Interpolate handles visual smoothness)")]
    [SerializeField] float smoothPos    = 6f;
    [Tooltip("How fast the camera zooms in/out (higher = snappier)")]
    [SerializeField] float smoothZoom   = 3f;

    float _ortho;

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

    public void SetShip(ShipController ship) => _ship = ship;

    void LateUpdate()
    {
        if (_ship == null || !_ship.gameObject.activeSelf) return;

        // Snap directly to the ship's interpolated position — no extra lerp.
        // The Rigidbody2D already interpolates the ship's transform between physics
        // steps, so adding a second lerp here only introduces frame-to-frame lag
        // that manifests as camera jitter at high speed.
        transform.position = new Vector3(_ship.transform.position.x, _ship.transform.position.y, -100f);

        float alt     = GameState.Instance?.GetAltitudeAt(_ship.Rb.position) ?? float.MaxValue;
        float tgtSize = TargetOrtho(alt);
        _ortho        = Mathf.Lerp(_ortho, tgtSize, smoothZoom * Time.deltaTime);
        _cam.orthographicSize = _ortho;
    }

    float TargetOrtho(float alt)
    {
        if (alt <= zoomStartAlt) return orthoClose;
        if (alt >= zoomFarAlt)   return orthoFar;
        float t = (alt - zoomStartAlt) / (zoomFarAlt - zoomStartAlt);
        return Mathf.Lerp(orthoClose, orthoFar, Mathf.Sqrt(t));
    }

    public float CurrentOrtho => _ortho;
}

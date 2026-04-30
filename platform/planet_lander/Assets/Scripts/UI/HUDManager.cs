using UnityEngine;
using UnityEngine.InputSystem;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    ShipController _ship;

    string _alertText;
    float  _alertTimer;
    bool   _alertClean;
    const float AlertDuration = 2.2f;

    float _noFuelCountdown = -1f;
    float _noFuelPulse;

    bool _paused;

    const float MinimapR  = 90f;
    const float MinimapX  = 20f;
    const float MinimapY  = 20f;
    const float MapRange  = 58000f;

    Material _glMat;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _glMat = new Material(Shader.Find("Hidden/Internal-Colored")
                           ?? Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default"));
        _glMat.hideFlags = HideFlags.HideAndDontSave;
        _glMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        _glMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        _glMat.SetInt("_Cull",     (int)UnityEngine.Rendering.CullMode.Off);
        _glMat.SetInt("_ZWrite",   0);
    }

    public void SetShip(ShipController ship) => _ship = ship;

    public void ShowLandingAlert(string text, bool clean)
    {
        _alertText  = text;
        _alertTimer = AlertDuration;
        _alertClean = clean;
    }

    public void ShowNoFuelWarning(float countdown) => _noFuelCountdown = countdown;
    public void HideNoFuelWarning()                => _noFuelCountdown = -1f;

    void Update()
    {
        if (_alertTimer > 0f) _alertTimer -= Time.deltaTime;
        _noFuelPulse += Time.deltaTime * 4f;

        var kb = Keyboard.current;
        if (kb != null && kb.escapeKey.wasPressedThisFrame)
        {
            _paused = !_paused;
            Time.timeScale = _paused ? 0f : 1f;
        }
    }

    void OnGUI()
    {
        if (_ship == null) return;

        float sw = Screen.width;
        float sh = Screen.height;

        // Score + pads counter (top-centre)
        int score      = GameState.Instance?.Score      ?? 0;
        int padsLanded = GameState.Instance?.PadsLanded ?? 0;
        int totalPads  = GameState.Instance?.TotalPads  ?? 0;

        GUI.Label(Rect(sw * 0.5f - 160f, 8f, 320f, 32f),
            $"SCORE  {score}",
            Style(26, TextAnchor.MiddleCenter, Color.white));

        // Pads progress bar — small row below the score
        Color padFill = padsLanded >= totalPads && totalPads > 0
            ? new Color(0.3f, 1f, 0.4f)   // all done → bright green
            : new Color(0.25f, 0.75f, 1f);
        float padRatio = totalPads > 0 ? (float)padsLanded / totalPads : 0f;
        float barW = 180f;
        float barX = sw * 0.5f - barW * 0.5f;
        DrawBar(new Rect(barX, 44f, barW, 14f), padRatio, padFill, $"PADS  {padsLanded}/{totalPads}", 100f);

        // Fuel bar
        DrawBar(new Rect(20f, sh - 60f, 200f, 22f), _ship.Fuel / 100f, FuelColor(_ship.Fuel), "FUEL");

        // Hull bar
        DrawBar(new Rect(20f, sh - 90f, 200f, 22f), _ship.Hull / 100f, HullColor(_ship.Hull), "HULL");

        // Controls hint (bottom-right, small)
        var hint = Style(11, TextAnchor.LowerRight, new Color(0.5f, 0.5f, 0.5f));
        GUI.Label(Rect(sw - 210f, sh - 70f, 200f, 65f),
            "W  thrust\nQ/E  lateral\nA/D  rotate\nTAB  trajectory\nR  restart", hint);

        // Speed & Altitude
        var infoSt = Style(15, TextAnchor.UpperLeft, new Color(0.8f, 0.8f, 0.8f));
        float spd  = _ship.Rb != null ? _ship.Rb.linearVelocity.magnitude : 0f;
        float alt  = GameState.Instance?.GetAltitudeAt(_ship.Rb.position) ?? 0f;
        GUI.Label(Rect(20f, sh - 120f, 200f, 24f), $"SPD  {spd:F0}", infoSt);
        GUI.Label(Rect(20f, sh - 142f, 200f, 24f), $"ALT  {alt:F0}", infoSt);

        // Landing alert
        if (_alertTimer > 0f)
        {
            float t = _alertTimer / AlertDuration;
            float a = t < 0.25f ? t / 0.25f : 1f;
            var   c = _alertClean ? new Color(0.3f, 1f, 0.4f, a) : new Color(1f, 0.7f, 0.2f, a);
            GUI.Label(Rect(sw * 0.5f - 200f, sh * 0.4f, 400f, 40f), _alertText,
                Style(28, TextAnchor.MiddleCenter, c, bold: true));
        }

        // No-fuel warning
        if (_noFuelCountdown >= 0f)
        {
            float pulse = Mathf.Sin(_noFuelPulse) * 0.5f + 0.5f;
            GUI.Label(Rect(sw * 0.5f - 220f, sh * 0.35f, 440f, 36f),
                $"NO FUEL — DYING IN {_noFuelCountdown:F1}",
                Style(22, TextAnchor.MiddleCenter, new Color(1f, 0.25f + pulse * 0.5f, 0.1f), bold: true));
        }

        // Planet arrows
        DrawPlanetArrows(sw, sh);

        // Minimap (GL only during Repaint)
        if (Event.current.type == EventType.Repaint) DrawMinimap(sw, sh);

        // Pause
        if (_paused)
        {
            GUI.color = new Color(0, 0, 0, 0.6f);
            GUI.DrawTexture(new Rect(0, 0, sw, sh), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(Rect(sw * 0.5f - 120f, sh * 0.5f - 30f, 240f, 60f), "PAUSED",
                Style(40, TextAnchor.MiddleCenter, Color.white, bold: true));
        }
    }

    // ------------------------------------------------------------------ helpers

    static Rect Rect(float x, float y, float w, float h) => new Rect(x, y, w, h);

    static GUIStyle Style(int size, TextAnchor anchor, Color color, bool bold = false) =>
        new GUIStyle(GUI.skin.label) {
            fontSize  = size,
            alignment = anchor,
            fontStyle = bold ? FontStyle.Bold : FontStyle.Normal,
            normal    = { textColor = color },
        };

    void DrawBar(Rect r, float t, Color fill, string label, float labelWidth = 50f)
    {
        GUI.color = new Color(0.15f, 0.15f, 0.15f);
        GUI.DrawTexture(r, Texture2D.whiteTexture);
        GUI.color = fill;
        GUI.DrawTexture(new Rect(r.x, r.y, r.width * Mathf.Clamp01(t), r.height), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(r.x + r.width + 6f, r.y, labelWidth, r.height), label,
            Style(11, TextAnchor.MiddleLeft, Color.white));
    }

    Color FuelColor(float v)
    {
        if (v < 20f)
        {
            float b = Mathf.Sin(Time.unscaledTime * 10f) > 0f ? 1f : 0.3f;
            return new Color(1f, 0.1f, 0.1f, b);
        }
        return v < 50f ? new Color(1f, 0.75f, 0.05f) : new Color(0.2f, 0.85f, 0.3f);
    }

    Color HullColor(float v) =>
        v < 30f ? new Color(1f, 0.2f, 0.1f) :
        v < 60f ? new Color(1f, 0.6f, 0.1f) :
                  new Color(0.25f, 0.55f, 1f);

    void DrawPlanetArrows(float sw, float sh)
    {
        if (_ship == null || GameState.Instance == null) return;

        float cx = sw * 0.5f;
        float cy = sh * 0.5f;

        foreach (var p in GameState.Instance.Planets)
        {
            var sp = Camera.main.WorldToScreenPoint(p.transform.position);
            // WorldToScreenPoint Y starts from bottom; convert to GUI Y (from top)
            sp.y = sh - sp.y;
            Vector2 dir = new Vector2(sp.x - cx, sp.y - cy);
            if (dir.magnitude < 200f) continue;

            Vector2 norm = dir.normalized;
            float ax = cx + norm.x * (cx - 55f);
            float ay = cy + norm.y * (cy - 55f);
            GUI.Label(new Rect(ax - 40f, ay - 12f, 80f, 24f), $"▶ {p.Config.name}",
                Style(13, TextAnchor.MiddleCenter, new Color(0.8f, 0.8f, 0.8f)));
        }
    }

    void DrawMinimap(float sw, float sh)
    {
        if (_ship == null || GameState.Instance == null || _glMat == null) return;

        // Center of minimap in GL pixel coords.
        // GL.LoadPixelMatrix() inside OnGUI uses Y=0 at TOP (same as GUI), Y grows downward.
        // Therefore world +Y  →  GL  -Y  (subtract to go up on screen).
        float cx = sw - MinimapX - MinimapR;
        float cy = MinimapY + MinimapR;   // pixels from top

        _glMat.SetPass(0);
        GL.PushMatrix();
        GL.LoadPixelMatrix();

        GLFilledCircle(cx, cy, MinimapR, new Color(0f, 0f, 0f, 0.55f), 48);
        GLCircle(cx, cy, MinimapR, new Color(0.5f, 0.5f, 0.5f, 0.8f), 48);

        Vector2 shipPos = _ship.Rb.position;
        foreach (var p in GameState.Instance.Planets)
        {
            Vector2 rel  = (Vector2)p.transform.position - shipPos;
            Vector2 mini = rel / MapRange * MinimapR;
            if (mini.magnitude > MinimapR * 0.92f) mini = mini.normalized * MinimapR * 0.92f;
            float dotR = Mathf.Clamp(p.Config.radius / MapRange * MinimapR, 3f, 12f);
            // world +Y → screen -Y (Y=0 at top)
            GLFilledCircle(cx + mini.x, cy - mini.y, dotR, p.Config.color, 16);
        }

        GLShipIcon(cx, cy, _ship.transform.up, Color.white);
        GL.PopMatrix();
    }

    void GLCircle(float cx, float cy, float r, Color col, int segs)
    {
        GL.Begin(GL.LINES);
        GL.Color(col);
        for (int i = 0; i < segs; i++)
        {
            float a0 = i       * Mathf.PI * 2f / segs;
            float a1 = (i + 1) * Mathf.PI * 2f / segs;
            GL.Vertex3(cx + Mathf.Cos(a0)*r, cy + Mathf.Sin(a0)*r, 0);
            GL.Vertex3(cx + Mathf.Cos(a1)*r, cy + Mathf.Sin(a1)*r, 0);
        }
        GL.End();
    }

    void GLFilledCircle(float cx, float cy, float r, Color col, int segs)
    {
        GL.Begin(GL.TRIANGLES);
        GL.Color(col);
        for (int i = 0; i < segs; i++)
        {
            float a0 = i       * Mathf.PI * 2f / segs;
            float a1 = (i + 1) * Mathf.PI * 2f / segs;
            GL.Vertex3(cx, cy, 0);
            GL.Vertex3(cx + Mathf.Cos(a0)*r, cy + Mathf.Sin(a0)*r, 0);
            GL.Vertex3(cx + Mathf.Cos(a1)*r, cy + Mathf.Sin(a1)*r, 0);
        }
        GL.End();
    }

    void GLShipIcon(float cx, float cy, Vector2 facing, Color col)
    {
        // GL en OnGUI tiene Y=0 arriba → negar Y para que +world_Y = arriba en minimap
        Vector2 f  = new Vector2(facing.x, -facing.y).normalized * 5f * 2f;
        Vector2 rt = new Vector2(-f.y, f.x) * 0.5f;   // perpendicular
        GL.Begin(GL.TRIANGLES);
        GL.Color(col);
        GL.Vertex3(cx + f.x,         cy + f.y,        0);
        GL.Vertex3(cx + rt.x - f.x,  cy + rt.y - f.y, 0);
        GL.Vertex3(cx - rt.x - f.x,  cy - rt.y - f.y, 0);
        GL.End();
    }
}

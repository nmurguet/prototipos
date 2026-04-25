using UnityEngine;

public class LandingPad : MonoBehaviour
{
    public Planet OwnerPlanet { get; private set; }
    public bool   IsUsed      { get; private set; }

    const float HalfWidth = 80f;
    const float Thickness = 12f;

    LineRenderer _line;

    public void Initialize(Planet planet, int terrainIndex)
    {
        OwnerPlanet = planet;

        float   angle   = terrainIndex / 128f * Mathf.PI * 2f;
        Vector2 surfPt  = planet.GetSurfacePoint(angle);
        Vector2 normal  = (surfPt - (Vector2)planet.transform.position).normalized;

        // Place slightly above surface, rotate to match tangent
        transform.position = surfPt + normal * (Thickness * 0.5f);
        float rot = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0f, 0f, rot);

        BuildVisual();
        BuildTrigger();
    }

    void BuildVisual()
    {
        var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default")
                  ?? Shader.Find("Sprites/Default");
        _line                = gameObject.AddComponent<LineRenderer>();
        _line.useWorldSpace  = false;
        _line.positionCount  = 2;
        _line.SetPosition(0, new Vector3(-HalfWidth, 0, 0));
        _line.SetPosition(1, new Vector3( HalfWidth, 0, 0));
        _line.startWidth     = _line.endWidth = Thickness * 0.6f;
        _line.material       = new Material(shader);
        _line.sortingOrder   = 3;
        SetColor(false);
    }

    void SetColor(bool used)
    {
        if (_line == null) return;
        _line.startColor = _line.endColor = used
            ? new Color(0.45f, 0.45f, 0.45f)
            : new Color(0.20f, 0.85f, 0.30f);
    }

    void BuildTrigger()
    {
        var col      = gameObject.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size      = new Vector2(HalfWidth * 2f, Thickness * 2f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        other.GetComponent<ShipController>()?.OnLandingPadEnter(this);
    }

    // Called by ShipController after validating the landing
    public void OnLanded(ShipController ship, bool clean)
    {
        bool first = !IsUsed;

        if (clean && first)
        {
            ship.AddFuel(65f);
            ship.AddHull(18f);
            GameState.Instance?.AddScore(100);
            HUDManager.Instance?.ShowLandingAlert("CLEAN LANDING  +100", true);
        }
        else
        {
            int score = first ? 40 : 20;
            ship.AddFuel(first ? 40f : 20f);
            GameState.Instance?.AddScore(score);
            HUDManager.Instance?.ShowLandingAlert($"HARD LANDING  +{score}", false);
        }

        GameState.Instance?.IncrementPadsLanded();
        IsUsed = true;
        SetColor(true);
    }
}

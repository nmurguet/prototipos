using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType { Fuel, Hull }
    public PickupType Type { get; private set; }

    const float BobAmp   = 18f;
    const float BobSpeed = 1.4f;
    const float Size     = 22f;

    Vector2 _anchor;
    float   _phase;

    public void Initialize(PickupType type, Vector2 pos)
    {
        Type    = type;
        _anchor = pos;
        _phase  = Random.Range(0f, Mathf.PI * 2f);
        transform.position = pos;

        BuildVisual();
        var col       = gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius    = Size * 1.5f;
    }

    void BuildVisual()
    {
        var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default")
                  ?? Shader.Find("Sprites/Default");
        Color c = Type == PickupType.Fuel
            ? new Color(0.20f, 0.90f, 0.40f)
            : new Color(0.20f, 0.50f, 1.00f);

        // Diamond outline
        var lr = gameObject.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop          = true;
        lr.positionCount = 4;
        lr.SetPosition(0, new Vector3( 0,    Size, 0));
        lr.SetPosition(1, new Vector3( Size, 0,    0));
        lr.SetPosition(2, new Vector3( 0,   -Size, 0));
        lr.SetPosition(3, new Vector3(-Size, 0,    0));
        lr.startColor    = lr.endColor = c;
        lr.startWidth    = lr.endWidth = 3f;
        lr.material      = new Material(shader);
        lr.sortingOrder  = 4;

        // Small center ring
        var dot = new GameObject("dot");
        dot.transform.SetParent(transform);
        dot.transform.localPosition = Vector3.zero;
        var dlr = dot.AddComponent<LineRenderer>();
        dlr.useWorldSpace = false;
        dlr.loop          = true;
        dlr.positionCount = 8;
        for (int i = 0; i < 8; i++)
        {
            float a = i * Mathf.PI * 2f / 8f;
            dlr.SetPosition(i, new Vector3(Mathf.Cos(a) * 5f, Mathf.Sin(a) * 5f, 0f));
        }
        dlr.startColor   = dlr.endColor = c;
        dlr.startWidth   = dlr.endWidth = 3f;
        dlr.material     = new Material(shader);
        dlr.sortingOrder = 4;
    }

    void Update()
    {
        transform.position = _anchor + new Vector2(0f, Mathf.Sin(Time.time * BobSpeed + _phase) * BobAmp);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var ship = other.GetComponent<ShipController>();
        if (ship == null) return;
        if (Type == PickupType.Fuel) ship.AddFuel(35f);
        else                         ship.AddHull(25f);
        Destroy(gameObject);
    }
}

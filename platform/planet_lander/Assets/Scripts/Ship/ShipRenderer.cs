using UnityEngine;
using UnityEngine.InputSystem;

// Procedural ship: triangle hull + landing legs + engine flame
public class ShipRenderer : MonoBehaviour
{
    const float NoseY  =  14f;
    const float WingX  =  10f;
    const float WingY  = -10f;
    const float NozzX  =   4f;
    const float NozzY  = -13f;

    static readonly Vector3[] HullShape =
    {
        new Vector3(    0,  NoseY, 0),
        new Vector3( WingX, WingY, 0),
        new Vector3( NozzX, NozzY, 0),
        new Vector3(-NozzX, NozzY, 0),
        new Vector3(-WingX, WingY, 0),
        new Vector3(    0,  NoseY, 0), // close
    };

    LineRenderer _hull;
    LineRenderer _flame;
    LineRenderer[] _legs = new LineRenderer[2];

    static readonly Vector3[][] LegShapes =
    {
        new[] { new Vector3( NozzX, NozzY, 0), new Vector3( NozzX + 6f, NozzY - 5f, 0) },
        new[] { new Vector3(-NozzX, NozzY, 0), new Vector3(-NozzX - 6f, NozzY - 5f, 0) },
    };

    void Awake()
    {
        var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default")
                  ?? Shader.Find("Sprites/Default");

        // Collider was pre-added by LevelManager; just set the shape
        var col = gameObject.GetComponent<PolygonCollider2D>();
        col.SetPath(0, new Vector2[] {
            new Vector2(0, NoseY), new Vector2(WingX, WingY), new Vector2(-WingX, WingY)
        });

        _hull = MakeLR("Hull", HullShape.Length, new Color(0.90f, 0.90f, 0.90f), 1.6f, 2, shader);
        for (int i = 0; i < HullShape.Length; i++) _hull.SetPosition(i, HullShape[i]);

        for (int i = 0; i < 2; i++)
        {
            _legs[i] = MakeLR($"Leg{i}", 2, new Color(0.65f, 0.65f, 0.65f), 1.1f, 2, shader);
            _legs[i].SetPosition(0, LegShapes[i][0]);
            _legs[i].SetPosition(1, LegShapes[i][1]);
        }

        _flame = MakeLR("Flame", 3, new Color(1f, 0.7f, 0.2f), 2.5f, 3, shader);
        _flame.enabled = false;
    }

    LineRenderer MakeLR(string goName, int pts, Color col, float width, int order, Shader shader)
    {
        var go = new GameObject(goName);
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        var lr = go.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.positionCount = pts;
        lr.startColor    = lr.endColor  = col;
        lr.startWidth    = lr.endWidth  = width;
        lr.material      = new Material(shader);
        lr.sortingOrder  = order;
        return lr;
    }

    float _noise;

    void Update()
    {
        var ship   = GetComponent<ShipController>();
        bool thrust = ship != null && ship.Fuel > 0f
                   && Keyboard.current != null && Keyboard.current.wKey.isPressed;

        _flame.enabled = thrust;
        if (!thrust) return;

        _noise = Mathf.Lerp(_noise, Random.Range(0.7f, 1.3f), 0.4f);
        float len = -(NozzY + 8f) * _noise;   // positive length downward

        _flame.SetPosition(0, new Vector3(-NozzX * 0.7f, NozzY,        0));
        _flame.SetPosition(1, new Vector3(0f,             NozzY - len,  0));
        _flame.SetPosition(2, new Vector3( NozzX * 0.7f, NozzY,        0));
    }
}

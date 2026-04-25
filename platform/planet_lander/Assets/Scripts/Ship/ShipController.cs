using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class ShipController : MonoBehaviour
{
    // ---- public state ----
    public float Fuel     { get; private set; } = 100f;
    public float Hull     { get; private set; } = 100f;
    public bool  IsLanded { get; private set; }
    public bool  IsDead   { get; private set; }
    public Rigidbody2D Rb { get; private set; }

    // Config asset (se carga de Resources/ShipConfig)
    // También editable en el Inspector para tweaking en caliente
    [Header("Config Asset")]
    [SerializeField] ShipConfig config;

    // Shortcuts para no escribir config. en todo el código
    float angularDamping => config.angularDamping;
    float thrustForce    => config.thrustForce;
    float rotSpeed       => config.rotSpeed;
    float maxSpeed       => config.maxSpeed;
    float thrustMainFuel => config.thrustMainFuel;
    float thrustLatFuel  => config.thrustLatFuel;
    float gracePeriod    => config.gracePeriod;
    float mapBound       => config.mapBound;
    float bounceFactor   => config.bounceFactor;
    float softThreshold  => config.softThreshold;
    float hardThreshold  => config.hardThreshold;
    float crashThreshold => config.crashThreshold;

    EngineSmoke _smoke;
    float       _graceTimer;
    bool        _noFuelWarning;
    LandingPad  _currentPad;

    // Input leído en Update, consumido en FixedUpdate
    bool _inputThrust, _inputLeft, _inputRight, _inputRotL, _inputRotR;

    void Awake()
    {
        if (config == null)
            config = Resources.Load<ShipConfig>("ShipConfig");
        if (config == null)
            config = ScriptableObject.CreateInstance<ShipConfig>(); // valores por defecto

        Rb = GetComponent<Rigidbody2D>();
        Rb.gravityScale   = 0f;
        Rb.linearDamping  = 0f;
        Rb.angularDamping = angularDamping;
        Rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        // Interpolate transform between physics steps so the ship never stutters
        // visually at high speed regardless of framerate vs physics rate mismatch.
        Rb.interpolation  = RigidbodyInterpolation2D.Interpolate;

        var pm = new PhysicsMaterial2D("ShipMat") { bounciness = 0f, friction = 0f };
        GetComponent<Collider2D>().sharedMaterial = pm;

        _smoke = GetComponent<EngineSmoke>();
    }

    // ------------------------------------------------------------------ fixed update

    void FixedUpdate()
    {
        if (IsDead || IsLanded) return;
        ApplyGravity();
        ApplyThrust();
        ClampSpeed();
        CheckBounds();
    }

    void ApplyGravity()
    {
        GameState.Instance?.GetNearestPlanet(Rb.position)?.ApplyGravityTo(Rb);
    }

    void ApplyThrust()
    {
        if (_inputRotL) Rb.angularVelocity =  rotSpeed;
        if (_inputRotR) Rb.angularVelocity = -rotSpeed;

        if (_inputThrust && Fuel > 0f)
        {
            Rb.AddForce((Vector2)transform.up * thrustForce);
            ConsumeFuel(thrustMainFuel * Time.fixedDeltaTime);
            _smoke?.Emit(true);
        }
        else _smoke?.Emit(false);

        if (_inputLeft && Fuel > 0f)
        {
            Rb.AddForce(-(Vector2)transform.right * thrustForce * 0.6f);
            ConsumeFuel(thrustLatFuel * Time.fixedDeltaTime);
        }
        if (_inputRight && Fuel > 0f)
        {
            Rb.AddForce((Vector2)transform.right * thrustForce * 0.6f);
            ConsumeFuel(thrustLatFuel * Time.fixedDeltaTime);
        }

        if (IsLanded && (_inputThrust || _inputLeft || _inputRight)) TakeOff();
    }

    void ConsumeFuel(float amount)
    {
        Fuel = Mathf.Max(0f, Fuel - amount);
        if (Fuel <= 0f && !_noFuelWarning) { _noFuelWarning = true; _graceTimer = gracePeriod; }
    }

    // ------------------------------------------------------------------ update

    void Update()
    {
        if (IsDead) return;

        // Leer input a framerate completo
        var kb = Keyboard.current;
        if (kb != null)
        {
            _inputThrust = kb.wKey.isPressed;
            _inputLeft   = kb.qKey.isPressed;
            _inputRight  = kb.eKey.isPressed;
            _inputRotL   = kb.aKey.isPressed;
            _inputRotR   = kb.dKey.isPressed;
        }

        if (_noFuelWarning && Fuel <= 0f)
        {
            _graceTimer -= Time.deltaTime;
            HUDManager.Instance?.ShowNoFuelWarning(_graceTimer);
            if (_graceTimer <= 0f) TriggerDeath();
        }
        else if (Fuel > 0f && _noFuelWarning)
        {
            _noFuelWarning = false;
            HUDManager.Instance?.HideNoFuelWarning();
        }
    }

    void ClampSpeed()
    {
        if (Rb.linearVelocity.magnitude > maxSpeed)
            Rb.linearVelocity = Rb.linearVelocity.normalized * maxSpeed;
    }

    void CheckBounds()
    {
        if (Rb.position.magnitude > mapBound) TriggerDeath();
    }

    // ------------------------------------------------------------------ collision

    void OnCollisionEnter2D(Collision2D col)
    {
        if (IsDead) return;
        if (col.gameObject.GetComponent<Planet>() != null)
            HandlePlanetCollision(col);
    }

    void HandlePlanetCollision(Collision2D col)
    {
        float linear  = col.relativeVelocity.magnitude;
        float angular = Mathf.Abs(Rb.angularVelocity * Mathf.Deg2Rad) * 0.55f;
        float impact  = linear + angular;

        if (impact < softThreshold) return;

        if (impact < hardThreshold)
        {
            DamageHull((impact - softThreshold) / (hardThreshold - softThreshold) * 20f);
            Bounce(col);
        }
        else if (impact < crashThreshold)
        {
            DamageHull(30f + (impact - hardThreshold) / (crashThreshold - hardThreshold) * 40f);
            Bounce(col);
            LevelManager.Instance?.SpawnExplosion(transform.position, false);
        }
        else TriggerDeath();
    }

    void Bounce(Collision2D col)
    {
        if (col.contactCount == 0) return;
        Rb.linearVelocity = Vector2.Reflect(Rb.linearVelocity, col.GetContact(0).normal) * bounceFactor;
    }

    // ------------------------------------------------------------------ landing pad trigger

    public void OnLandingPadEnter(LandingPad pad)
    {
        if (IsDead || IsLanded) return;

        var planet = GameState.Instance?.GetNearestPlanet(Rb.position);
        if (planet == null) return;

        // Gear must face toward planet center
        Vector2 toPlanet = ((Vector2)planet.transform.position - Rb.position).normalized;
        bool    gearOk   = Vector2.Dot(-(Vector2)transform.up, toPlanet) > 0.5f;

        float linear  = Rb.linearVelocity.magnitude;
        float angular = Mathf.Abs(Rb.angularVelocity * Mathf.Deg2Rad) * 0.55f;
        float impact  = linear + angular;

        if (!gearOk)
        {
            if (impact >= softThreshold) DamageHull(Mathf.Lerp(5f, 40f, impact / crashThreshold));
            return;
        }

        if (impact < softThreshold)
        {
            DoLanding(pad, clean: true);
        }
        else if (impact < hardThreshold)
        {
            DamageHull((impact - softThreshold) / (hardThreshold - softThreshold) * 15f);
            DoLanding(pad, clean: false);
        }
        else if (impact < crashThreshold)
        {
            DamageHull(25f + (impact - hardThreshold) / (crashThreshold - hardThreshold) * 35f);
            BounceFromPlanet();
            LevelManager.Instance?.SpawnExplosion(transform.position, false);
        }
        else TriggerDeath();
    }

    void DoLanding(LandingPad pad, bool clean)
    {
        IsLanded             = true;
        _currentPad          = pad;
        Rb.linearVelocity    = Vector2.zero;
        Rb.angularVelocity   = 0f;
        Rb.isKinematic       = true;
        pad.OnLanded(this, clean);
    }

    void TakeOff()
    {
        IsLanded       = false;
        _currentPad    = null;
        Rb.isKinematic = false;
    }

    void BounceFromPlanet()
    {
        var planet = GameState.Instance?.GetNearestPlanet(Rb.position);
        if (planet == null) return;
        Vector2 away = (Rb.position - (Vector2)planet.transform.position).normalized;
        Rb.linearVelocity = away * Rb.linearVelocity.magnitude * bounceFactor;
    }

    // ------------------------------------------------------------------ damage / death

    public void AddFuel(float v) => Fuel = Mathf.Min(100f, Fuel + v);
    public void AddHull(float v) => Hull = Mathf.Min(100f, Hull + v);

    void DamageHull(float amount)
    {
        Hull = Mathf.Max(0f, Hull - amount);
        if (Hull <= 0f) TriggerDeath();
    }

    public void TriggerDeath()
    {
        if (IsDead) return;
        IsDead = true;
        GameState.Instance?.SetAlive(false);
        LevelManager.Instance?.SpawnExplosion(transform.position, true);
        LevelManager.Instance?.ScheduleRestart(1.6f);
        gameObject.SetActive(false);
    }

    // ------------------------------------------------------------------ reset

    public void ResetShip(Vector2 spawnPos)
    {
        IsDead             = false;
        IsLanded           = false;
        Fuel               = 100f;
        Hull               = 100f;
        _noFuelWarning     = false;
        _graceTimer        = 0f;
        _currentPad        = null;
        Rb.isKinematic     = false;
        Rb.linearVelocity  = Vector2.zero;
        Rb.angularVelocity = 0f;
        transform.SetPositionAndRotation(spawnPos, Quaternion.identity);
        gameObject.SetActive(true);
    }
}

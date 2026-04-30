using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public ShipController Ship { get; private set; }

    readonly List<Planet>     _planets = new List<Planet>();
    readonly List<LandingPad> _pads    = new List<LandingPad>();
    readonly List<Pickup>     _pickups = new List<Pickup>();

    GameObject _shipGO;
    GameObject _starsGO;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        // Si la escena ya tiene planetas (generados por SceneBuilder), solo conectar.
        // Si no, construir todo desde cero (modo Bootstrap / escena vacía).
        var existingPlanets = FindObjectsByType<Planet>(FindObjectsSortMode.None);
        if (existingPlanets.Length > 0)
            WireExistingScene();
        else
            BuildScene();
    }

    // ------------------------------------------------------------------ wire existing (pre-built scene)

    void WireExistingScene()
    {
        EnsureGameState();

        foreach (var p in FindObjectsByType<Planet>(FindObjectsSortMode.None))
        {
            _planets.Add(p);
            GameState.Instance?.RegisterPlanet(p);      // ← faltaba esto
            foreach (var pad in p.GetComponentsInChildren<LandingPad>())
                _pads.Add(pad);
        }

        foreach (var pk in FindObjectsByType<Pickup>(FindObjectsSortMode.None))
            _pickups.Add(pk);

        // Ship
        var sc = FindFirstObjectByType<ShipController>();
        if (sc != null)
        {
            Ship    = sc;
            _shipGO = sc.gameObject;
            Ship.ResetShip(Ship.transform.position);
        }

        _starsGO = FindFirstObjectByType<ParallaxStars>()?.gameObject;

        GameState.Instance?.SetTotalPads(_pads.Count);

        CameraController.Instance?.SetShip(Ship);
        HUDManager.Instance?.SetShip(Ship);
    }

    // ------------------------------------------------------------------ build (empty scene)

    void BuildScene()
    {
        EnsureGameState();
        BuildCamera();
        BuildStars();
        BuildPlanets();
        BuildPickups();
        BuildShip();
        BuildHUD();
    }

    void EnsureGameState()
    {
        if (GameState.Instance == null)
            new GameObject("GameState").AddComponent<GameState>();
        GameState.Instance.Reset();
    }

    void BuildCamera()
    {
        var go  = new GameObject("MainCamera");
        go.tag  = "MainCamera";
        go.AddComponent<Camera>();
        go.AddComponent<CameraController>();
        go.transform.position = new Vector3(0f, 0f, -100f);
    }

    void BuildStars()
    {
        _starsGO = new GameObject("Stars");
        _starsGO.AddComponent<ParallaxStars>();
    }

    void BuildPlanets()
    {
        var db = Resources.Load<PlanetDatabase>("PlanetDatabase");
        var configs = db != null ? db.planets : PlanetConfig.AllPlanets;
        foreach (var cfg in configs)
        {
            var go     = new GameObject(cfg.name);
            var planet = go.AddComponent<Planet>();
            planet.Initialize(cfg);
            _planets.Add(planet);

            int[] padIndices = planet.FindFlatZones(cfg.padCount);
            planet.FlattenForPads(padIndices);   // level terrain before placing pads

            foreach (int idx in padIndices)
            {
                var padGO = new GameObject($"Pad_{cfg.name}_{idx}");
                padGO.transform.SetParent(go.transform);
                var pad = padGO.AddComponent<LandingPad>();
                pad.Initialize(planet, idx);
                _pads.Add(pad);
            }
        }

        GameState.Instance?.SetTotalPads(_pads.Count);
    }

    static readonly Vector2[] PickupPositions =
    {
        new Vector2( 17500f, -13750f),
        new Vector2(-20000f,   6250f),
        new Vector2(  8750f,  18750f),
        new Vector2(-13750f, -17500f),
        new Vector2( 26250f,  -2500f),
        new Vector2(  2500f, -26000f),
        new Vector2( 33750f,  27500f),
        new Vector2(-28750f,  33000f),
    };

    void BuildPickups()
    {
        for (int i = 0; i < PickupPositions.Length; i++)
        {
            var go   = new GameObject($"Pickup_{i}");
            var pick = go.AddComponent<Pickup>();
            pick.Initialize(i % 2 == 0 ? Pickup.PickupType.Fuel : Pickup.PickupType.Hull,
                            PickupPositions[i]);
            _pickups.Add(pick);
        }
    }

    void BuildShip()
    {
        _shipGO = new GameObject("Ship");
        _shipGO.AddComponent<PolygonCollider2D>(); // must exist before ShipController.Awake
        _shipGO.AddComponent<ShipRenderer>();
        _shipGO.AddComponent<EngineSmoke>();
        _shipGO.AddComponent<AtmosphericEntry>();
        _shipGO.AddComponent<TrajectoryPredictor>();
        Ship = _shipGO.AddComponent<ShipController>();

        // Spawn above Terra
        Ship.ResetShip(new Vector2(0f, 4800f + 300f));

        CameraController.Instance?.SetShip(Ship);
    }

    void BuildHUD()
    {
        var go  = new GameObject("HUD");
        var hud = go.AddComponent<HUDManager>();
        hud.SetShip(Ship);
    }

    // ------------------------------------------------------------------ effects

    public void SpawnExplosion(Vector3 pos, bool fatal)
    {
        var go  = new GameObject("Explosion");
        go.transform.position = pos;
        go.AddComponent<Explosion>().Initialize(pos, fatal);
    }

    // ------------------------------------------------------------------ restart

    public void ScheduleRestart(float delay) => StartCoroutine(RestartAfter(delay));

    IEnumerator RestartAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        DoRestart();
    }

    void DoRestart()
    {
        GameState.Instance.Reset();

        // Si la escena fue pre-construida, solo reseteamos la nave y los pickups
        bool preBuilt = GameObject.Find("GeneratedLevel") != null;
        if (preBuilt)
        {
            // Reactivar pickups destruidos (no podemos, así que solo reseteamos la nave)
            Ship.ResetShip(new Vector2(0f, 4800f + 300f));
            foreach (var pad in _pads) if (pad) pad.gameObject.SetActive(true);
            CameraController.Instance?.SetShip(Ship);
            HUDManager.Instance?.SetShip(Ship);
            return;
        }

        // Escena vacía: destruir y reconstruir todo
        foreach (var p in _planets) if (p) Destroy(p.gameObject);
        foreach (var p in _pads)    if (p) Destroy(p.gameObject);
        foreach (var p in _pickups) if (p) Destroy(p.gameObject);
        if (_shipGO)  Destroy(_shipGO);
        if (_starsGO) Destroy(_starsGO);

        _planets.Clear(); _pads.Clear(); _pickups.Clear();

        BuildStars();
        BuildPlanets();
        BuildPickups();
        BuildShip();

        HUDManager.Instance?.SetShip(Ship);
        CameraController.Instance?.SetShip(Ship);
    }
}

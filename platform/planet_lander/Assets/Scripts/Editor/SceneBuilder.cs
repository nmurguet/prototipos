using UnityEngine;
using UnityEditor;

// Menú: Game → Build Level  (o Ctrl+Shift+B)
// Crea todos los GameObjects en edit mode para que puedas inspeccionarlos
// y ajustar valores antes de darle Play.
public static class SceneBuilder
{
    const string ROOT         = "GeneratedLevel";
    const string CONFIG_DIR   = "Assets/Resources";
    const string SHIP_ASSET   = "Assets/Resources/ShipConfig.asset";
    const string PLANET_ASSET = "Assets/Resources/PlanetDatabase.asset";

    // ------------------------------------------------------------------ assets

    static ShipConfig EnsureShipConfig()
    {
        var asset = AssetDatabase.LoadAssetAtPath<ShipConfig>(SHIP_ASSET);
        if (asset != null) return asset;

        asset = ScriptableObject.CreateInstance<ShipConfig>();
        AssetDatabase.CreateAsset(asset, SHIP_ASSET);
        AssetDatabase.SaveAssets();
        Debug.Log("[SceneBuilder] Creado ShipConfig.asset en Resources/");
        return asset;
    }

    static PlanetDatabase EnsurePlanetDatabase()
    {
        var asset = AssetDatabase.LoadAssetAtPath<PlanetDatabase>(PLANET_ASSET);
        if (asset != null) return asset;

        asset = ScriptableObject.CreateInstance<PlanetDatabase>();
        asset.planets = PlanetConfig.AllPlanets;
        AssetDatabase.CreateAsset(asset, PLANET_ASSET);
        AssetDatabase.SaveAssets();
        Debug.Log("[SceneBuilder] Creado PlanetDatabase.asset en Resources/");
        return asset;
    }

    [MenuItem("Game/Build Level %#b")]
    static void BuildLevel()
    {
        var shipCfg    = EnsureShipConfig();
        var planetDb   = EnsurePlanetDatabase();

        // Destruir nivel anterior si existe
        var old = GameObject.Find(ROOT);
        if (old != null) Undo.DestroyObjectImmediate(old);

        var root = new GameObject(ROOT);
        Undo.RegisterCreatedObjectUndo(root, "Build Level");

        // ---- GameState ----
        var gs = new GameObject("GameState");
        gs.AddComponent<GameState>();
        gs.transform.SetParent(root.transform);

        // ---- LevelManager ----
        var lm = new GameObject("LevelManager");
        lm.AddComponent<LevelManager>();
        Undo.RegisterCreatedObjectUndo(lm, "Build Level");

        // ---- Camera ----
        var cam = new GameObject("MainCamera");
        cam.tag = "MainCamera";
        cam.AddComponent<Camera>();
        cam.AddComponent<CameraController>();
        cam.transform.position = new Vector3(0f, 0f, -100f);
        cam.transform.SetParent(root.transform);

        // ---- Stars ----
        var stars = new GameObject("Stars");
        stars.AddComponent<ParallaxStars>();
        stars.transform.SetParent(root.transform);

        // ---- Planetas + pads (usa PlanetDatabase) ----
        foreach (var cfg in planetDb.planets)
        {
            var go     = new GameObject(cfg.name);
            go.transform.SetParent(root.transform);
            var planet = go.AddComponent<Planet>();
            planet.Initialize(cfg);

            int[] padIndices = planet.FindFlatZones(cfg.padCount);
            planet.FlattenForPads(padIndices);   // level terrain, mesh and collider rebuilt

            foreach (int idx in padIndices)
            {
                var padGO = new GameObject($"Pad_{cfg.name}_{idx}");
                padGO.transform.SetParent(go.transform);
                padGO.AddComponent<LandingPad>().Initialize(planet, idx);
            }
        }

        // ---- Pickups ----
        var pickupsRoot = new GameObject("Pickups");
        pickupsRoot.transform.SetParent(root.transform);
        var positions = new Vector2[]
        {
            new( 17500f, -13750f), new(-20000f,   6250f),
            new(  8750f,  18750f), new(-13750f, -17500f),
            new( 26250f,  -2500f), new(  2500f, -26000f),
            new( 33750f,  27500f), new(-28750f,  33000f),
        };
        for (int i = 0; i < positions.Length; i++)
        {
            var go   = new GameObject($"Pickup_{i}");
            go.transform.SetParent(pickupsRoot.transform);
            go.AddComponent<Pickup>().Initialize(
                i % 2 == 0 ? Pickup.PickupType.Fuel : Pickup.PickupType.Hull,
                positions[i]);
        }

        // ---- Ship ----
        var ship = new GameObject("Ship");
        ship.transform.SetParent(root.transform);
        ship.AddComponent<PolygonCollider2D>();
        ship.AddComponent<ShipRenderer>();
        ship.AddComponent<EngineSmoke>();
        ship.AddComponent<AtmosphericEntry>();
        ship.AddComponent<TrajectoryPredictor>();
        var sc = ship.AddComponent<ShipController>();
        ship.transform.position = new Vector3(0f, 4800f + 300f, 0f);

        // ---- HUD ----
        var hud = new GameObject("HUD");
        hud.AddComponent<HUDManager>();
        hud.transform.SetParent(root.transform);

        EditorUtility.SetDirty(root);
        Selection.activeGameObject = root;

        Debug.Log($"[SceneBuilder] Nivel generado: {PlanetConfig.AllPlanets.Length} planetas.");
    }

    [MenuItem("Game/Clear Level")]
    static void ClearLevel()
    {
        var old = GameObject.Find(ROOT);
        if (old != null) Undo.DestroyObjectImmediate(old);

        var lm = GameObject.Find("LevelManager");
        if (lm != null) Undo.DestroyObjectImmediate(lm);
    }
}

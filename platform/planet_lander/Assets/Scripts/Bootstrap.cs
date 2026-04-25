using UnityEngine;

// Auto-creates the LevelManager on any scene load — no manual setup needed.
// Just press Play on an empty 2D scene.
public static class Bootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        if (Object.FindFirstObjectByType<LevelManager>() != null) return;
        var go = new GameObject("LevelManager");
        go.AddComponent<LevelManager>();
        Object.DontDestroyOnLoad(go);
    }
}

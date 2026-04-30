using UnityEngine;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    public int  Score      { get; private set; }
    public int  PadsLanded { get; private set; }
    public int  TotalPads  { get; private set; }
    public bool IsAlive    { get; private set; } = true;

    readonly List<Planet> _planets = new List<Planet>();
    public IReadOnlyList<Planet> Planets => _planets;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterPlanet(Planet p)   { if (!_planets.Contains(p)) _planets.Add(p); }
    public void UnregisterPlanet(Planet p) { _planets.Remove(p); }

    public Planet GetNearestPlanet(Vector2 pos)
    {
        Planet nearest  = null;
        float  minDist  = float.MaxValue;
        foreach (var p in _planets)
        {
            float d = Vector2.Distance(pos, (Vector2)p.transform.position);
            if (d < minDist) { minDist = d; nearest = p; }
        }
        return nearest;
    }

    public float GetAltitudeAt(Vector2 pos)
    {
        var p = GetNearestPlanet(pos);
        if (p == null) return float.MaxValue;
        Vector2 dir = pos - (Vector2)p.transform.position;
        return dir.magnitude - p.GetSurfaceRadiusAt(Mathf.Atan2(dir.y, dir.x));
    }

    public void AddScore(int pts)          => Score += pts;
    public void IncrementPadsLanded()      => PadsLanded++;
    public void SetAlive(bool alive)       => IsAlive = alive;
    public void SetTotalPads(int n)        => TotalPads = n;

    public void Reset()
    {
        Score      = 0;
        PadsLanded = 0;
        IsAlive    = true;
        // TotalPads is NOT reset — it's set once per level build, not per life.
    }
}

using UnityEngine;

[System.Serializable]
public struct HarmonicEntry
{
    public float frequency;
    public float amplitude;
}

[System.Serializable]
public class PlanetConfig
{
    public string  name;
    public Vector2 position;
    public float   radius;
    public float   atmosphereRadius;
    public float   surfaceGravity;
    public float   atmosphereDrag;
    public float   roughness;
    public int     terrainStyle;
    public Color   color;
    public int     padCount;

    public static readonly HarmonicEntry[][] Styles =
    {
        // Style 0: smooth
        new[] {
            new HarmonicEntry { frequency=2,  amplitude=0.55f },
            new HarmonicEntry { frequency=3,  amplitude=0.28f },
            new HarmonicEntry { frequency=5,  amplitude=0.12f },
            new HarmonicEntry { frequency=8,  amplitude=0.05f },
        },
        // Style 1: rolling
        new[] {
            new HarmonicEntry { frequency=3,  amplitude=0.45f },
            new HarmonicEntry { frequency=5,  amplitude=0.28f },
            new HarmonicEntry { frequency=9,  amplitude=0.16f },
            new HarmonicEntry { frequency=15, amplitude=0.07f },
            new HarmonicEntry { frequency=25, amplitude=0.04f },
        },
        // Style 2: jagged
        new[] {
            new HarmonicEntry { frequency=2,  amplitude=0.30f },
            new HarmonicEntry { frequency=4,  amplitude=0.25f },
            new HarmonicEntry { frequency=7,  amplitude=0.20f },
            new HarmonicEntry { frequency=12, amplitude=0.14f },
            new HarmonicEntry { frequency=20, amplitude=0.07f },
            new HarmonicEntry { frequency=35, amplitude=0.04f },
        },
    };

    public static PlanetConfig[] AllPlanets => new[]
    {
        new PlanetConfig { name="Terra",   position=new Vector2(0,0),           radius=4800, atmosphereRadius=9600,  surfaceGravity=215, atmosphereDrag=1.00f, roughness=0.10f, terrainStyle=1, color=new Color(0.22f,0.62f,0.30f), padCount=6 },
        new PlanetConfig { name="Vulcan",  position=new Vector2(35000,-27500),  radius=2800, atmosphereRadius=5000,  surfaceGravity=310, atmosphereDrag=0.40f, roughness=0.24f, terrainStyle=2, color=new Color(0.82f,0.22f,0.12f), padCount=3 },
        new PlanetConfig { name="Glacius", position=new Vector2(-40000,12500),  radius=6200, atmosphereRadius=12800, surfaceGravity=145, atmosphereDrag=1.50f, roughness=0.06f, terrainStyle=0, color=new Color(0.52f,0.78f,0.95f), padCount=5 },
        new PlanetConfig { name="Dust",    position=new Vector2(17500,37500),   radius=3600, atmosphereRadius=7000,  surfaceGravity=190, atmosphereDrag=0.70f, roughness=0.16f, terrainStyle=1, color=new Color(0.80f,0.55f,0.22f), padCount=3 },
        new PlanetConfig { name="Shadow",  position=new Vector2(-27500,-35000), radius=2400, atmosphereRadius=4200,  surfaceGravity=350, atmosphereDrag=0.28f, roughness=0.20f, terrainStyle=2, color=new Color(0.28f,0.22f,0.40f), padCount=3 },
        new PlanetConfig { name="Iris",    position=new Vector2(5000,-52000),   radius=3200, atmosphereRadius=6500,  surfaceGravity=110, atmosphereDrag=1.80f, roughness=0.05f, terrainStyle=0, color=new Color(0.55f,0.30f,0.88f), padCount=4 },
        new PlanetConfig { name="Ferro",   position=new Vector2(50000,18000),   radius=1600, atmosphereRadius=2600,  surfaceGravity=480, atmosphereDrag=0.15f, roughness=0.30f, terrainStyle=2, color=new Color(0.62f,0.65f,0.70f), padCount=2 },
        new PlanetConfig { name="Verdant", position=new Vector2(-18000,54000),  radius=5200, atmosphereRadius=10500, surfaceGravity=175, atmosphereDrag=1.20f, roughness=0.11f, terrainStyle=1, color=new Color(0.30f,0.75f,0.42f), padCount=4 },
    };
}

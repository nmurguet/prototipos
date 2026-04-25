using UnityEngine;

[CreateAssetMenu(fileName = "PlanetDatabase", menuName = "PlanetLander/Planet Database")]
public class PlanetDatabase : ScriptableObject
{
    public PlanetConfig[] planets;

    // Valores por defecto listos para editar
    void Reset()
    {
        planets = PlanetConfig.AllPlanets;
    }
}

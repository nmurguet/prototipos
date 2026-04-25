using UnityEngine;

[CreateAssetMenu(fileName = "ShipConfig", menuName = "PlanetLander/Ship Config")]
public class ShipConfig : ScriptableObject
{
    [Header("Physics")]
    public float angularDamping = 1.0f;

    [Header("Thrust")]
    public float thrustForce    = 340f;
    public float rotSpeed       = 325f;
    public float maxSpeed       = 4500f;
    public float thrustMainFuel = 8f;
    public float thrustLatFuel  = 4f;

    [Header("Survival")]
    public float gracePeriod    = 5f;
    public float mapBound       = 80000f;

    [Header("Collisions")]
    public float bounceFactor   = 0.30f;
    public float softThreshold  = 72f;
    public float hardThreshold  = 140f;
    public float crashThreshold = 210f;
}

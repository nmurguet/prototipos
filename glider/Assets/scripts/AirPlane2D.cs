using UnityEngine;
using System.Collections;

public class AirPlane2D : MonoBehaviour {
    public Transform frontWings;
    public float verticalRange = 10f;
    public bool invert = false;
    private Vector3 frontWingStartRot = Vector3.zero;

    void Start()
    {
        frontWingStartRot = frontWings.localEulerAngles;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (Input.GetKeyDown(KeyCode.L))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        
        Vector3 vertical = new Vector3(Input.GetAxis("Vertical") * verticalRange, 0, 0);
        if (invert)
        {
            vertical = -vertical;
        }
        frontWings.localEulerAngles = frontWingStartRot - vertical;
    }
}

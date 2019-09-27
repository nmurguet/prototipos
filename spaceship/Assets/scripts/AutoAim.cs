using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAim : MonoBehaviour
{

    public Transform target;

    public float smooth;
    private float zVelocity = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target) { 
        Vector3 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float z1 = Mathf.SmoothDampAngle(transform.eulerAngles.z, angle + 90f, ref zVelocity, smooth);
        transform.rotation = Quaternion.Euler(0f, 0f, z1);
        }
    }
}

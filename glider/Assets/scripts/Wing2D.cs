using UnityEngine;
using System.Collections;

public class Wing2D : MonoBehaviour {

    public Rigidbody2D plane;
    public float lift = 1f;
    public bool debug = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 velocity = plane.GetPointVelocity(transform.position);
        float speed = velocity.magnitude;
        Vector2 direction = velocity.normalized;
        Vector2 worldNrm = transform.up;
        if (Vector2.Dot(worldNrm, direction) < 0)
            worldNrm = -worldNrm;

        float angle = 1f - Mathf.Clamp((Vector2.Angle(worldNrm, direction) / 90f), 0, 1f);
        Vector2 drag = -worldNrm * angle * speed * speed * lift;

        if (debug)
        {
            Debug.DrawRay(transform.position, drag, Color.green);
            Debug.Log(angle);
        }
        plane.AddForceAtPosition(drag * Time.fixedDeltaTime, transform.position);
    }
}

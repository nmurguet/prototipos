using UnityEngine;
using System.Collections;

public class Thruster : MonoBehaviour
{
	public Rigidbody plane;
	public float maxSpeed = 20f;
	public float acceleration = 5f;
	public float speed = 0f;

	void OnGUI()
	{
		GUI.VerticalSlider (new Rect(0,0,24,256),speed,maxSpeed,0f);
	}

	void Update()
	{
		float changeInVelocity = Input.GetAxis ("Acceleration") * acceleration * Time.deltaTime;

		speed = Mathf.Clamp (speed + changeInVelocity,0f,maxSpeed);
	}

	void FixedUpdate ()
	{
		Vector3 targetVelocity = transform.forward * speed;
		plane.AddForceAtPosition (targetVelocity,transform.position);
	}
}

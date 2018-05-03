using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class plane : MonoBehaviour {

	public float torque; 

	public float surfaceArea = 4.24f; 
	public float airDensity = 0.0583f; 
	public float airSpeed = 0.0f; 

	public float maxThrust = 100.0f; 

	public float speed; 
	public float velocity;
	public float acceleration = 5f;

	public float lift = 1f; 


	public bool automatic; 


	private Rigidbody2D rb; 

	public Transform engine; 

	// Use this for initialization
	void Start () {

		rb = GetComponent<Rigidbody2D> ();
		automatic = false; 
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey (KeyCode.R)) {

			SceneManager.LoadScene (SceneManager.GetActiveScene ().name);

		}
		if (Input.GetKeyDown (KeyCode.Q)) {

			if (automatic == false) {
				automatic = true; 
			} else if (automatic == true) {
				automatic = false;
			}

		}

		if (Input.GetKey (KeyCode.Z)) {
			
				speed += 0.05f;
			

		}
		if (Input.GetKey (KeyCode.X)) {
			
				speed -= 0.05f;

		}

		if (speed > 3f) {
			speed = 3f;
		}
		if (speed < 0f) {
			speed = 0f;
		}


		velocity = rb.velocity.x;

		if (rb.velocity.x > 10f) {
			rb.velocity = new Vector2 (10f, rb.velocity.y);
		}
		if (rb.velocity.x < -10f) {
			rb.velocity = new Vector2 (-10f, rb.velocity.y);
		}

		if (automatic == true) {
			if (rb.velocity.x < -0.5f) {
				transform.localScale = new Vector3 (transform.localScale.x, -1, transform.localScale.z);

			} else {
				transform.localScale = new Vector3 (transform.localScale.x, 1, transform.localScale.z);

			}
		} else if (automatic == false) {

			if (Input.GetKeyDown (KeyCode.E)) {
				transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y * -1, transform.localScale.z);

			}

		}
		
	}

	void FixedUpdate()
	{

		if (Input.GetKey (KeyCode.W)) {
			if (transform.localScale.y == 1) {
				rb.AddTorque (-1f * torque);
			} else if (transform.localScale.y == -1) {

				rb.AddTorque (1f*torque);
			}

		}

		if (Input.GetKey (KeyCode.S)) {
			if (transform.localScale.y == 1) {
				rb.AddTorque (1f * torque);
			} else if (transform.localScale.y == -1) {
				rb.AddTorque (-1f * torque);

			}

		}


		/*
		lift = rb.velocity.x * speed *0.05f; 
		if (lift > 10f) {
			lift = 10f; 
		}
		rb.AddRelativeForce (Vector2.up * lift); */

		Vector3 targetVelocity = transform.forward * speed;
		//rb.AddForceAtPosition (targetVelocity, engine.position);
		rb.AddRelativeForce(Vector2.right * speed);



		Vector2 velocity = rb.GetPointVelocity(transform.position);
		float speedy = velocity.magnitude;
		Vector2 direction = velocity.normalized;
		Vector2 worldNrm = transform.up;
		if (Vector2.Dot(worldNrm, direction) < 0)
			worldNrm = -worldNrm;

		float angle = 1f - Mathf.Clamp((Vector2.Angle(worldNrm, direction) / 90f), 0, 1f);
		Vector2 drag = -worldNrm * angle * speedy * speedy * lift;

	
		rb.AddForceAtPosition(drag * Time.fixedDeltaTime, transform.position);



	}
}

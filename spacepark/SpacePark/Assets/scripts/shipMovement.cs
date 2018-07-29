using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shipMovement : MonoBehaviour {

	private Rigidbody2D rb;

	public float turnForce;
	public float thrustForce;
	public float strafeForce;


	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();


		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate()
	{
		Movement ();

	}


	void Movement()
	{



		if (Input.GetKey (KeyCode.A)) {
			rb.AddTorque (0.3f * turnForce);


		}
		if (Input.GetKey (KeyCode.D)) {
			rb.AddTorque (-0.3f * turnForce);
		}


		if (Input.GetKey (KeyCode.S)) {
			
			rb.AddRelativeForce (Vector2.up * thrustForce);


		}
		if (Input.GetKey (KeyCode.W)) {
			
			rb.AddRelativeForce (Vector2.up * -thrustForce);


		}

		if (Input.GetKey (KeyCode.E)) {
			
			rb.AddRelativeForce (Vector2.right * -strafeForce);


		}
		if (Input.GetKey (KeyCode.Q)) {
			
			rb.AddRelativeForce (Vector2.right * strafeForce);


		}



	}




}

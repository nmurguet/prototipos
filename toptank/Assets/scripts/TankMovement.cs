using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankMovement : MonoBehaviour {
	private Rigidbody2D rb; 
	public float moveSpeed; 
	public float torque; 


	//Turret
	public GameObject turret; 
	public float smooth; 
	private float zVelocity = 0f;

	public GameObject bullet; 
	public GameObject shootPoint; 

	public GameObject flash; 


	//bullet delay
	public float timeBetweenShots = 0.5f;

	private float timeStamp;



	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();


	}
	
	// Update is called once per frame
	void Update () {
		Movement (); 
		MoveTurret (); 
		ShootTurret (); 
	}



	void Movement()
	{

		if (Input.GetAxisRaw ("Vertical") > 0.5f || Input.GetAxisRaw ("Vertical") < -0.5f) {



			rb.AddRelativeForce(Vector2.up * moveSpeed * Input.GetAxisRaw("Vertical"));
		}


		if (Input.GetAxisRaw ("Horizontal") > 0.5f || Input.GetAxisRaw ("Horizontal") < -0.5f) {

			rb.AddTorque (Input.GetAxisRaw ("Horizontal") * -torque);


		}

	}



	void MoveTurret()
	{
		//Smooth turret FIX THIS
		/*
		transform.rotation = gameObject.transform.rotation; 

		Vector3 diff_left = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		diff_left.Normalize ();

		float rot_zl = Mathf.Atan2 (diff_left.y, diff_left.x) * Mathf.Rad2Deg;
		float zl = Mathf.SmoothDampAngle(turret.transform.eulerAngles.z,rot_zl + 92f, ref zVelocity, smooth);
		turret.transform.rotation = Quaternion.Euler (0f, 0f, zl);
		*/

		//Fast turret- boring turret - FIXED

		Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint (transform.position);
		float angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg; 
		float zl = Mathf.SmoothDampAngle(turret.transform.eulerAngles.z,angle+90f, ref zVelocity, smooth);
		//this is for instant turret without zl
		//turret.transform.rotation = Quaternion.AngleAxis (angle + 90f, Vector3.forward);
		turret.transform.rotation = Quaternion.Euler (0f, 0f, zl);
	}


	void ShootTurret()
	{
		
		if (Input.GetMouseButtonDown (0)&& Time.time >=timeStamp) {

			Instantiate (bullet, shootPoint.transform.position, shootPoint.transform.rotation);
			flash.SetActive (true); 
			StartCoroutine(waitandDisable(0.2f));
			timeStamp = Time.time + timeBetweenShots;

		}


	}


	IEnumerator waitandDisable(float seconds)
	{
		yield return new WaitForSeconds (seconds);
		flash.SetActive(false); 
	}
}

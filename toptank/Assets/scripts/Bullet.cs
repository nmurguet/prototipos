using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	private Rigidbody2D rb; 



	public float speed; 

	public GameObject explosionEffect;

	//destroy overtime
	public float timer;
	private float timerCounter;

	// Use this for initialization
	void Start () {

		rb = GetComponent<Rigidbody2D> (); 

		timerCounter = timer; 
		
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 pos = transform.position; 

		Vector3 velocity = new Vector3 (0, -speed * Time.deltaTime, 0);
		pos += transform.rotation * velocity;
		transform.position = pos; 


		timerCounter -= Time.deltaTime; 

		if (timerCounter < 0f) {
			Destroy (gameObject);

		}
		
	}



	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "obstacle") {
			Instantiate (explosionEffect, transform.position, Quaternion.identity);
			Vector2 dir = other.transform.position - transform.position;
			other.gameObject.GetComponent<Rigidbody2D> ().AddForce(dir*300f);
			other.gameObject.GetComponent<Rigidbody2D> ().AddTorque (Random.Range (-10f, 10f));
			Destroy (gameObject);




		}

	}
}

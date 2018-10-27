using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBounce : MonoBehaviour {


	public float speed = 20f; 

	public int damage = 50;

	public int initialUp = 1;

	public int bounces = 3;

	public Rigidbody2D rb; 

	public Rigidbody2D player;

	public GameObject impactEffect; 
	// Use this for initialization
	void Start () {
		Vector2 shoot_up = new Vector2 (0f, 10f);
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Rigidbody2D>();
		speed = speed+Mathf.Abs( player.velocity.x);
		rb.velocity = (transform.right* speed) + transform.up * initialUp;
	}

	void OnTriggerEnter2D(Collider2D hitInfo)
	{
		Debug.Log (hitInfo.name);
		EnemyWalker enemy = hitInfo.GetComponent<EnemyWalker> ();
		if (enemy != null) {
			Debug.Log ("Hago daño");
			enemy.TakeDamage (damage);
			Vector2 m_NewForce = new Vector2(rb.velocity.x/4f, 1.5f);
			enemy.GetComponent<Rigidbody2D> ().AddForce (m_NewForce,ForceMode2D.Impulse);
			Instantiate (impactEffect, transform.position, Quaternion.identity);
			Destroy (gameObject);

		}
		if (enemy == null) {
			Debug.Log ("rebotar");
			bounces = bounces - 1;




		}


	}

	// Update is called once per frame
	void FixedUpdate () {

		if (bounces <= 0) {
			Instantiate (impactEffect, transform.position, Quaternion.identity);
			Destroy (gameObject);

		}




	}
}



	

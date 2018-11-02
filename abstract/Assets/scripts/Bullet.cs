using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public float speed = 20f; 

	public int damage = 50;

	public Rigidbody2D rb; 


	public GameObject impactEffect; 
	// Use this for initialization
	void Start () {

		rb.velocity = transform.right * speed;
		
	}


	void OnTriggerEnter2D(Collider2D hitInfo)
	{
		Debug.Log (hitInfo.name);
		EnemyWalker enemy = hitInfo.GetComponent<EnemyWalker> ();
		if (enemy != null) {
			enemy.TakeDamage (damage);
			Vector2 m_NewForce = new Vector2(rb.velocity.x/4f, 1.2f);
			enemy.GetComponent<Rigidbody2D> ().AddForce (m_NewForce,ForceMode2D.Impulse);

		}

		Instantiate (impactEffect, transform.position, Quaternion.identity);
		Destroy (gameObject);
	}

}

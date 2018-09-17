using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCharacter : MonoBehaviour {

	public Transform target; 

	public Rigidbody2D rb; 

	public bool move; 

	public Vector2 velocity; 

	public float speed; 
	// Use this for initialization
	void Start () {
		

	}
	
	// Update is called once per frame
	void Update () {

		if (move) {
			transform.position = Vector2.MoveTowards (transform.position, target.position, speed * Time.deltaTime);

		}

		
	}
}

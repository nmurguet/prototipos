using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class line : MonoBehaviour {

	public LineRenderer pad;
	public GameObject left; 
	public GameObject right;

	public EdgeCollider2D edge; 

	public List<Vector2> newVerticies = new List<Vector2> (); 
	// Use this for initialization
	void Start () {

		pad = GetComponent<LineRenderer> ();
		edge = GetComponent<EdgeCollider2D> (); 



		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		pad.SetPosition (0, left.gameObject.transform.position);
		pad.SetPosition (1, right.gameObject.transform.position);
		newVerticies.Clear (); 
		Vector2 padleft = new Vector2 (left.transform.position.x, left.transform.position.y);
		Vector2 padright = new Vector2 (right.transform.position.x, right.transform.position.y);

		newVerticies.Add (padleft);
		newVerticies.Add (padright);
		edge.points = newVerticies.ToArray (); 



		
	}

	void Update()
	{
		

		if (Input.GetKey (KeyCode.Q)) {
			right.transform.position = new Vector3(right.transform.position.x,right.transform.position.y + 1f * Time.deltaTime, right.transform.position.z);

		}
		if (Input.GetKey (KeyCode.A)) {
			right.transform.position = new Vector3(right.transform.position.x,right.transform.position.y - 1f * Time.deltaTime, right.transform.position.z);

		}

		if (Input.GetKey (KeyCode.O)) {
			left.transform.position = new Vector3(left.transform.position.x,left.transform.position.y + 1f * Time.deltaTime, left.transform.position.z);

		}
		if (Input.GetKey (KeyCode.L)) {
			left.transform.position = new Vector3(left.transform.position.x,left.transform.position.y - 1f * Time.deltaTime, left.transform.position.z);

		}


	}
}

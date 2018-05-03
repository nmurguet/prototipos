using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Transform target; 

	public float speed;


	public bool following;
	public float offset_x;
	public float offset_y;


	public Transform walker;


	// Use this for initialization
	void Start () {

		following = true; 
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	//	CheckBoundaries ();
		if (following == true) {
			if (target) {
				transform.position = Vector3.Lerp (transform.position, target.position, speed) + new Vector3 (offset_x, offset_y, -10);

			}

		}
	}

	public void StartFollowing(Transform tar)
	{

		target = tar; 
		following = true; 


	}

	public void StoptFollowing()
	{
		target = null;
		following = false; 


	}

	void CheckBoundaries()
	{
		if (transform.position.y < 1.5f) {
			transform.position = new Vector3(transform.position.x,1.5f,transform.position.z);

		}
		if (transform.position.y > 123f) {
			transform.position = new Vector3(transform.position.x,123f,transform.position.z);

		}

	}


	public void SetTarget(Transform tar)
	{
		target = tar; 

	}



}

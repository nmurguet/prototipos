using UnityEngine;
using System.Collections;

public class FallowCamera : MonoBehaviour {

    public Transform target;
    public float distance;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = target.position - new Vector3(0,0,distance);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathOvertime : MonoBehaviour {
	public float timer;
	private float timerCounter; 
	// Use this for initialization
	void Start () {

		timerCounter = timer; 
	}
	
	// Update is called once per frame
	void Update () {




		timerCounter -= Time.deltaTime; 

		if (timerCounter < 0f) {
			Destroy (gameObject);

		}
		
	}
}

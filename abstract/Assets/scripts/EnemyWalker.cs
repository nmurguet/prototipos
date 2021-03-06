﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalker : MonoBehaviour {

	public int health = 100; 
	public GameObject deathEffect; 
	public GameObject deathBody;

	// Use this for initialization
	public void TakeDamage(int damage)
	{
		health -= damage; 
		if (health<= 0)
		{
			Die();
		}

	}

	void Die()
	{
		Instantiate(deathEffect, transform.position, Quaternion.identity);
        Quaternion spawnRotation = Quaternion.Euler(0, 0, 90);
        Instantiate(deathBody, transform.position, spawnRotation);
		Destroy(gameObject);

	}

	// Update is called once per frame
	void Update () {
		
	}
}

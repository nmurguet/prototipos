﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalker : MonoBehaviour {

	public int health = 100; 
	public GameObject deathEffect; 

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
		Destroy(gameObject);

	}

	// Update is called once per frame
	void Update () {
		
	}
}
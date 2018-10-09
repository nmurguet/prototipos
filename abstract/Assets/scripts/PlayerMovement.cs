﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public CharacterController2D controller; 


	public Animator animator;

	public RuntimeAnimatorController Player_red;
	public RuntimeAnimatorController Player_green;

	public float runSpeed = 40f; 
	float horizontalMove = 0f;

	bool jump = false; 
	bool crouch = false; 

	
	// Update is called once per frame
	void Update () {

		horizontalMove = Input.GetAxisRaw ("Horizontal") * runSpeed;

		animator.SetFloat ("Speed",Mathf.Abs(horizontalMove));

		if(Input.GetKeyDown(KeyCode.Q)){
			Debug.Log ("pasa algo?");
			animator.runtimeAnimatorController = Player_red;
		}
		if(Input.GetKeyDown(KeyCode.E)){
			animator.runtimeAnimatorController = Player_green;
		}
			

		if (Input.GetButtonDown ("Jump")) {
			
			jump = true;
			animator.SetBool ("isJumping", jump);



		}
		if (Input.GetButtonDown ("Crouch")) {
			crouch = true; 
		} else if (Input.GetButtonUp ("Crouch")) {
			crouch = false; 
		}


	}


	public void OnLanding()
	{
		animator.SetBool ("isJumping", false);


	}

	public void OnCrouching(bool isCrouching)
	{
		animator.SetBool ("isCrouching", isCrouching);
	}




	void FixedUpdate()
	{
		//move the character
		controller.Move(horizontalMove * Time.fixedDeltaTime,crouch,jump);
		jump = false; 


	}
}

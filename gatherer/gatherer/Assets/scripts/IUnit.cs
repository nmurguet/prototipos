using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit {

	bool isIdle(); 
	void MoveTo(Vector3 position, float stopDistance, Action onArriveAtPosition);
	void PlayAnimationMine(Vector3 lookAtPosition, Action onAnimationCompleted); 
	
}

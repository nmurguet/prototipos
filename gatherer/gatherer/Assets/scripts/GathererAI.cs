using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GathererAI : MonoBehaviour {

	[SerializeField] private Transform goldNodeTransform; 
	[SerializeField] private Transform storageTransform; 


	private IUnit unit; 

	private void Awake()
	{

		unit = gameObject.GetComponent<IUnit> (); 

		unit.MoveTo (goldNodeTransform.position, 10f, () => {
			unit.PlayAnimationMine (goldNodeTransform.position, () => {
				unit.MoveTo (storageTransform.position, 5f, null);
			});
		
		});

	}

			}



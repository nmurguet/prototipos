using UnityEngine;
using System.Collections;

public class AirPlane : MonoBehaviour {
	public Transform frontWings;
	public Transform leftRudder;
	public Transform rightRudder;
	public GameObject rudder;
	public float rutterRange = 10f;
	public float verticalRange = 10f;
	public float rotationSpeed = 90f;
	public bool invert = false;
	private Vector3 frontWingStartRot = Vector3.zero;
	private Vector3 leftRutterStartRot = Vector3.zero;
	private Vector3 rightRutterStartRot = Vector3.zero;

	void Start () {
		leftRutterStartRot = leftRudder.localEulerAngles;
		frontWingStartRot = frontWings.localEulerAngles;
		rightRutterStartRot = rightRudder.localEulerAngles;
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape))
			Application.Quit ();
		if(Input.GetKeyDown(KeyCode.R))
		{
			rudder.SetActive(!rudder.activeSelf);
		}
		if (Input.GetKeyDown (KeyCode.L))
		{
			Application.LoadLevel(Application.loadedLevel);
		}

		Vector3 horizontal = new Vector3(Input.GetAxis ("Horizontal") * rutterRange,0,0);
		Vector3 vertical = new Vector3 (Input.GetAxis ("Vertical") * verticalRange,0,0);
		if (invert) {
			horizontal = -horizontal;
			vertical = -vertical;
		}
		leftRudder.localEulerAngles = leftRutterStartRot - horizontal;
		rightRudder.localEulerAngles = rightRutterStartRot + horizontal;
		frontWings.localEulerAngles = frontWingStartRot - vertical;
	}
}

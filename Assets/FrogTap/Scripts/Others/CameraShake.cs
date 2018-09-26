using UnityEngine;
using System.Collections;

// Based on: https://gist.github.com/ftvs/5822103
public class CameraShake : MonoBehaviour {
	
	//Public variables
	public float shakeDuration;										//How long the object should shake for.
	public float shakeAmount = 0.1f;								//Amplitude of the shake. A larger value shakes the camera harder.

	//Private variables
	private Transform myTransform;									//Reference to this object's transform component.
	private Vector3 startPosition;									//Store start position of this object.
	private float cameraSpeed = 0.9f;								//Speed this object will get back to its position.


	// Use this for initialization
	void Start () {
		//Setting up references.
		myTransform = transform;
		startPosition = myTransform.localPosition;
		enabled = false;										//...disable this script to avoid use the update method.
	}

	// Update is called once per frame.
	void Update() {
		//If the time is paused, then do nothing.
		if (Time.timeScale == 0)
			return;
		
		//If the shake timer is higher than 0...
		if (shakeDuration > 0) {
			//...set the camera's local position to a random value inside a unit sphere times the shake amount and...
			//...make sure the Z value stays the same.
			Vector3 position = transform.localPosition + Random.insideUnitSphere * shakeAmount;
			myTransform.localPosition = new Vector3 (position.x, position.y, startPosition.z);

			shakeDuration -= Time.deltaTime;				//...decrease the timer by Time.deltaTime.
		} 
		else {
			//Otherwise, when the timer has finished...
			shakeDuration = 0f;								//...reset shake duration to 0...
			//...move camera to its start position...
			float step = cameraSpeed * Time.deltaTime;
			myTransform.localPosition = Vector3.MoveTowards (myTransform.localPosition, startPosition, step);
			//...and if the camera arrives to its start position... 
			if (myTransform.localPosition.y == startPosition.y) {
				enabled = false;							//...disable this script to avoid use the update method.
			}
		}
	}
}
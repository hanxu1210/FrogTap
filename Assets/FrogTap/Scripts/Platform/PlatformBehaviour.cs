using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBehaviour : MonoBehaviour {
	
	//Public variables
	public float speed = 6f;										//How much speed will be applied to platform in y axis.

	//Public variables hidden in inspector
	[HideInInspector]
	public Vector3 startPosition;									//Store start position of this object.

	//Private variables
	private Transform myTransform;									//Reference to this object's transform component.
	private Vector3 positionToReach;								//Position where platform will move.
	private bool isMovingUp;										//True if the start platform is moving to its start position.


	// Use this for initialization
	void Start () {
		//Setting up references.
		myTransform = transform;
		this.enabled = false;								//...disable this script to avoid use the update method.
	}
	
	// Update is called once per frame
	void Update () {
		var distanceToReach = (myTransform.position - positionToReach).sqrMagnitude;	//Get the distance of position to reach.

		//If the platform arrives to its goal...
		if (distanceToReach <= 0.001f) {
			//...check if this platform is start platform and it was moving to its start position...
			if (isMovingUp) {
				GameController.instance.playerControlScript.AppearCharacter ();			//...and appear the character.
				isMovingUp = false;							//Set this variable to false to use this script in a normal way.
			}
			this.enabled = false;							//...disable this script to avoid use the update method.
			return;
		}

		//Move platform to position desired
		myTransform.position = Vector3.Lerp (myTransform.position, positionToReach, Time.deltaTime * speed);
	}

	// This method move the platform to down depending on the kind of platform (last or current)
	public void MoveDown(bool moveOutCamera){
		//If this platform will move out of the camera view...
		if (moveOutCamera) {
			positionToReach = new Vector3 (myTransform.position.x, -7.5f, 0);	 //...it means this object is last platform.
		} else {
			//Otherwise this object is current platform (character is on it).
			positionToReach = new Vector3 (myTransform.position.x, -2.7f, 0);
		}
		this.enabled = true;								//Enable this script to use the update method.
	}

	// This method is used to move the start platform to its start position dynamically
	public void MoveUp(){
		positionToReach = startPosition;
		isMovingUp = true;
		this.enabled = true;								//Enable this script to use the update method.
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerBehaviour : MonoBehaviour {

	//Public variables
	public float speed = 6f;										//How much speed will be applied to container in y axis.

	//Private variables
	private Transform myTransform;									//Reference to this object's transform component.
	private Vector3 positionToReach;								//Position where container will move.


	// Use this for initialization
	void Start () {
		//Setting up references.
		myTransform = transform;
		positionToReach = new Vector3 (0, 2.5f, 0);
	}
	
	// Update is called once per frame
	void Update () {
		var distanceToReach = (myTransform.position - positionToReach).sqrMagnitude;	//Get the distance of position to reach.

		//If the container arrives to its goal...
		if (distanceToReach <= 0.001f) {
			this.enabled = false;							//...disable this script to avoid use the update method.
			return;
		}

		//Move container to position desired
		myTransform.position = Vector3.Lerp (myTransform.position, positionToReach, Time.deltaTime * speed);
	}
}

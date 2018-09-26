using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundElement : MonoBehaviour {

	//Public variables
	public float minY;												//Minimum distance in the y axis where the element will be positioned.
	public float maxY;												//Maximum distance in the y axis where the element will be positioned.
	public float minSize;											//Minimum size the element will have.
	public float maxSize;											//Maximum size the element will have.
	public float maxSpeed;											//Maximum speed of the element on the x axis.

	//Private variables
	private Transform myTransform;									//Reference to this object's transform component.
	private Rigidbody2D myRigidbody2D;								//Reference to this object's rigidbody2D component.
	private Vector2 speed;											//Speed according to element's size.


	// Use this for initialization
	void Start () {
		//Setting up references.
		myTransform = transform;
		myRigidbody2D = GetComponent<Rigidbody2D> ();

		//Generate and set a random size for the element.
		float randomSize = Random.Range (minSize, maxSize);
		myTransform.localScale = new Vector3 (randomSize, randomSize, myTransform.position.z);
		//Set a speed according to element's size (The smaller the element, the faster it will be).
		float minSpeed = minSize * maxSpeed / maxSize;
		float newSpeed = maxSpeed + minSpeed - (randomSize * maxSpeed / maxSize);
		speed = new Vector2 (newSpeed, 0);
		//Reposition the element.
		Vector3 randomPos = new Vector3 (Random.Range (GameController.instance.leftWall.position.x, GameController.instance.rightWall.position.x), 
			                    Random.Range (minY, maxY), myTransform.position.z);
		myTransform.position = randomPos;
	}

	//	Method used to move the element on the x axis.
	void FixedUpdate(){
		myRigidbody2D.MovePosition (myRigidbody2D.position + speed * Time.fixedDeltaTime);
	}

	// Detect collisions with this object
	void OnTriggerExit2D(Collider2D col){
		//If this object collides with left wall...
		if (col.name == GameController.instance.leftWall.name) {
			//...reposition the element.
			myTransform.position = new Vector3 (GameController.instance.rightWall.position.x, Random.Range (minY, maxY), 
				myTransform.position.z);
		}
	}
}

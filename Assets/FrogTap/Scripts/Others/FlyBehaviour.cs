using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyBehaviour : MonoBehaviour {

	//Private variables
	private Transform myTransform;									//Reference to this object's transform component.
	private Animator myAnimator;									//Reference to this object's animator component.


	// Use this for initialization
	void Start () {
		//Setting up references.
		myTransform = transform;
		myAnimator = GetComponent<Animator> ();
	}

	// Set the fly as hidden
	public void FlyHidden(){
		myTransform.parent.SetParent (null);					//Set the fly as independent object.
		myAnimator.SetBool ("IsDeactivate", true);				//Show fly's disappear animation.
	}

	// Set the fly as dead
	public void FlyDead(){
		myTransform.parent.SetParent (null);					//Set the fly as independent object.
		myAnimator.SetBool ("IsDeath", true);					//Show fly's death animation.
	}

	// Reset fly's values
	public void Restart(){
		//Set the fly with others flies.
		myTransform.parent.SetParent (GameController.instance.levelManager.parentFlies);
		myTransform.parent.localPosition = Vector3.zero;
		//Show fly's normal animation.
		myAnimator.SetBool ("IsDeath", false);
		myAnimator.SetBool ("IsDeactivate", false);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateObject : MonoBehaviour {

	// This method is invoked when an object's animation has finished. 
	public void Deactivate(){
		this.gameObject.SetActive (false);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTongueBehaviour : MonoBehaviour {

	//Public variables
	public PlayerControl playerControl;								//Reference to the playerControl script.


	// Detect collisions with this object "Tongue_end"
	void OnTriggerEnter2D(Collider2D col){
		//If this object collides with a wall...
		if (col.CompareTag ("Wall")) {
			playerControl.ReturnTongue ();				//...the tongue will return to its origin to get a collision with the character. 
		} 
		//Otherwise, if this object collides with the character...
		else if (col.CompareTag ("Player")) {
			playerControl.ContinueRotation ();						//...the player can continue playing.
		} 
		//Otherwise, if this object collides with a fly...
		else if (col.CompareTag ("Fly")) {
			col.GetComponent<FlyBehaviour> ().FlyDead ();			//...set the fly as dead...	
			GameController.instance.IncreaseFlies (1);				//...and increase the number of flies collected.
		} 
		//Otherwise, if this object collides with a platform...
		else if (col.CompareTag ("Platform")) {
			playerControl.MoveCharacterToUP (col.transform, true);	//...the character will move to that platform.
		} 
		//Otherwise, if this object collides with the top spikes...
		else if (col.name == "Spikes_top") {
			playerControl.MoveCharacterToUP (col.transform, false);	//...the character will move to "top spikes".
		}
	}
}

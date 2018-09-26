using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterProduct : MonoBehaviour {

	//Public variables
	[Header("UI")]
	public GameObject panelPrice;									//GameObject price panel.
	public Text txtPrice;											//Text to display the price of this character.
	[Header("Product and Price")]
	public GameObject frog;											//Character prefab related to this product.
	public int price;												//Price for this product.

	//Private variables
	private bool isBought;											//True if this product has been bought.


	// Use this for initialization
	void Start () {
		//Check if this product has been bought.
		isBought = Utilities.GetBool (frog.name, false);

		//If this product has been bought...
		if (isBought)
			panelPrice.SetActive (false);							//...deactivate the price panel.
		else
			//Otherwise...
			txtPrice.text = price.ToString();						//...show its price.
	}

	// Method used to choose a character to play
	public void SelectProduct () {
		//If this product has been bought...
		if (isBought) {
			//...check if the current character is different to this product...
			if (GameController.instance.playerControlScript.name != frog.name) {
				GameController.instance.reskinManager.SelectWorld (frog.name);	//...reskin the game...
				PlayerPrefs.SetString ("FrogSelected", frog.name);				//...and save it as selected.
			}
			GameController.instance.GoHomeFromShop ();				//...and go to main menu.
		} 
		else {
			//Otherwise, try to buy the character...
			int amountFlies = GameController.instance.fliesCounter;		//...get the amount of flies collected...
			//...if there are enough flies to pay the price of this product...
			if (amountFlies >= price) {
				isBought = true;										//...then buy it...
				panelPrice.SetActive (false);							//...deactivate the price panel...
				Utilities.SetBool (frog.name, isBought);				//...save the frog as bought...

				//and decrease, update, save and display the amount of flies.
				amountFlies -= price;
				GameController.instance.fliesCounter = amountFlies;
				PlayerPrefs.SetInt ("Flies", amountFlies);
				GameController.instance.UIGame.txtFliesShop.text = amountFlies.ToString ();
			} else {
				//...otherwise, play no enough flies sound.
				Utilities.PlaySFX (GameController.instance.audioGame.generalAudioSource,
					GameController.instance.audioGame.noEnoughFliesSFX, 1f);
			}
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReskinManager : MonoBehaviour {

	//Public variables
	public SpriteRenderer background;								//Reference to background's spriteRenderer component.
	public World[] worlds;											//List of available worlds.

	//Private variables
	private Transform parentElements;								//Parent of all background elements.
	private Vector3 startFrogPosition = new Vector3 (0, -2.25f, 0);	//Position where the frog will be created.
	private bool gameStarts = true;									//True if it's the first time the game starts.


	// Use this for initialization
	void Start () {
		//Load the name of the frog stored.
		string frogSelected = PlayerPrefs.GetString ("FrogSelected", worlds [0].character.name);

		//If the frog stored is the same to default frog, create the default world.
		if (frogSelected == worlds [0].character.name) {
			CreateBackgroundElements (worlds [0].bgElements);			//Create the default background elements.
			//Set the default start position to start platform.
			Transform startPlatform = GameController.instance.levelManager.startPlatform;
			startPlatform.GetComponent<PlatformBehaviour> ().startPosition = worlds [0].startPlatformPosition;
			//Create the default frog.
			GameObject temFrog = (GameObject)Instantiate (worlds [0].character, startFrogPosition, Quaternion.identity);
			temFrog.name = worlds [0].character.name;
			GameController.instance.playerControlScript = temFrog.GetComponent<PlayerControl> ();
			//Create the default flies and place the first container.
			GameController.instance.levelManager.PlaceFirstContainer (worlds [0].fly);
			gameStarts = false;						//After this method is called it isn't the first time the game starts.
		} 
		else {
			//Otherwise, create the world of frog stored.
			SelectWorld (frogSelected);
		}
	}

	// This method search the world of the frog given
	public void SelectWorld(string nameCharacter){
		for (int i = 0; i < worlds.Length; i++) {
			if (worlds [i].character.name == nameCharacter) {
				ChangeSkin (i);
				break;
			}
		}
	}

	// Method called to change the skin of game depending on world selected
	void ChangeSkin(int worldIndex){
		background.sprite = worlds [worldIndex].background;				//Change the background.
		//If there is a parent for all background elements.
		if (parentElements != null)
			Destroy (parentElements.gameObject);						//...destroy it.
		CreateBackgroundElements (worlds [worldIndex].bgElements);		//Create the background elements for this world.

		//Change the skin of start platform and reposition it.
		Transform startPlatform = GameController.instance.levelManager.startPlatform;
		startPlatform.GetComponent<SpriteRenderer> ().sprite = worlds [worldIndex].startPlatform;
		startPlatform.position = worlds [worldIndex].startPlatformPosition;
		startPlatform.GetComponent<PlatformBehaviour> ().startPosition = startPlatform.position;
		//If it's the first time the game starts... 
		if (gameStarts) {
			//...create this world's flies and place the first container.
			GameController.instance.levelManager.PlaceFirstContainer (worlds [worldIndex].fly);
			gameStarts = false;						//Don't enter to this block of code again.
		} else {
			//Otherwise, replace the current flies for this world's flies.
			GameController.instance.levelManager.ReplaceFlies (worlds [worldIndex].fly);
		}

		//Change the skin of each platform.
		List<Transform> platformsList = GameController.instance.levelManager.platformsList;
		for (int i = 0; i < platformsList.Count; i++) {
			platformsList [i].GetComponent<SpriteRenderer> ().sprite = worlds [worldIndex].platform;
		}

		//If exist a character...
		if (GameController.instance.playerControlScript != null)
			Destroy (GameController.instance.playerControlScript.gameObject);		//...destroy it.
		//Create this world's frog.
		GameObject temFrog = (GameObject)Instantiate (worlds [worldIndex].character, startFrogPosition, Quaternion.identity);
		temFrog.name = worlds [worldIndex].character.name;
		GameController.instance.playerControlScript = temFrog.GetComponent<PlayerControl> ();
	}

	// Method used to create background elements like clouds, planets and ghosts
	void CreateBackgroundElements(BgElement[] bgElements){
		//Create the parent of all background elements.
		parentElements = new GameObject ("BG_elements").transform;
		parentElements.SetParent (GameController.instance.cameraShake.transform);

		//Loop through the collection "bgElements" and in each iteration...
		for (int i = 0; i < bgElements.Length; i++) {
			//...create the amount desired of each background element.
			for (int a = 0; a < bgElements [i].amount; a++) {
				InstantiateObject (bgElements [i].element, Vector3.zero, parentElements);
			}
		}
	}

	// This method instantiate an object and return it
	GameObject InstantiateObject(GameObject obj, Vector3 pos, Transform parentObject){
		//Create the object
		GameObject temObj = (GameObject)Instantiate (obj, pos, Quaternion.identity);
		temObj.transform.SetParent (parentObject);				//set it a parent to keep the hierarchy clean.
		return temObj;											//and return it.
	}
}

[System.Serializable]
public class World {
	[Header("Enviroment")]
	public Sprite background;									//Background sprite.
	public BgElement[] bgElements;								//List of background elements prefabs.
	[Header("Platforms")]
	public Sprite startPlatform;								//Start platform sprite.
	public Vector3 startPlatformPosition;						//Start platform position.
	public Sprite platform;										//Platform sprite.
	[Header("Fly")]
	public GameObject fly;										//Fly prefab to instantiate.
	[Header("Character")]
	public GameObject character;								//Character prefab to instantiate.
}

[System.Serializable]
public class BgElement {
	public GameObject element;									//Background element prefab.
	public int amount;											//How many background elements will be instantiated for this prefab.
}

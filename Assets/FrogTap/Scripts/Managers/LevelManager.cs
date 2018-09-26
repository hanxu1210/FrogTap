using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	//Public variables
	[Header("Platforms")]
	public GameObject platform;										//Platform prefab to instantiate.
	public int amountPlatforms = 3;									//Amount of platforms to instantiate.
	public float minWidthSize;										//Minimum width size the platform will have.
	public float maxWidthSize;										//Maximum width size the platform will have.
	public Transform startPlatform;									//Platform where frog starts.
	public Transform parentPlatforms;								//Parent of all platforms to keep the hierarchy clean.
	[Header("Platform's position")]									//The next variables will be used for horizontal, vertical and static containers.
	public float minX;												//Minimum distance in the x axis where the platforms will be positioned.
	public float maxX;												//Maximum distance in the x axis where the platforms will be positioned.
	public float minY;												//Minimum distance in the y axis where the platforms will be positioned.
	public float maxY;												//Maximum distance in the y axis where the platforms will be positioned.
	[Header("Levels")]
	public List<Transform> containersCycle;							//Collection of containers that will be used as cycle.
	public int containersPerLevel = 5;								//Number of containers per level.
	[Header("Difficulty")]
	public float[] containerSpeed;									//The speed of each container will increase when the cycle is repeated.

	//Public variables hidden in inspector
	[HideInInspector]
	public List<Transform> platformsList;							//List of platforms which will be pooled (reused).
	[HideInInspector]
	public Transform parentFlies;									//Parent of all flies to keep the hierarchy clean.
	[HideInInspector]
	public List<Transform> fliesList;								//List of flies which will be pooled (reused).
	[HideInInspector]
	public ContainerBehaviour currentScriptContainer;				//Reference to current container's containerBehaviour script.

	//Private variables
	private List<Animator> containerAnimatorList;					//List of the animators component of the containers.
	private Animator currentContainerAnimator;						//Store the animator component of current container.
	private Transform container;									//Store current container's tranform component.
	private Transform pointPlatform;								//Store the child object "pointPlatform" of current container.
	private Vector3 containerStartPosition;							//Store start position of one container to be used as a reference.  
	private int counterPlatforms;									//Counter used to move within the array "platformsList".
	private int counterContainers;									//Counter used to move within the array "containersCycle".
	private int counterContainerSpeed;								//Counter used to move within the array "containerSpeed".
	private int counterFlies;										//Counter used to move within the array "fliesList".
	private int counterAchieve;										//Counter used to know when increase the level.
	private float currentContainerSpeed;							//Store current container's speed.


	// Use this for initialization
	void Start () {
		InstantiatePlatforms();									//Create the necessary platforms to reuse them throughout the game.
		//Create the parent of all flies to keep the hierarchy clean.
		GameObject obj = new GameObject ("Flies");
		parentFlies = obj.transform;
		parentFlies.position = new Vector3 (-999, 0, 0);

		//Store the animator component of each container
		containerAnimatorList = new List<Animator> ();
		for (int i = 0; i < containersCycle.Count; i++) {
			containerAnimatorList.Add (containersCycle [i].GetComponent<Animator> ());
		}

		containerStartPosition = containersCycle [counterContainers].position;		//Store the start position of the first container.
		//Set the next values to the next variables to use correctly the method "PlacePlatform" just for the first time we enter this method.
		counterAchieve = containersPerLevel;
		counterContainers = -1;
		currentContainerSpeed = containerSpeed [0];				//Get the start speed for the containers.
	}

	// Method invoked by reskin manager to place the first container of cycle.
	public void PlaceFirstContainer(GameObject fly){
		InstantiateFlies(fly);									//Create the necessary flies to reuse them throughout the game.
		PlacePlatform ();										//Place the first container of cycle.
		currentScriptContainer.enabled = true;					//Move the first container to the camera view.
	}

	// Instantiate platforms according to amount of platforms required and add them to the list "platformsList".
	void InstantiatePlatforms(){
		//Loop the correct number of times and in each iteration...
		for (int i = 0; i < amountPlatforms; i++) {
			//...create a platform and add it to its corresponding list.
			platformsList.Add (InstantiateObject (platform, new Vector3 (-999f, 0, 0), i, parentPlatforms).transform);
		}
	}

	// Instantiate a fly per platform and add it to the list "fliesList".
	void InstantiateFlies(GameObject objFly){
		//Loop the correct number of times and in each iteration...
		for (int i = 0; i < amountPlatforms; i++) {
			//...create a fly and add it to its corresponding list.
			fliesList.Add (InstantiateObject (objFly, new Vector3 (-999f, 0, 0), i, parentFlies).transform);
		}
	}

	// Method used to replace the current flies
	public void ReplaceFlies(GameObject newFly){
		//If the new fly to instantiate is the same to current fly, then do nothing.
		if ((newFly.name + "0") == fliesList [0].name)
			return;
		
		//Loop the correct number of times and in each iteration...
		for (int i = 0; i < fliesList.Count; i++) {
			Destroy (fliesList [i].gameObject);						//...delete the fly.
		}

		//Loop the correct number of times and in each iteration...
		for (int i = 0; i < fliesList.Count; i++) {
			//...create a fly and replace it into the list "fliesList".
			fliesList [i] = InstantiateObject (newFly, fliesList [i].position, i, fliesList [i].parent).transform;
		}
	}

	// This method place each platform just up of the character to create the game levels.
	public void PlacePlatform(){
		//Check if the variable "counterAchieve" reachs the number of platforms per level. 
		if (counterAchieve == containersPerLevel) {
			//If the current container has an animation...
			if (currentContainerAnimator != null) {
				currentContainerAnimator.enabled = false;			//...then disable it...
				currentContainerAnimator = null;					//...and set it as null.
			}

			counterContainers++;									//Increase this counter to use the next container of cycle.
			//If the array "containersCycle" has reached its limit...
			if (counterContainers == containersCycle.Count){
				counterContainerSpeed++;							//...increase the speed of the containers...
				//...check if the array "containerSpeed" has reached its limit...
				if (counterContainerSpeed == containerSpeed.Length)
					counterContainerSpeed = 0;						//...and restart this counter to use the array "containerSpeed" again.
				currentContainerSpeed = containerSpeed [counterContainerSpeed];		//Get the speed for the containers.
				counterContainers = 0;								//...restart this counter to use the array "containersCycle" again.
			}
			container = containersCycle [counterContainers];		//Get the corresponding container of cycle.
			pointPlatform = container.GetChild (0);					//Get the child "pointPlatform" of the container.

			//If the current container has an animation...
			if (containerAnimatorList [counterContainers] != null) {
				currentContainerAnimator = containerAnimatorList [counterContainers];	//...get it...
				currentContainerAnimator.speed = currentContainerSpeed;					//...set the current speed...
				currentContainerAnimator.enabled = true;								//...and show the animation.
			}
			counterAchieve = 0;										//Reset this counter to use it again.
		}
		counterAchieve++;											//Increase this counter to reach the number of platforms per level.

		Transform platform = platformsList [counterPlatforms];		//Get the next platform in the platform list.
		platform.SetParent (pointPlatform);							//Set this platform as child of current container.

		//Depending on the container, this one will have a different configuration
		switch (container.name) {
		case "Container_static":
			//Static container, the platform will be positioned randomly in x and y axis.
			platform.localPosition = new Vector3 (Random.Range (minX, maxX), Random.Range (minY, maxY), 0);
			break;
		case "Container_horizontal":
			//Horizontal movement, the platform will be positioned randomly in y axis.
			platform.localPosition = new Vector3 (0, Random.Range (minY, maxY), 0);
			break;
		case "Container_vertical":
			//Vertical movement, the platform will be positioned randomly in x axis.
			platform.localPosition = new Vector3 (Random.Range (minX, maxX), 0, 0);
			break;
		default:
			//Circular, rectangular and triangular container.
			platform.localPosition = Vector3.zero;		//The platform will be positioned in the center of container.
			ChangeAnimationDirection ();				//Change or not the direction of the container's animation.
			break;
		}

		platform.localScale = new Vector3 (Random.Range (minWidthSize, maxWidthSize), 1, 1);		//Change the platform's size.
		Transform fly = fliesList [counterFlies];								//Get the next fly in the flies list.
		fly.SetParent(platform);												//Set the next fly as child of the platform.
		fly.localPosition = new Vector3 (0, -0.4f, 0);							//Change the fly's local position.

		container.position = containerStartPosition;							//Reset container's position.
		currentScriptContainer = container.GetComponent<ContainerBehaviour>();	//Get current container's containerBehaviour script. 

		counterPlatforms++;				//Increase this counter for next time we enter to this method, use the next platform in the list.
		counterFlies++;					//Increase this counter for next time we enter to this method, use the next fly in the list.
		//If the array "platformsList" has reached its limit...
		if (counterPlatforms == platformsList.Count)
			counterPlatforms = 0;						//...restart this counter to use the array again.
		//If the array "fliesList" has reached its limit...
		if (counterFlies == fliesList.Count)
			counterFlies = 0;							//...restart this counter to use the array again.
	}

	// To create variety in the animations, this method changes the direction of the container's animation.
	void ChangeAnimationDirection(){
		bool changeContainerAnimation = Random.Range (0, 2) == 0;

		if (changeContainerAnimation) {
			if (currentContainerAnimator.GetBool ("IsReverse"))
				currentContainerAnimator.SetBool ("IsReverse", false);		//Show normal animation.
			else
				currentContainerAnimator.SetBool ("IsReverse", true);		//Show reverse animation.
		}
	}

	// This method instantiate an object and return it
	GameObject InstantiateObject(GameObject obj, Vector3 pos, int numberObject, Transform parentObject){
		//Create the object
		GameObject temObj = (GameObject)Instantiate (obj, pos, Quaternion.identity);
		temObj.transform.SetParent (parentObject);				//set it a parent to keep the hierarchy clean.
		temObj.name = obj.name + numberObject.ToString ();		//set it the original name plus number of object.
		return temObj;											//and return it.
	}

	// Reset level manager's values
	public void Restart(){
		//Loop through the collection "platformsList" and in each iteration...
		for (int i = 0; i < amountPlatforms; i++) {
			//...reposition each platform out of the camera view...
			platformsList [i].position = new Vector3 (-999f, 0, 0);
			platformsList [i].SetParent (parentPlatforms);
			//...and if there is a fly attached to this platform, remove it.
			if (platformsList [i].childCount > 0)
				platformsList [i].GetChild (0).SetParent (parentFlies);
		}
		//Set the next values to the next variables to use correctly the method "PlacePlatform".
		counterAchieve = containersPerLevel;
		counterContainers = -1;
		counterContainerSpeed = 0;
		currentContainerSpeed = containerSpeed [counterContainerSpeed];

		PlacePlatform ();								//Place the first container of cycle.
		currentScriptContainer.enabled = true;			//Move the first container to the camera view.
	}
}

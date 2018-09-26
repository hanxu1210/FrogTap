using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour {

	//Public variables
	[Header("Character")]
	public Transform bodyBottom;									//Reference to bottom character's transform component.
	public Transform bodyTop;										//Reference to top character's transform component.
	public float characterSpeed = 5f;								//How much speed will be applied to character in y axis.
	[Header("Tongue")]
	public Transform tongueTransform;								//Reference to tongue's transform component.
	public Transform endTongue;										//Reference to end tongue's transform component.
	public float speedTongue = 1f;									//How fast the tongue will move.
	[Header("Indicator")]
	public Transform indicatorTransform;							//Reference to indicator's transform component.
	public Transform indicatorDirection;							//Reference to indicator direction's transform component.
	public float speedIndicator = 3f;								//How fast the indicator will rotate.
	[Header("Effect")]
	public GameObject smokeFX;										//Effect used when the character dies. 
	[Header("Audio Clips")]
	public AudioClip launchTongueSFX;								//Sound used when the character launches its tongue.
	public AudioClip crashSFX;										//Sound used when the character crashes with the spikes.
	public AudioClip scoreSFX;										//Sound used when the character reaches the next platform.

	//Private variables
	private Camera myCamera;										//Reference to main camera.
	private Transform myTransform;									//Reference to this object's transform component.
	private CircleCollider2D myCircleColider2D;						//Reference to this object's circleCollider2D component.
	private Animator myAnimator;									//Reference to this object's animator component.
	private AudioSource myAudioSource;								//Reference to frog's audio source component.
	private GameObject crashingFX;									//Effect used when the character crashes with the spikes.
	private Transform currentPlatform;								//Current platform is when the character is up.
	private Transform lastPlatform = null;							//Last platform is when the character leaves it.
	private CircleCollider2D circleColliderEndTongue;				//Reference to end tongue's circleCollider2D component.
	private LineRenderer lineRenderTongue;							//Reference to tongue's line renderer component.
	private Vector3 directionTongueTravel;							//Direction where tongue will move.
	private Vector3 positionToReach;								//Position where character will move when this one goes up.
	private Vector3 startPosition;									//Store start position of this object.
	private Vector2 speedPlatform = new Vector2 (0, -0.34f);		//How fast the current platform will move to down.
	private bool isControlActivated;								//True if the controls are activated.
	private bool moveCharacter;										//True if character can move.
	private bool rotateIndicator;									//True if the indicator can rotate.
	private bool moveTongue;										//True if tongue can move.
	private bool movePlatformToDown;								//True if current platform can go down.
	private bool characterGoToNextPlatform;							//False if the character goes to top spikes.
	private bool tongueCanCollide;									//Used when the tongue returns and this one needs to collide with the character.
	private float minDistanceToGoal;								//Minimum distance the character needs to reach its goal.


	// Use this for initialization
	void Start () {
		//Setting up references.
		myCamera = Camera.main;
		myTransform = transform;
		myCircleColider2D = GetComponent<CircleCollider2D> ();
		myAnimator = GetComponent<Animator> ();
		myAudioSource = GameController.instance.audioGame.frogAudioSource;
		lineRenderTongue = tongueTransform.GetComponent<LineRenderer> ();
		circleColliderEndTongue = endTongue.GetComponent<CircleCollider2D> ();
		crashingFX = GameController.instance.crashingFX;

		indicatorTransform.gameObject.SetActive (false);						//Hide indicator object.
		startPosition = myTransform.position;									//Store start position of this object.
		currentPlatform = GameController.instance.levelManager.startPlatform;	//Set the start Platform as current platform.
		myTransform.SetParent (currentPlatform);								//Set the start platform as parent of the character.
	}

	// Update is called once per frame
	void Update () {
		//If the time is paused, then do nothing.
		if (Time.timeScale == 0)
			return;

		//If we are running on unity editor, use mouse control to play.
		#if UNITY_EDITOR
		//If we get one left click of mouse on the screen and the controls are activated...
		if ((Input.GetMouseButtonDown(0)||Input.GetKeyDown (KeyCode.JoystickButton0)) && isControlActivated) {
			LaunchTongue();			//...get the mouse position to launch the frog's tongue.
		}
		//Otherwise, if we are running on Android, iOS, Windows Metro or Unity Windows Phone 8.1, use touch control to play.
		#elif UNITY_ANDROID || UNITY_IOS || UNITY_METRO || UNITY_WP_8_1
		//If we get one touch on the screen and the controls are activated...
		if (Input.GetKeyDown (KeyCode.JoystickButton0) && isControlActivated) {
			Touch myTouch = Input.touches [0];			//...store the first touch detected...

			//...check if the phase of that touch is equal to began...
			// if (myTouch.phase == TouchPhase.Began) {
			// 	LaunchTongue(myTouch.position);			//... and get the touch position to launch the frog's tongue.
			// }
			LaunchTongue();
		}
		#endif

		//Indicator's rotation
		if (rotateIndicator) {
			Vector3 rot = indicatorTransform.localEulerAngles;
			rot.z += speedIndicator;
			indicatorTransform.localEulerAngles = rot;
		}

		//Draw the tongue
		if (moveTongue) {
			endTongue.Translate (directionTongueTravel * speedTongue * Time.deltaTime, Space.World);
			lineRenderTongue.SetPosition (1, new Vector3 (endTongue.localPosition.x, endTongue.localPosition.y, 0));
		}

		//Character goes up (Next platform or top spikes)
		if (moveCharacter) {
			var distanceToReach = (myTransform.position - positionToReach).sqrMagnitude;	//Get the distance of character's goal.

			//If the character arrives to its goal...
			if (distanceToReach <= minDistanceToGoal) {
				//...verifies if the goal was the next platform...
				if (characterGoToNextPlatform) {
					Restart (true);									//...and reset character's values.
				} else {
					//...otherwise, the goal was the top spikes and the game ends.
					CharacterDead ();
				}
				return;
			}

			//Move character to position desired
			myTransform.position = Vector3.Lerp (myTransform.position, positionToReach, Time.deltaTime * characterSpeed);
			lineRenderTongue.SetPosition (0, new Vector3 (myTransform.localPosition.x, myTransform.localPosition.y, 0));
		}

		//Current platform goes down
		if (movePlatformToDown) {
			currentPlatform.Translate (speedPlatform * Time.deltaTime);
		}
	}

	// Detect collisions with this object
	void OnTriggerEnter2D(Collider2D col){
		//If the character collides with the bottom spikes...
		if (col.name == "Spikes_bottom") {
			isControlActivated = false;						//...deactivate the controls...
			movePlatformToDown = false;						//...stop current platform's movement...
			moveTongue = false;								//...stop tongue's movement...
			CharacterDead ();								//...and the game ends.
		}
	}

	// In main menu the character is static, with this method the user enable the character to play
	public void EnableCharacter(){
		isControlActivated = true;							//Activate the controls.
		rotateIndicator = true;								//Indicator can rotate.
		indicatorTransform.gameObject.SetActive (true);		//Show indicator object.
	}

	// In game over menu when the start platform arrives to its start position, this method is called to appear the character.
	public void AppearCharacter(){
		smokeFX.SetActive (true);							//Show smoke effect.
		myTransform.position = startPosition;				//Set its start position.
	}

	// Method used to launch frog's tongue when the user tap on the screen
	void LaunchTongue(){
		//Avoid to launch the tongue when the user tap on pause button. 
		// Vector2 myTapPos = myCamera.ScreenToWorldPoint (tapPosition);
		// if (myTapPos.y > 5)
		// 	return;

		rotateIndicator = false;										//Stop indicator's rotation.
		Vector3 targetPosition = indicatorDirection.position;			//Get position where tongue will move.
		Vector3 currentPosition = endTongue.position;					//Get current tongue position.
		directionTongueTravel = targetPosition - currentPosition;		//Get the difference between target and current position.
		directionTongueTravel.Normalize ();								//Get the direction where tongue will translate.
		moveTongue = true;												//Tongue can move.
		circleColliderEndTongue.enabled = true;							//Tongue can detect collisions.
		bodyTop.localEulerAngles = new Vector3 (0, 0, 50);				//Character opens its mouth.
		Utilities.PlaySFX (myAudioSource, launchTongueSFX, 1f);			//Play launch sound.

		//If the tongue's speed is negative...
		if (speedTongue < -1) {
			speedTongue *= -1;											//...then, change it to positive speed to launch it.
		}
		indicatorTransform.gameObject.SetActive(false);					//Hide indicator object.
		isControlActivated = false;										//Deactivate the controls.
	}

	// Method called when the tongue collides with a wall
	public void ReturnTongue(){
		tongueCanCollide = true;							//Tongue can collide with the character.
		//If the tongue's speed is positive...
		if (speedTongue > -1) {
			speedTongue *= -1;								//...then, change it to negative speed to return the tongue.
		}
	}

	// This method is invoked when the indicator has to continue rotating
	public void ContinueRotation(){
		//If the tongue can't collide with the character, then do nothing.
		if (!tongueCanCollide)
			return;

		bodyTop.localEulerAngles = Vector3.zero;				//Character closes its mouth.
		tongueCanCollide = false;								//Tongue cannot collide with the character.
		moveTongue = false;										//Stop tongue's movement.
		circleColliderEndTongue.enabled = false;				//Tongue cannot detect collisions (Avoid mistakes).
		endTongue.localPosition = new Vector3 (0, 0, -1.5f);	//Reset end tongue's local position.
		lineRenderTongue.SetPosition (1, Vector3.zero);			//Set default tongue's parameter (position 1).
		indicatorTransform.gameObject.SetActive(true);			//Show indicator object.
		rotateIndicator = true;									//Indicator continue rotating.
		isControlActivated = true;								//Activate controls.
	}

	// Method used to move the character to UP (Next platform or Top spikes) depending on object detected.
	public void MoveCharacterToUP(Transform objectDetected, bool isPlatform){
		//If the platform obtained is the same to current platform, then do nothing.
		if (myTransform.parent.name == objectDetected.name)
			return;
		
		moveTongue = false;										//Stop tongue's movement.
		circleColliderEndTongue.enabled = false;				//Tongue cannot detect collisions (Avoid mistakes).
		tongueTransform.SetParent (null);						//Tongue object doesn't have parent.
		myTransform.SetParent (tongueTransform);				//Character follows its tongue.
		movePlatformToDown = false;								//Stop current platform's movement.
		lastPlatform = currentPlatform;							//Set current platform as last platform.
		currentPlatform = objectDetected;						//Set next platform as current platform.
		positionToReach = new Vector3 (endTongue.position.x, currentPlatform.position.y + 0.5f, 0);		//Get position to travel.
		bodyTop.localEulerAngles = Vector3.zero;				//Character closes its mouth.
		moveCharacter = true;									//Character can move to position desired.
		//If the object obtained is a platform...
		if (isPlatform) {
			//...verify if there is a fly attached to this platform...
			if (objectDetected.childCount > 0)
				objectDetected.GetChild (0).GetChild (0).GetComponent<FlyBehaviour> ().FlyHidden ();	//... and hide it.

			currentPlatform.parent = null;						//...set the platform as independent object to use it again...
			//...and before to move the character to next platform, place the next platform.
			GameController.instance.levelManager.PlacePlatform ();
		}
		characterGoToNextPlatform = isPlatform;					//Assign if character goes to next platform or top spikes.

		//Set minimum distance the character needs to reach its goal.	
		if (characterGoToNextPlatform)
			minDistanceToGoal = 0.001f;
		else
			minDistanceToGoal = 0.8f;
	}

	// The character dies when this one crash with the spikes
	void CharacterDead(){
		moveCharacter = false;										//Stop character's movement.
		bodyTop.localEulerAngles = Vector3.zero;					//Character closes its mouth.
		myAnimator.SetBool ("IsDeath", true);						//Show character's death animation. 
		myCircleColider2D.enabled = false;							//Character cannot detect collisions with other objects.
		myTransform.SetParent (null);								//Character is an independent object.

		Utilities.PlaySFX (myAudioSource, crashSFX, 1f);			//Play crash sound.
		//Show crash effect.
		crashingFX.transform.position = myTransform.position;
		crashingFX.SetActive (true);
		//Position the character and platforms out of view of the camera.
		myTransform.position = new Vector3 (-6, 0, 0);
		if (currentPlatform.CompareTag ("Platform"))
			currentPlatform.position = new Vector3 (lastPlatform.position.x, -7.5f, 0);
		lastPlatform.position = new Vector3 (lastPlatform.position.x, -7.5f, 0);
		//Move the start platform to its start position.
		GameController.instance.levelManager.startPlatform.GetComponent<PlatformBehaviour> ().MoveUp ();

		//Hide tongue and indicator to see just the character.
		tongueTransform.gameObject.SetActive (false);
		indicatorTransform.gameObject.SetActive (false);
		tongueTransform.SetParent (myTransform);

		GameController.instance.GameOver ();						//Call game over method.
	}

	// Reset character's values, method invoked when the character arrives to next platform or when it dies
	public void Restart(bool isAlive){
		//If character is alive...
		if (isAlive) {
			//...it means the character arrives to next platform and therefore...
			GameController.instance.IncreaseScore ();				//...increase score...
			moveCharacter = false;									//...stop character's movement...
			movePlatformToDown = true;								//...current platform can go down...
			Utilities.PlaySFX (myAudioSource, scoreSFX, 0.6f);		//...play score sound...
			GameController.instance.levelManager.currentScriptContainer.enabled = true;		//...move container to game area...
			currentPlatform.GetComponent<PlatformBehaviour> ().MoveDown (false);			//...move current platform to down...
			lastPlatform.GetComponent<PlatformBehaviour> ().MoveDown (true);				//...and move last platform to out of camera.	
		}
		else {
			//otherwise, the character is died and it's necessary restart individual values of this one 
			myTransform.position = startPosition;				//Set its start position.
			bodyTop.localEulerAngles = Vector3.zero;			//Character closes its mouth.
			myAnimator.SetBool ("IsDeath", false);				//Show character's normal animation. 
			myCircleColider2D.enabled = true;					//Character can detect collisions with other objects.
			currentPlatform = GameController.instance.levelManager.startPlatform;		//Set the startPlatform as current platform.
		}
		myTransform.SetParent (currentPlatform);				//Set the current platform as parent of the character.

		//Reset the values of the character's tongue.
		tongueTransform.SetParent (myTransform);				//Tongue follows the character.
		tongueTransform.localPosition = Vector3.zero;			//Set tongue object in character's center.
		lineRenderTongue.SetPosition (0, Vector3.zero);			//Set default tongue's parameter (position 0).
		lineRenderTongue.SetPosition (1, Vector3.zero);			//Set default tongue's parameter (position 1).
		endTongue.localPosition = new Vector3 (0, 0, -1.5f);	//Reset end tongue's local position.
		tongueTransform.gameObject.SetActive (true);			//Show tongue.
		tongueCanCollide = false;								//Tongue cannot collide with the character.

		//Reset indicator's values.
		indicatorTransform.localEulerAngles = new Vector3 (0, 0, 180f);		//Reset indicator's rotation.
		indicatorTransform.gameObject.SetActive (isAlive);		//Show/Hide indicator object.
		rotateIndicator = isAlive;								//Indicator can or not rotate.
		isControlActivated = isAlive;							//Controls are or not activated to play.
	}

	// Method invoked when the user reset the game from pause menu and it's necessary disable the character to avoid mistakes
	public void DisableCharacter(){
		myCircleColider2D.enabled = false;						//Character cannot detect collisions with other objects.
		moveCharacter = false;									//Stop character's movement.
		moveTongue = false;										//Stop tongue's movement.
		movePlatformToDown = false;								//Stop current platform's movement.

		//Move the start platform to its start position.
		Transform startPlatform = GameController.instance.levelManager.startPlatform;
		startPlatform.position = startPlatform.GetComponent<PlatformBehaviour> ().startPosition;
	}
}

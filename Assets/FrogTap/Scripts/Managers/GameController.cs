using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	//Public variables
	public static GameController instance;							//A public static reference to itself.
	[Header("Script References")]
	public LevelManager levelManager;								//Reference to the levelManager script.
	public ReskinManager reskinManager;								//Reference to the reskinManager script.
	public CameraShake cameraShake;									//Reference to the cameraShake script.
	[Header("Walls")]
	public Transform leftWall;										//Reference to left wall's transform component.
	public Transform rightWall;										//Reference to right wall's transform component.
	[Header("UI Game")]
	public UIGame UIGame;											//Reference to the UIGame script.
	[Header("Audio Game")]
	public AudioGame audioGame;										//Reference to the audioGame script.
	[Header("Frog's crash effect")]
	public GameObject crashingFX;									//Effect used when the character crashes with the spikes.

	//Public variables hidden in inspector
	[HideInInspector]
	public PlayerControl playerControlScript;						//Reference to playerControl script component of frog.
	[HideInInspector]
	public bool SFXOn = true;										//True if sound effects are enabled.
	[HideInInspector]
	public int fliesCounter;										//The flies counter.

	//Private variables
	private int score;												//Score counter.
	private int bestScore;											//Store the best score reached.
	private bool playImmediately;				//True if user plays a new game session immediately or first the main menu is opened.
	private bool playSfXInSoundStateMethod;		//The first time the "UpdateSoundState" method is invoked, the SFX button can't played.


	void Awake(){
		//Ensure that there is only one manager.
		if(instance == null)
			instance = this;
		else
			Destroy (gameObject);

		//Use this line of code to delete values stored such as score, the flies counter and characters purchased.
		//Utilities.DeletePlayerPrefs ();
	}

	// Use this for initialization
	void Start () {
		//Reposition the walls depending on screen's width.
		float boundMaxX = Camera.main.orthographicSize * Screen.width / Screen.height;
		boundMaxX += 0.155f;
		rightWall.position = new Vector3 (boundMaxX, rightWall.position.y, rightWall.position.z);
		leftWall.position = new Vector3 (-boundMaxX, leftWall.position.y, leftWall.position.z);

		//Check if sound effects are enabled and then change the state of sounds.
		SFXOn = Utilities.GetBool("SFXOn", true);
		UpdateSoundState ();

		bestScore = PlayerPrefs.GetInt ("BestScore");				//Load best score stored.
		fliesCounter = PlayerPrefs.GetInt ("Flies");				//Load the amount of flies stored.
	}
	
	// Update is called once per frame
	void Update () {
		//Verifies if user press back button on mobile devices.
		if (Input.GetKeyDown(KeyCode.Escape)){ 
			OnBackButtonPressed();
		}
	}

	public void StartGame(){
		UIGame.menuMain.SetActive (false);					//Hide main menu.
		UIGame.txtFlies.text = fliesCounter.ToString ();	//Display flies counter.
		UIGame.menuGame.SetActive (true);					//Show game menu.
		playerControlScript.EnableCharacter ();				//Enable the character to play.
	}

	public void PauseGame(){
		PlayButtonSound ();								//Play click button sound.
		UIGame.menuPause.SetActive (true);				//Show pause menu.
		Time.timeScale = 0;								//Freeze time.
	}

	public void ResumeGame(){
		PlayButtonSound ();								//Play click button sound.
		UIGame.menuPause.SetActive (false);				//Hide pause menu.
		Time.timeScale = 1;								//The time continues.
	}

	public void GameOver(){
		//Enable camera's shake behaviour.
		cameraShake.shakeDuration = 0.5f;
		cameraShake.enabled = true;

		UIGame.menuGame.SetActive (false);						//Hide game menu.
		UIGame.txtScoreGameOver.text = score.ToString ();		//Display score.
		//If current score is higher than best score...
		if (score > bestScore) {
			bestScore = score;									//...update it...
			PlayerPrefs.SetInt ("BestScore", bestScore);		//...and save it.
		}
		//Display best score.
		UIGame.txtBestScoreGameOver.text = "BEST " + bestScore.ToString ();
		UIGame.menuGameOver.SetActive (true);			//Show game over menu.

		PlayerPrefs.SetInt ("Flies", fliesCounter);		//Save the flies counter.
	}

	public void RestartGame(){
		PlayButtonSound ();								//Play click button sound.
		playImmediately = true;							//Play a new game session immediately.
		StartCoroutine ("ResetValues");					//Call the coroutine function "ResetValues".
	}

	public void OpenShopMenu(){
		PlayButtonSound ();								//Play click button sound.
		UIGame.menuShop.SetActive (true);				//Show shop menu.
		UIGame.menuMain.SetActive (false);				//Hide main menu.
		UIGame.txtFliesShop.text = fliesCounter.ToString ();	//Display flies counter.
	}

	// Method used to go to main menu from gameover menu
	public void GoHomeFromGameOver(){
		PlayButtonSound ();								//Play click button sound.
		playImmediately = false;						//Open the main menu before to play a new game session.
		StartCoroutine ("ResetValues");					//Call the coroutine function "ResetValues".
	}

	// Method used to go to main menu from pause menu
	public void GoHomeFromPauseMenu(){
		PlayButtonSound ();								//Play click button sound.
		UIGame.menuGame.SetActive (false);				//Hide game menu.
		playerControlScript.DisableCharacter();			//Disable the character to avoid mistakes.
		Time.timeScale = 1;								//The time continues.
		fliesCounter = PlayerPrefs.GetInt ("Flies");	//Load the amount of flies stored.
		playImmediately = false;						//Open the main menu before to play a new game session.
		StartCoroutine ("ResetValues");					//Call the coroutine function "ResetValues".
	}

	// Method used to go to main menu from shop menu
	public void GoHomeFromShop(){
		//If gameover menu is active...
		if (UIGame.menuGameOver.activeSelf) {
			GoHomeFromGameOver ();						//...show main menu from game over menu.
		} else {
			//Otherwise...
			PlayButtonSound ();							//...play click button sound...
			UIGame.menuShop.SetActive (false);			//...hide shop menu...
			UIGame.menuMain.SetActive (true);			//...and show main menu.
		}
	}

	// This function reset all values of game to play again  
	IEnumerator ResetValues(){
		UIGame.screenFade.SetActive (true);				//Show fade animation.
		yield return new WaitForSeconds (0.4f);			//To appreciate the fade animation, we wait for 0.4 seconds.

		//Restart score.
		score = 0;
		UIGame.txtScore.text = score.ToString ();
		//Restart scripts.
		levelManager.Restart ();
		playerControlScript.Restart (false);

		UIGame.menuGameOver.SetActive (false);			//Hide game over menu.
		UIGame.menuPause.SetActive (false);				//Hide pause menu.
		UIGame.menuShop.SetActive(false);				//Hide shop menu.

		//If user will play immediately...
		if (playImmediately)
			StartGame ();								//..start a new game session.
		else
			//Otherwise...
			UIGame.menuMain.SetActive (true);			//...show main menu and the user will decide when play.
	}

	public void ShareGame(){
		PlayButtonSound ();								//Play click button sound.
		//This method will take a screenshot of game and it will add text with the url of the store where this game is published.
		StartCoroutine (NativeShare.ShareScreenshotWithText ("OMG! I have reached " + score.ToString () +
		" points in Frog Tap! Can you beat my score?"));
	}

	public void RateGame(){
		PlayButtonSound ();								//Play click button sound.
		NativeShare.Rate ();							//This method will send the user to the virtual store to rate this game.
	}

	// Enable or disable the sound effects and store the changes  
	public void ChangeSoundState(){
		//If sound effects are enabled...
		if (SFXOn) {
			SFXOn = false;								//...set sound effects as disabled.
		} else {
			//Otherwise...
			SFXOn = true;								//...set sound effects as enabled.
		}
		Utilities.SetBool ("SFXOn", SFXOn);				//Save the changes.
		UpdateSoundState ();							//Update sprites of the sound buttons and the audio sources states.
	}

	// Update sprites of the sound buttons and the audio sources states
	void UpdateSoundState(){
		//If sound effects are enabled...
		if (SFXOn) {
			//...update the sprite of each button as sound on.	
			UIGame.imgBtnSound.sprite = UIGame.spriteSoundOn;
			UIGame.imgBtnSoundPauseMenu.sprite = UIGame.spriteSoundOn;
			UIGame.imgBtnSoundGameOverMenu.sprite = UIGame.spriteSoundOn;
		}else {
			//Otherwise, update the sprite of each button as sound off.	
			UIGame.imgBtnSound.sprite = UIGame.spriteSoundOff;
			UIGame.imgBtnSoundPauseMenu.sprite = UIGame.spriteSoundOff;
			UIGame.imgBtnSoundGameOverMenu.sprite = UIGame.spriteSoundOff;
		}
		//Set each audio source as enabled or disabled according to "SFXOn" value.
		audioGame.generalAudioSource.enabled = SFXOn;
		audioGame.frogAudioSource.enabled = SFXOn;

		//If it's the first time this method is invoked, the SFX button can't played.
		if (playSfXInSoundStateMethod)
			PlayButtonSound ();							//Play click button sound.
		playSfXInSoundStateMethod = true;				//The SFX button can plays after the first time this method is invoked.
	}

	public void PlayButtonSound(){
		Utilities.PlaySFX (audioGame.generalAudioSource, audioGame.buttonSFX, 0.5f);		//...play click button sound.
	}

	// This method increase and display the new score
	public void IncreaseScore(){
		score++;
		UIGame.txtScore.text = score.ToString ();
	}

	// This method increase the flies counter and display it
	public void IncreaseFlies(int fliesToAdd){
		fliesCounter += fliesToAdd;
		UIGame.txtFlies.text = fliesCounter.ToString ();
		Utilities.PlaySFX (audioGame.generalAudioSource, audioGame.flySFX, 0.7f);		//Play "pick up fly" sound.
	}

	// This method is called when the user goes out the game.
	public void OnApplicationPause(bool pause){
		if(pause){
			//If the user was playing...
			if (UIGame.menuGame.activeSelf)
				PauseGame ();							//...pause the game.
		}
	}

	// The user press back button on mobile devices.
	// The user will move in the game depending what menus are activated.
	void OnBackButtonPressed(){
		//If credits menu is active...
		if (UIGame.menuCredits.activeSelf) {
			PlayButtonSound ();							//...play click button sound...
			UIGame.menuCredits.SetActive (false);		//...hide credits menu...
			UIGame.menuMain.SetActive (true);			//...and show main menu.
		}
		//Otherwise, if game over menu is active...
		else if (UIGame.menuGameOver.activeSelf) {
			//...and shop menu is active...
			if (UIGame.menuShop.activeSelf) {
				GoHomeFromGameOver ();					//...show main menu from game over menu.
			} else {
				//...otherwise, restart game.
				RestartGame ();
			}
		}
		//Otherwise, if shop menu is active...
		else if (UIGame.menuShop.activeSelf) {
			PlayButtonSound ();							//...play click button sound...
			UIGame.menuShop.SetActive (false);			//...hide shop menu...
			UIGame.menuMain.SetActive (true);			//...and show main menu.
		}
		//Otherwise, if game menu is active...
		else if (UIGame.menuGame.activeSelf) {
			//...and if pause menu is inactive... 
			if (!UIGame.menuPause.activeSelf) {
				PauseGame ();							//..pause the game.
			} else {
				//...Otherwise, unpause the game.
				ResumeGame ();
			}
		}
	}
}

[System.Serializable]
public class UIGame {
	[Header("Menus")]
	public GameObject menuMain;									//GameObject main menu.
	public GameObject menuCredits;								//GameObject credits menu.
	public GameObject menuShop;									//GameObject shop menu.
	public GameObject menuGame;									//GameObject game menu.
	public GameObject menuPause;								//GameObject pause menu.
	public GameObject menuGameOver;								//GameObject game over menu.
	public GameObject screenFade;								//GameObject fade screen.

	[Header("Texts")]
	public Text txtScore;										//Text to display the score in game menu.
	public Text txtFlies;										//Text to display the number of flies in game menu.
	public Text txtScoreGameOver;								//Text to display the score in game over menu.
	public Text txtBestScoreGameOver;							//Text to display the best score in game over menu.
	public Text txtFliesShop;									//Text to display the number of flies in shop menu.

	[Header("Buttons")]
	public Image imgBtnSound;									//Sound button image of the main menu.
	public Image imgBtnSoundPauseMenu;							//Sound button image of the pause menu.
	public Image imgBtnSoundGameOverMenu;						//Sound button image of the game over menu.
	public Sprite spriteSoundOn;								//Sprite sound on.
	public Sprite spriteSoundOff;								//Sprite sound off.
}

[System.Serializable]
public class AudioGame{
	[Header("Audio Sources")]
	public AudioSource generalAudioSource;						//General audio source component.
	public AudioSource frogAudioSource;							//Frog audio source component.

	[Header("Audio Clips")]
	public AudioClip buttonSFX;									//Button sound.
	public AudioClip noEnoughFliesSFX;							//No enough flies sound.
	public AudioClip flySFX;									//Sound used when the character picks up a fly.
}

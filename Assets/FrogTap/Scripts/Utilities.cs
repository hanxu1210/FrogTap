using UnityEngine;
using System.Collections;

public static class Utilities {

	// Method used to play a sound effect
	public static void PlaySFX(AudioSource source, AudioClip clip, float volume = 1.0f, bool loop = false){
		//If sound effects are not enabled, do nothig.
		if (!GameController.instance.SFXOn)
			return;

		if (source){
			source.clip = clip;
			source.volume = volume;
			source.loop = loop;
			source.Play ();
		}
	}

	// PlayerPrefs bool, source http://wiki.unity3d.com/index.php/BoolPrefs
	public static void SetBool(string key, bool value){
		PlayerPrefs.SetInt (key, value ? 1 : 0);
	}

	public static bool GetBool(string key){
		return PlayerPrefs.GetInt (key) == 1 ? true : false;
	}

	public static bool GetBool(string key, bool defaultValue){
		if(PlayerPrefs.HasKey(key)){
			return GetBool (key);	
		}
		return defaultValue;
	}

	public static void DeletePlayerPrefs(){
		PlayerPrefs.DeleteAll ();
	}
}

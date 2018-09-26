using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
#if UNITY_METRO || UNITY_WP_8_1
using WP81Plugin;
#endif

// Based on: https://github.com/ChrisMaire/unity-native-sharing
public class NativeShare : MonoBehaviour{

	//Public variables
	public static string _applicationAndroidID = "com.ARG.TheScape";
	public static string _applicationIOSID = "1057997453";
	public static string _applicationWindowsPhoneID ="9nblggh625b6";

	//Private variables
	private static string _screenshotName = "Screenshot.png";

	public static IEnumerator ShareScreenshotWithText(string text)
    {
		#if UNITY_METRO || UNITY_WP_8_1
		string screenShotPath = ServicesNativeWP81.PathLocalFolderWP81(_screenshotName);
		Application.CaptureScreenshot(screenShotPath);
		#else
		string screenShotPath = Application.persistentDataPath + "/" + _screenshotName;
		Application.CaptureScreenshot(_screenshotName);
		#endif
		
		yield return 0;

		text += "\n" + GetURLStore();
		Share(text,screenShotPath,"");
    }

	static void Share(string shareText, string imagePath, string url, string subject = "Share")
	{
	#if UNITY_EDITOR
		Debug.Log("To use the share method, build the game for android, IOS or windows phone 8.1 platform.");
	#elif UNITY_ANDROID
		AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
		AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
		
		intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
		AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
		AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + imagePath);
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
		intentObject.Call<AndroidJavaObject>("setType", "image/png");
		
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareText);
		
		AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
		
		AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, subject);
		currentActivity.Call("startActivity", jChooser);
	#elif UNITY_IOS
		CallSocialShareAdvanced(shareText, subject, url, imagePath);
	#elif UNITY_METRO || UNITY_WP_8_1
		ServicesNativeWP81.Share(" ",shareText);
	#endif
	}

	public static void Rate()
	{
	#if UNITY_ANDROID
		Application.OpenURL("market://details?id=" + _applicationAndroidID);
	#elif UNITY_IOS
		Application.OpenURL("itms-apps://itunes.apple.com/app/id"+ _applicationIOSID); 
	#elif UNITY_METRO || UNITY_WP_8_1
		ServicesNativeWP81.RateGame();
	#else
		Debug.Log("No rate set up for this platform.");
	#endif
	}

	static string GetURLStore(){
		string URLStore = "";
	#if UNITY_ANDROID
		URLStore = "https://play.google.com/store/apps/details?id=" + _applicationAndroidID;
	#elif UNITY_IOS
		URLStore = "https://itunes.apple.com/app/id" + _applicationIOSID;
	#elif UNITY_METRO || UNITY_WP_8_1
		URLStore = "https://www.microsoft.com/store/apps/"+_applicationWindowsPhoneID;
	#endif
		return URLStore;
	}

#if UNITY_IOS
	public struct ConfigStruct
	{
		public string title;
		public string message;
	}

	[DllImport ("__Internal")] private static extern void showAlertMessage(ref ConfigStruct conf);
	
	public struct SocialSharingStruct
	{
		public string text;
		public string url;
		public string image;
		public string subject;
	}
	
	[DllImport ("__Internal")] private static extern void showSocialSharing(ref SocialSharingStruct conf);
	
	public static void CallSocialShare(string title, string message)
	{
		ConfigStruct conf = new ConfigStruct();
		conf.title  = title;
		conf.message = message;
		showAlertMessage(ref conf);
	}

	public static void CallSocialShareAdvanced(string defaultTxt, string subject, string url, string img)
	{
		SocialSharingStruct conf = new SocialSharingStruct();
		conf.text = defaultTxt; 
		conf.url = url;
		conf.image = img;
		conf.subject = subject;
		
		showSocialSharing(ref conf);
	}
#endif
}

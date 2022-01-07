/*****************************************************************************************************************
 - Toast.cs - 
-----------------------------------------------------------------------------------------------------------------
 Script Type:       Utilities
 Author(s):         Victor Dang
 Game Name:         
 Engine Version:    
-----------------------------------------------------------------------------------------------------------------
 Description: 
     Use this class to display a toast message (Android only) at the bottom of the screen. For those who are 
	 unfamiliar with the toast notifications, it is the gray message bubble that appears at the bottom of the 
	 screen.
	 
	 NOTES:
	 - The current implementation is inefficient, use the class sparingly
*****************************************************************************************************************/

using UnityEngine;

public class Toast
{
	static AndroidJavaObject currentActivity;
	static AndroidJavaClass toastClass;
	static AndroidJavaClass unityPlayer;
		
		
	#region Toasts

	/// <summary>
	/// Displays a toast notification on the bottom of the screen.
	/// Reference Material: https://medium.com/@agrawalsuneet/native-android-in-unity-8ebfb42edfe8
	/// </summary>
	/// <param name="message"></param>
	public static void Show(string message)
	{
		#if UNITY_EDITOR
			Debug.Log(message);
		#elif UNITY_ANDROID
			if (toastClass == null)
				toastClass = new AndroidJavaClass("android.widget.Toast");
				
			if (unityPlayer == null)
				unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				
			if (currentActivity == null)
				currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			
			toastClass.CallStatic<AndroidJavaObject>("makeText", currentActivity, message, toastClass.GetStatic<int>("LENGTH_SHORT")).Call("show");
		#endif
	}

	#endregion

}
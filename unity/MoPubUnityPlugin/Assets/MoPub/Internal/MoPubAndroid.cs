using UnityEngine;
using System.Collections.Generic;


#if UNITY_ANDROID

//public enum MoPubAdPosition
//{
//	TopLeft,
//	TopCenter,
//	TopRight,
//	Centered,
//	BottomLeft,
//	BottomCenter,
//	BottomRight
//}


public enum MoPubLocationAwareness
{
	TRUNCATED,
	DISABLED,
	NORMAL
}


public class MoPubAndroid
{
	private static readonly AndroidJavaClass _pluginClass = new AndroidJavaClass ("com.mopub.unity.MoPubUnityPlugin");
	private readonly AndroidJavaObject _plugin;

	public MoPubAndroid (string adUnitId)
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		_plugin = new AndroidJavaObject ("com.mopub.unity.MoPubUnityPlugin", adUnitId);
	}


	public static void addFacebookTestDeviceId (string hashedDeviceId)
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		_pluginClass.CallStatic ("addFacebookTestDeviceId", hashedDeviceId);
	}
		

	// Enables/disables location support for banners and interstitials
	public static void setLocationAwareness (MoPubLocationAwareness locationAwareness)
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		_pluginClass.CallStatic ("setLocationAwareness", locationAwareness.ToString ());
	}


//	// Creates a banner of the given type at the given position
//	public void createBanner (MoPubAdPosition position)
//	{
//		if (Application.platform != RuntimePlatform.Android)
//			return;
//
//		_plugin.Call ("createBanner", (int)position);
//	}
//
//
//	// Destroys the banner and removes it from view
//	public void destroyBanner ()
//	{
//		if (Application.platform != RuntimePlatform.Android)
//			return;
//
//		_plugin.Call ("destroyBanner");
//	}
//
//
//	// Shows/hides the banner
//	public void showBanner (bool shouldShow)
//	{
//		if (Application.platform != RuntimePlatform.Android)
//			return;
//
//		_plugin.Call ("hideBanner", !shouldShow);
//	}
//
//
//	// Sets the keywords for the current banner
//	public void setBannerKeywords (string keywords)
//	{
//		if (Application.platform != RuntimePlatform.Android)
//			return;
//
//		_plugin.Call ("setBannerKeywords", keywords);
//	}


//	// Starts loading an interstitial ad
//	public void requestInterstitialAd (string keywords = "")
//	{
//		if (Application.platform != RuntimePlatform.Android)
//			return;
//
//		_plugin.Call ("requestInterstitialAd", keywords);
//	}
//
//
//	// If an interstitial ad is loaded this will take over the screen and show the ad
//	public void showInterstitialAd ()
//	{
//		if (Application.platform != RuntimePlatform.Android)
//			return;
//
//		_plugin.Call ("showInterstitialAd");
//	}


	// Reports an app download to MoPub
	public static void reportApplicationOpen ()
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		_pluginClass.CallStatic ("reportApplicationOpen");
	}


	// Initializes the rewarded video system
	public static void initializeRewardedVideo ()
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		_pluginClass.CallStatic ("initializeRewardedVideo");
	}


	// Starts loading a rewarded video ad
	public void requestRewardedVideo (List<MoPubMediationSetting> mediationSettings = null, 
	                                  string keywords = null, 
	                                  double latitude = MoPub.LAT_LONG_SENTINEL, 
	                                  double longitude = MoPub.LAT_LONG_SENTINEL, 
	                                  string customerId = null)
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		var json = mediationSettings == null ? null : MoPubInternal.ThirdParty.MiniJSON.Json.Serialize (mediationSettings);
		_plugin.Call ("requestRewardedVideo", json, keywords, latitude, longitude, customerId);
	}


	// If a rewarded video ad is loaded this will take over the screen and show the ad
	public void showRewardedVideo ()
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		_plugin.Call ("showRewardedVideo");
	}
}

#endif
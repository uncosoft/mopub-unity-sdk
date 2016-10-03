using UnityEngine;
using System.Collections.Generic;


#if UNITY_ANDROID

public class MoPubAndroidRewardedVideo
{
	private static readonly AndroidJavaClass _pluginClass =
		new AndroidJavaClass ("com.mopub.unity.MoPubRewardedVideoUnityPlugin");
	private readonly AndroidJavaObject _plugin;

	public MoPubAndroidRewardedVideo (string adUnitId)
	{
		if (Application.platform != RuntimePlatform.Android)
			return;

		_plugin = new AndroidJavaObject ("com.mopub.unity.MoPubRewardedVideoUnityPlugin", adUnitId);
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

		var json = (mediationSettings == null) ?
			null :
			MoPubInternal.ThirdParty.MiniJSON.Json.Serialize (mediationSettings);
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
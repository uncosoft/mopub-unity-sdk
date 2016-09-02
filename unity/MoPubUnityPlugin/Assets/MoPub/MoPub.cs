using UnityEngine;
using System;
using System.Collections.Generic;


#if UNITY_IPHONE || UNITY_ANDROID

#if UNITY_IPHONE
using MP = MoPubBinding;
#elif UNITY_ANDROID
	using MP = MoPubAndroid;
#endif


public class MoPubMediationSetting : Dictionary<string,object>
{
	public MoPubMediationSetting (string adVendor)
	{
		this.Add ("adVendor", adVendor);
	}
}


public static class MoPub
{
	public const double LAT_LONG_SENTINEL = 99999.0;
	public const string ADUNIT_NOT_FOUND_MSG = "AdUnit {0} not found: no plugin was initialized";

	private static Dictionary<string, MP> _pluginsDict = new Dictionary<string, MP> ();


	// Construct a plugin for each adUnit
	public static void loadPluginsForAdUnits (string[] adUnitIds)
	{
		Debug.Log (adUnitIds.Length + " AdUnits loaded for plugins:\n" + string.Join (", ", adUnitIds));
		foreach (string adUnitId in adUnitIds) {
			_pluginsDict.Add (adUnitId, new MP (adUnitId));
		}
	}


	// Enables/disables location support for banners and interstitials
	public static void enableLocationSupport (bool shouldUseLocation)
	{
#if UNITY_IPHONE
		MoPubBinding.enableLocationSupport (true);
#elif UNITY_ANDROID
		MoPubAndroid.setLocationAwareness (MoPubLocationAwareness.NORMAL);
#endif
	}


#if UNITY_IPHONE
	public static void createBanner (string adUnitId, MoPubAdPosition position, MoPubBannerType bannerType = MoPubBannerType.Size320x50)
	{
		MP plugin;
		if (_pluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.createBanner (bannerType, position);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}
#elif UNITY_ANDROID
	public static void createBanner (string adUnitId, MoPubAdPosition position)
	{
		MP plugin;
		if (_pluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.createBanner (position);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}
#endif


	// Destroys the banner and removes it from view
	public static void destroyBanner (string adUnitId)
	{
		MP plugin;
		if (_pluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.destroyBanner ();
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}


	// Shows/hides the banner
	public static void showBanner (string adUnitId, bool shouldShow)
	{
		MP plugin;
		if (_pluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.showBanner (shouldShow);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}


	// Starts loading an interstitial ad
	public static void requestInterstitialAd (string adUnitId, string keywords = "")
	{
		MP plugin;
		if (_pluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.requestInterstitialAd (keywords);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}


	// If an interstitial ad is loaded this will take over the screen and show the ad
	public static void showInterstitialAd (string adUnitId)
	{
		MP plugin;
		if (_pluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.showInterstitialAd ();
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}


	// Reports an app download to MoPub. iTunesAppId is iOS only.
	public static void reportApplicationOpen (string iTunesAppId = null)
	{
#if UNITY_IPHONE
		MoPubBinding.reportApplicationOpen (iTunesAppId);
#elif UNITY_ANDROID
		MoPubAndroid.reportApplicationOpen ();
#endif
	}


	// Initializes the rewarded video system
	public static void initializeRewardedVideo ()
	{
		MP.initializeRewardedVideo ();
	}


	// Starts loading a rewarded video ad
	public static void requestRewardedVideo (string adUnitId,
	                                         List<MoPubMediationSetting> mediationSettings = null,
	                                         string keywords = null,
	                                         double latitude = LAT_LONG_SENTINEL,
	                                         double longitude = LAT_LONG_SENTINEL,
	                                         string customerId = null)
	{
		MP plugin;
		if (_pluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.requestRewardedVideo (mediationSettings, keywords, latitude, longitude, customerId);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}


	// If a rewarded video ad is loaded this will take over the screen and show the ad
	public static void showRewardedVideo (string adUnitId)
	{
		MP plugin;
		if (_pluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.showRewardedVideo ();
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}
}

#endif
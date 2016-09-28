using UnityEngine;
using System;
using System.Collections.Generic;


#if UNITY_IPHONE || UNITY_ANDROID

#if UNITY_IPHONE
using MP = MoPubBinding;
#elif UNITY_ANDROID
using MP = MoPubAndroid;
using MPBanner = MoPubAndroidBanner;
using MPInterstitial = MoPubAndroidInterstitial;
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

#if UNITY_ANDROID
	private static Dictionary<string, MPBanner> _bannerPluginsDict = new Dictionary<string, MPBanner> ();	
	private static Dictionary<string, MPInterstitial> _interstitialPluginsDict = new Dictionary<string, MPInterstitial> ();	

	public static void loadBannerPluginsForAdUnits (string[] bannerAdUnitIds)
	{
		Debug.Log (bannerAdUnitIds.Length + " banner AdUnits loaded for plugins:\n" + string.Join (", ", bannerAdUnitIds));
		foreach (string bannerAdUnitId in bannerAdUnitIds) {
			_bannerPluginsDict.Add (bannerAdUnitId, new MPBanner (bannerAdUnitId));			
		}
	}

	public static void loadInterstitialPluginsForAdUnits (string[] interstitialAdUnitIds)
	{
		Debug.Log (interstitialAdUnitIds.Length + " interstitial AdUnits loaded for plugins:\n" + string.Join (", ", interstitialAdUnitIds));
		foreach (string interstitialAdUnitId in interstitialAdUnitIds) {
			_interstitialPluginsDict.Add (interstitialAdUnitId, new MPInterstitial (interstitialAdUnitId));			
		}
	}


#endif
	

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
		MPBanner plugin;
		if (_bannerPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.createBanner (position);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
	}
#endif


	// Destroys the banner and removes it from view
	public static void destroyBanner (string adUnitId)
	{
#if UNITY_IPHONE
		MP plugin;
		if (_pluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.destroyBanner ();
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
#elif UNITY_ANDROID
		MPBanner plugin;
		if (_bannerPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.destroyBanner ();
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
#endif		
	}


	// Shows/hides the banner
	public static void showBanner (string adUnitId, bool shouldShow)
	{
#if UNITY_IPHONE
		MP plugin;
		if (_pluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.showBanner (shouldShow);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
#elif UNITY_ANDROID
		MPBanner plugin;
		if (_bannerPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.showBanner (shouldShow);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
#endif		
	}


	// Starts loading an interstitial ad
	public static void requestInterstitialAd (string adUnitId, string keywords = "")
	{
#if UNITY_IPHONE
		MP plugin;
		if (_pluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.requestInterstitialAd (keywords);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
#elif UNITY_ANDROID
		MPInterstitial plugin;
		if (_interstitialPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.requestInterstitialAd (keywords);
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
#endif
	}


	// If an interstitial ad is loaded this will take over the screen and show the ad
	public static void showInterstitialAd (string adUnitId)
	{
#if UNITY_IPHONE
		MP plugin;
		if (_pluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.showInterstitialAd ();
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
#elif UNITY_ANDROID
		MPInterstitial plugin;
		if (_interstitialPluginsDict.TryGetValue (adUnitId, out plugin)) {
			plugin.showInterstitialAd ();
		} else {
			Debug.LogWarning (String.Format (ADUNIT_NOT_FOUND_MSG, adUnitId));
		}
#endif
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
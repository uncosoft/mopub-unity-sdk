using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;



#if UNITY_IPHONE

public enum MoPubBannerType
{
	Size320x50,
	Size300x250,
	Size728x90,
	Size160x600
}


public enum MoPubAdPosition
{
	TopLeft,
	TopCenter,
	TopRight,
	Centered,
	BottomLeft,
	BottomCenter,
	BottomRight
}



public class MoPubBinding
{
	private string adUnitId;

	public MoPubBinding (string adUnitId)
	{
		this.adUnitId = adUnitId;
	}

	[DllImport ("__Internal")]
	private static extern void _moPubEnableLocationSupport (bool shouldUseLocation);

	// Enables/disables location support for banners and interstitials
	public static void enableLocationSupport (bool shouldUseLocation)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			_moPubEnableLocationSupport (shouldUseLocation);
	}


	[DllImport ("__Internal")]
	private static extern void _moPubCreateBanner (int bannerType, int position, string adUnitId);

	// Creates a banner of the given type at the given position
	public void createBanner (MoPubBannerType bannerType, MoPubAdPosition position)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			_moPubCreateBanner ((int)bannerType, (int)position, adUnitId);
	}


	[DllImport ("__Internal")]
	private static extern void _moPubDestroyBanner (string adUnitId);

	// Destroys the banner and removes it from view
	public void destroyBanner ()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			_moPubDestroyBanner (adUnitId);
	}


	[DllImport ("__Internal")]
	private static extern void _moPubShowBanner (string adUnitId, bool shouldShow);

	// Shows/hides the banner
	public void showBanner (bool shouldShow)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			_moPubShowBanner (adUnitId, shouldShow);
	}


	[DllImport ("__Internal")]
	private static extern void _moPubRefreshAd (string adUnitId, string keywords);

	// Refreshes the ad banner with optional keywords
	public void refreshAd (string keywords)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			_moPubRefreshAd (adUnitId, keywords);
	}


	[DllImport ("__Internal")]
	private static extern void _moPubRequestInterstitialAd (string adUnitId, string keywords);

	// Starts loading an interstitial ad
	public void requestInterstitialAd (string keywords = "")
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			_moPubRequestInterstitialAd (adUnitId, keywords);
	}


	[DllImport ("__Internal")]
	private static extern void _moPubShowInterstitialAd (string adUnitId);

	// If an interstitial ad is loaded this will take over the screen and show the ad
	public void showInterstitialAd ()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			_moPubShowInterstitialAd (adUnitId);
	}


	[DllImport ("__Internal")]
	private static extern void _moPubReportApplicationOpen (string iTunesAppId);

	// Reports an app download to MoPub
	public static void reportApplicationOpen (string iTunesAppId)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			_moPubReportApplicationOpen (iTunesAppId);
	}


	[DllImport ("__Internal")]
	private static extern void _moPubInitializeRewardedVideo ();

	// Initializes the rewarded video system
	public static void initializeRewardedVideo ()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			_moPubInitializeRewardedVideo ();
	}


	[DllImport ("__Internal")]
	private static extern void _moPubRequestRewardedVideo (string adUnitId, string json, string keywords, double latitude, double longitude, string customerId);

	// Starts loading a rewarded video ad
	public void requestRewardedVideo (List<MoPubMediationSetting> mediationSettings = null, string keywords = null,
	                                  double latitude = MoPub.LAT_LONG_SENTINEL, double longitude = MoPub.LAT_LONG_SENTINEL, string customerId = null)
	{
		var json = mediationSettings == null ? null : MoPubInternal.ThirdParty.MiniJSON.Json.Serialize (mediationSettings);
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			_moPubRequestRewardedVideo (adUnitId, json, keywords, latitude, longitude, customerId);
	}


	[DllImport ("__Internal")]
	private static extern void _moPubShowRewardedVideo (string adUnitId);

	// If a rewarded video ad is loaded this will take over the screen and show the ad
	public void showRewardedVideo ()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
			_moPubShowRewardedVideo (adUnitId);
	}
}
#endif
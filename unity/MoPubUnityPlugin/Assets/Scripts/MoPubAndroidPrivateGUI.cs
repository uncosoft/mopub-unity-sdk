using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class MoPubAndroidPrivateGUI : MonoBehaviour
{
	#if UNITY_ANDROID
	private int _selectedToggleIndex;
	private string[] _bannerAdUnits;
	private string[] _interstitialAdUnits;
	private string[] _rewardedVideoAdUnits;

	private string[] _networkList = new string[] {
		"MoPub",
		"Millennial",
		"AdMob",
		"Chartboost",
		"Vungle",
		"Facebook",
		"AdColony",
		"Unity Ads"
	};

	private Dictionary<string, string[]> _bannerDict = new Dictionary<string, string[]> () {				
		{ "AdMob", new string[] { "173f4589c04a43b1b2e2e49d05f58e80" } },		
		{ "Facebook", new string[] { "b40a96dd275e4ce5be2cdf5faa92007d" } },
		{ "Millennial", new string[] { "1aa442709c9f11e281c11231392559e4" } },
		{ "MoPub", new string[] { "23b49916add211e281c11231392559e4" } },		
	};

	private Dictionary<string, string[]> _interstitialDict = new Dictionary<string, string[]> () {
		{ "AdColony", new string[] { "3aa79f11389540db8e250a80e4d16a46" } },
		{ "AdMob", new string[] { "554e8baff8d84137941b5a55354105fc" } },
		{ "Chartboost", new string[] { "376366b49d324dedae3d5edb360c27b4" } },
		{ "Facebook", new string[] { "9792d876011f4359887d2d26380e8a84" } },
		{ "Millennial", new string[] { "c6566f7bd85c40afb7afc4232a1cd463" } },
		{ "MoPub", new string[] { "3aba0056add211e281c11231392559e4" } },
		{ "Unity Ads", new string[] { "079f9caa99eb429588c2c3633e1ce3e3" } },
		{ "Vungle", new string[] { "4f5e1e97f87c406cb7878b9eff1d2a77" } }
	};

	private Dictionary<string, string[]> _rewardedVideoDict = new Dictionary<string, string[]> () {
		{ "AdColony", new string[] { "e258c916e659447d9d98256a3ab2979e" } },		
		{ "Chartboost", new string[] { "df605ab15b56400285c99e521ecc2cb1" } },		
		{ "MoPub", new string[] { "db2ef0eb1600433a8cdc31c75549c6b1" } },
		{ "Unity Ads", new string[] { "4302e96be4584fa6b653a0668a845407" } },
		{ "Vungle", new string[] { "2d38f4e6881341369e9fc2c2d01ddc9d" } }
	};


	static bool IsNullOrEmpty (string[] strArray)
	{
		return (strArray == null || strArray.Length == 0);
	}


	void Start ()
	{
		var allAdUnits = new string[0];

		foreach (var bannerAdUnits in _bannerDict.Values) {
			allAdUnits = allAdUnits.Union (bannerAdUnits).ToArray ();
		}

		foreach (var interstitialAdUnits in _interstitialDict.Values) {
			allAdUnits = allAdUnits.Union (interstitialAdUnits).ToArray ();
		}

		foreach (var rewardedVideoAdUnits in _rewardedVideoDict.Values) {
			allAdUnits = allAdUnits.Union (rewardedVideoAdUnits).ToArray ();
		}

		MoPub.loadPluginsForAdUnits (allAdUnits);
		MoPub.initializeRewardedVideo ();
	}


	void OnGUI ()
	{
		GUI.skin.button.margin = new RectOffset (0, 0, 10, 0);
		GUI.skin.button.stretchWidth = true;
		GUI.skin.button.fixedHeight = (Screen.width >= 960 || Screen.height >= 960) ? 100 : 50;

		var halfWidth = Screen.width / 2;
		GUILayout.BeginArea (new Rect (0, 0, halfWidth, Screen.height));
		GUILayout.BeginVertical ();

//		if (GUILayout.Button ("Create Banner (bottom right)")) {
//			if (_bannerAdUnit == "") {
//				Debug.LogWarning ("No banner ad unit ID is available for the currently selected platform");
//				return;
//			}
//			MoPub.createBanner (_bannerAdUnit, MoPubAdPosition.BottomRight);
//		}
//
//
//		if (GUILayout.Button ("Destroy Banner")) {
//			MoPub.destroyBanner (_bannerAdUnit);
//		}
//
//
//		GUILayout.BeginHorizontal ();
//		if (GUILayout.Button ("Show Banner")) {
//			MoPub.showBanner (_bannerAdUnit, true);
//		}
//
//
//		if (GUILayout.Button ("Hide Banner")) {
//			MoPub.showBanner (_bannerAdUnit, false);
//		}
//		GUILayout.EndHorizontal ();
//
//
//		GUILayout.Space (20);
//		if (GUILayout.Button ("Request Interstitial")) {
//			MoPub.requestInterstitialAd (_interstitialAdUnit);
//			Debug.Log ("requesting interstitial with ad unit: " + _interstitialAdUnit);
//		}
//
//
//		if (GUILayout.Button ("Show Interstitial")) {
//			MoPub.showInterstitialAd (_interstitialAdUnit);
//		}
//

		GUILayout.EndVertical ();
		GUILayout.EndArea ();

		GUILayout.BeginArea (new Rect (Screen.width - halfWidth, 0, halfWidth, Screen.height));
		GUILayout.BeginVertical ();


		if (GUILayout.Button ("Report App Open")) {
			MoPub.reportApplicationOpen ();
		}


		if (GUILayout.Button ("Enable Location Support")) {
			MoPub.enableLocationSupport (true);
		}


		// no need to show the rewarded ad buttons if this network doesnt have them
		GUILayout.Space (20);
		if (!IsNullOrEmpty (_rewardedVideoAdUnits)) {
			var adColonySettings = new MoPubMediationSetting ("AdColony");
			adColonySettings.Add ("withConfirmationDialog", true);
			adColonySettings.Add ("withResultsDialog", true);

			var chartboostSettings = new MoPubMediationSetting ("Chartboost");
			chartboostSettings.Add ("customId", "the-user-id");

			var vungleSettings = new MoPubMediationSetting ("Vungle");
			vungleSettings.Add ("userId", "the-user-id");
			vungleSettings.Add ("cancelDialogBody", "Cancel Body");
			vungleSettings.Add ("cancelDialogCloseButton", "Shut it Down");
			vungleSettings.Add ("cancelDialogKeepWatchingButton", "Watch On");
			vungleSettings.Add ("cancelDialogTitle", "Cancel Title");

			var mediationSettings = new List<MoPubMediationSetting> ();
			mediationSettings.Add (adColonySettings);
			mediationSettings.Add (chartboostSettings);
			mediationSettings.Add (vungleSettings);

			foreach (string rewardedVideoAdUnit in _rewardedVideoAdUnits) {
				if (GUILayout.Button ("Request Rewarded Video: " + rewardedVideoAdUnit)) {				
					MoPub.requestRewardedVideo (rewardedVideoAdUnit, mediationSettings, "rewarded, video, mopub", 37.7833, 122.4167, "customer101");
					Debug.Log ("requesting rewarded video with AdUnit: " + rewardedVideoAdUnit + " and mediation settings: " + MoPubInternal.ThirdParty.MiniJSON.Json.Serialize (mediationSettings));
				}
							
				if (GUILayout.Button ("Show Rewarded Video: " + rewardedVideoAdUnit)) {
					MoPub.showRewardedVideo (rewardedVideoAdUnit);
				}
			}
		} else {
			GUILayout.Label ("No rewarded video ad unit for this network");
		}

		GUILayout.EndVertical ();
		GUILayout.EndArea ();


//		GUI.changed = true;
		_selectedToggleIndex = GUI.Toolbar (new Rect (0, Screen.height - GUI.skin.button.fixedHeight, Screen.width, GUI.skin.button.fixedHeight), _selectedToggleIndex, _networkList);
//		if (GUI.changed) {
			string network = _networkList [_selectedToggleIndex];

			_bannerAdUnits = _bannerDict.ContainsKey (network) ? _bannerDict [network] : null;
			_interstitialAdUnits = _interstitialDict.ContainsKey (network) ? _interstitialDict [network] : null;
			_rewardedVideoAdUnits = _rewardedVideoDict.ContainsKey (network) ? _rewardedVideoDict [network] : null;
//		}
	}		
	#endif
}

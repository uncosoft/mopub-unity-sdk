using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class MoPubAndroidDemoGUI : MonoBehaviour
{
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

	#if UNITY_ANDROID

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


	#elif UNITY_IPHONE

	private Dictionary<string, string[]> _bannerDict = new Dictionary<string, string[]> () {
		{ "AdMob", new string[] { "d2b8a9fcd92440e79c7437d2b51f25a6" } },
		{ "Facebook", new string[] { "fb759131fd7a40e6b9d324e637a4b299" } },
		{ "Millennial", new string[] { "1b282680106246aa83036892b32ec7cc" } },
		{ "MoPub", new string[] { "23b49916add211e281c11231392559e4" } },
	};

	private Dictionary<string, string[]> _interstitialDict = new Dictionary<string, string[]> () {
		{ "AdColony", new string[] { "09fed773d1e34cba968d910b4fbdc850" } },
		{ "AdMob", new string[] { "4f9d8fb8521f4420b2429184f720f42b" } },
		{ "Chartboost", new string[] { "a97fa010d9c24d06ae267be2a1487af1" } },
		{ "Facebook", new string[] { "27614fde27df488493327f2b952f9d21" } },
		{ "Millennial", new string[] { "0da9e2762f1a48bab695887fb7798b66" } },
		{ "MoPub", new string[] { "3aba0056add211e281c11231392559e4" } },
		{ "Unity Ads", new string[] { "676a0fa97aca48cbbe489de5b2fa4cd1" } },
		{ "Vungle", new string[] { "c87b1701e1084507bf8be89cd13b890c" } }
	};

	private Dictionary<string, string[]> _rewardedVideoDict = new Dictionary<string, string[]> () {
		{ "AdColony", new string[] { "52aa460767374250a5aa5174c2345be3" } },
		{ "Chartboost", new string[] { "2942576082c24e0f80c6172703572870" } },
		{ "MoPub", new string[] { "fdd35fb5d55b4ccf9ceb27c7a3926b7d" } },
		{ "Unity Ads", new string[] { "4302e96be4584fa6b653a0668a845407" } },
		{ "Vungle", new string[] { "19a24d282ecb49c5bb43c65f501e33bf" } }
	};

	#endif

	static bool IsAdUnitArrayNullOrEmpty (string[] adUnitArray)
	{
		return (adUnitArray == null || adUnitArray.Length == 0);
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
		// Set label text font size
		var headerStyle = GUI.skin.GetStyle ("label");
		headerStyle.fontSize = 30;

		// Set button text font size
		var buttonStyle = GUI.skin.GetStyle ("button");
		buttonStyle.fontSize = 20;

		// Set button margins and section margins
		GUI.skin.button.margin = new RectOffset (0, 0, 10, 0);
		GUI.skin.button.stretchWidth = true;
		GUI.skin.button.fixedHeight = (Screen.width >= 960 || Screen.height >= 960) ? 100 : 50;
		var sectionMargin = 40;

		// Tabs for networks
		_selectedToggleIndex = GUI.Toolbar (new Rect (0, Screen.height - GUI.skin.button.fixedHeight, Screen.width, GUI.skin.button.fixedHeight), _selectedToggleIndex, _networkList);
		string network = _networkList [_selectedToggleIndex];
		_bannerAdUnits = _bannerDict.ContainsKey (network) ? _bannerDict [network] : null;
		_interstitialAdUnits = _interstitialDict.ContainsKey (network) ? _interstitialDict [network] : null;
		_rewardedVideoAdUnits = _rewardedVideoDict.ContainsKey (network) ? _rewardedVideoDict [network] : null;


		GUILayout.BeginArea (new Rect (0, 0, Screen.width, Screen.height));
		GUILayout.BeginVertical ();


		// Banner AdUnits
		GUILayout.Space (sectionMargin);
		GUILayout.Label ("Banners:");
		if (!IsAdUnitArrayNullOrEmpty (_bannerAdUnits)) {
			foreach (string bannerAdUnit in _bannerAdUnits) {
				GUILayout.BeginHorizontal ();

				if (GUILayout.Button ("Create Banner: " + bannerAdUnit)) {
					MoPub.createBanner (bannerAdUnit, MoPubAdPosition.BottomRight);
					Debug.Log ("requesting banner with AdUnit: " + bannerAdUnit);
				}

				if (GUILayout.Button ("Destroy Banner")) {
					MoPub.destroyBanner (bannerAdUnit);
				}

				if (GUILayout.Button ("Show Banner")) {
					MoPub.showBanner (bannerAdUnit, true);
				}

				if (GUILayout.Button ("Hide Banner")) {
					MoPub.showBanner (bannerAdUnit, false);
				}

				GUILayout.EndHorizontal ();
			}
		} else {
			GUILayout.Label ("No banner AdUnits for " + network);
		}


		// Interstitial AdUnits
		GUILayout.Space (sectionMargin);
		GUILayout.Label ("Interstitials:");
		if (!IsAdUnitArrayNullOrEmpty (_interstitialAdUnits)) {
			foreach (string interstitialAdUnit in _interstitialAdUnits) {
				GUILayout.BeginHorizontal ();

				if (GUILayout.Button ("Request Interstitial: " + interstitialAdUnit)) {
					MoPub.requestInterstitialAd (interstitialAdUnit);
					Debug.Log ("requesting interstitial with AdUnit: " + interstitialAdUnit);
				}

				if (GUILayout.Button ("Show Interstitial")) {
					MoPub.showInterstitialAd (interstitialAdUnit);
				}

				GUILayout.EndHorizontal ();
			}
		} else {
			GUILayout.Label ("No interstitial AdUnits for " + network);
		}


		// Rewarded Video AdUnits
		GUILayout.Space (sectionMargin);
		GUILayout.Label ("Rewarded Videos:");
		if (!IsAdUnitArrayNullOrEmpty (_rewardedVideoAdUnits)) {
			// Set up mediation settings
#if UNITY_ANDROID
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
#else
			var adColonySettings = new MoPubMediationSetting( "AdColony" );
			adColonySettings.Add( "showPrePopup", true );
			adColonySettings.Add( "showPostPopup", true );

			var vungleSettings = new MoPubMediationSetting( "Vungle" );
			vungleSettings.Add( "userIdentifier", "the-user-id" );

			var mediationSettings = new List<MoPubMediationSetting>();
			mediationSettings.Add( adColonySettings );
			mediationSettings.Add( vungleSettings );
#endif

			foreach (string rewardedVideoAdUnit in _rewardedVideoAdUnits) {
				GUILayout.BeginHorizontal ();

				if (GUILayout.Button ("Request Rewarded Video: " + rewardedVideoAdUnit)) {
					MoPub.requestRewardedVideo (rewardedVideoAdUnit, mediationSettings, "rewarded, video, mopub", 37.7833, 122.4167, "customer101");
					Debug.Log ("requesting rewarded video with AdUnit: " + rewardedVideoAdUnit + " and mediation settings: " + MoPubInternal.ThirdParty.MiniJSON.Json.Serialize (mediationSettings));
				}

				if (GUILayout.Button ("Show Rewarded Video")) {
					MoPub.showRewardedVideo (rewardedVideoAdUnit);
				}

				GUILayout.EndHorizontal ();
			}
		} else {
			GUILayout.Label ("No rewarded video AdUnits for " + network);
		}


		// Report App Open
		GUILayout.Space (sectionMargin);
		GUILayout.Label ("Report App Open:");
		if (GUILayout.Button ("Report App Open")) {
			MoPub.reportApplicationOpen ();
		}


		// Enable Location Support
		GUILayout.Space (sectionMargin);
		GUILayout.Label ("Enable Location Support:");
		if (GUILayout.Button ("Enable Location Support")) {
			MoPub.enableLocationSupport (true);
		}


		GUILayout.EndVertical ();
		GUILayout.EndArea ();
	}
}

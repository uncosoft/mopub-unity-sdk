using UnityEngine;
using System.Collections.Generic;


public class MoPubIosPrivateGUI : MonoBehaviour
{
#if UNITY_IPHONE
	private int _selectedToggleIndex;
	private string _bannerAdUnit = "60f3aa089e234ce6ad64046174d065a0";
	private string _interstitialAdUnit = "6995f8b34ce6453197c1af577a9c283d";
	private string _rewardedVideoAdUnit = null;
	private string[] _adVendorList = new string[] { "iAd", "Millennial", "AdMob", "Chartboost", "Vungle", "Facebook", "AdColony", "MoPub", "Unity Ads" };


	void Start()
	{
		MoPub.initializeRewardedVideo();
	}


	void OnGUI()
	{
		GUI.skin.button.margin = new RectOffset( 0, 0, 10, 0 );
		GUI.skin.button.stretchWidth = true;
		GUI.skin.button.fixedHeight = ( Screen.width >= 960 || Screen.height >= 960 ) ? 70 : 30;

		var halfWidth = Screen.width / 2;
		GUILayout.BeginArea( new Rect( 0, 0, halfWidth, Screen.height ) );
		GUILayout.BeginVertical();

		if( GUILayout.Button( "Create Banner (bottom right)" ) )
		{
			MoPub.createBanner( _bannerAdUnit, MoPubAdPosition.BottomRight, MoPubBannerType.Size320x50 );
		}


		if( GUILayout.Button( "Create Banner (top center)" ) )
		{
			MoPub.createBanner( _bannerAdUnit, MoPubAdPosition.TopCenter, MoPubBannerType.Size320x50 );
		}


		if( GUILayout.Button( "Destroy Banner" ) )
		{
			MoPub.destroyBanner();
		}


		GUILayout.BeginHorizontal();
		if( GUILayout.Button( "Show Banner" ) )
		{
			MoPub.showBanner( true );
		}


		if( GUILayout.Button( "Hide Banner" ) )
		{
			MoPub.showBanner( false );
		}
		GUILayout.EndHorizontal();


		GUILayout.Space( 20 );
		if( GUILayout.Button( "Request Interstitial" ) )
		{
			MoPub.requestInterstitialAd( _interstitialAdUnit );
			Debug.Log( "requesting interstitial with ad unit: " + _interstitialAdUnit );
		}


		if( GUILayout.Button( "Show Interstitial" ) )
		{
			MoPub.showInterstitialAd( _interstitialAdUnit );
		}


		GUILayout.EndVertical();
		GUILayout.EndArea();

		GUILayout.BeginArea( new Rect( Screen.width - halfWidth, 0, halfWidth, Screen.height ) );
		GUILayout.BeginVertical();


		if( GUILayout.Button( "Report App Open" ) )
		{
			MoPub.reportApplicationOpen( "ITUNES_APP_ID" );
		}


		if( GUILayout.Button( "Enable Location Support" ) )
		{
			MoPub.enableLocationSupport( true );
		}


		// no need to show the rewarded ad buttons if this network doesnt have them
		GUILayout.Space( 20 );
		if( _rewardedVideoAdUnit != null )
		{
			if( GUILayout.Button( "Request Rewarded Video" ) )
			{
				var adColonySettings = new MoPubMediationSetting( "AdColony" );
				adColonySettings.Add( "showPrePopup", true );
				adColonySettings.Add( "showPostPopup", true );

				var vungleSettings = new MoPubMediationSetting( "Vungle" );
				vungleSettings.Add( "userIdentifier", "the-user-id" );

				var unitySettings = new MoPubMediationSetting( "UnityAds" );
				unitySettings.Add( "userIdentifier", "the-user-id" );

				var mediationSettings = new List<MoPubMediationSetting>();
				mediationSettings.Add( adColonySettings );
				mediationSettings.Add( vungleSettings );

				MoPub.requestRewardedVideo( _rewardedVideoAdUnit, mediationSettings, "rewarded, video, mopub", 37.7833, 122.4167, "customer101" );
				Debug.Log( "requesting rewarded video with ad unit: " + _rewardedVideoAdUnit + " and mediation settings: " + MoPubInternal.ThirdParty.MiniJSON.Json.Serialize( mediationSettings ) );
			}


			if( GUILayout.Button( "Show Rewarded Video" ) )
			{
				MoPub.showRewardedVideo( _rewardedVideoAdUnit );
			}
		}
		else
		{
			GUILayout.Label( "No rewarded video ad unit for this network" );
		}


		GUILayout.EndVertical();
		GUILayout.EndArea();


		GUI.changed = false;
		_selectedToggleIndex = GUI.Toolbar( new Rect( 0, Screen.height - GUI.skin.button.fixedHeight, Screen.width, GUI.skin.button.fixedHeight ), _selectedToggleIndex, _adVendorList );
		if( GUI.changed )
		{
			_rewardedVideoAdUnit = null;
			_bannerAdUnit = "";
			switch( _selectedToggleIndex )
			{
				case 0: // iAd
					_bannerAdUnit = "60f3aa089e234ce6ad64046174d065a0";
					_interstitialAdUnit = "6995f8b34ce6453197c1af577a9c283d";
					break;
				case 1: // Millennial
					_bannerAdUnit = "1b282680106246aa83036892b32ec7cc";
					_interstitialAdUnit = "0da9e2762f1a48bab695887fb7798b66";
					break;
				case 2: // AdMob
					_bannerAdUnit = "d2b8a9fcd92440e79c7437d2b51f25a6";
					_interstitialAdUnit = "4f9d8fb8521f4420b2429184f720f42b";
					break;
				case 3: // Chartboost
					_interstitialAdUnit = "a97fa010d9c24d06ae267be2a1487af1";
					_rewardedVideoAdUnit = "2942576082c24e0f80c6172703572870";
					break;
				case 4: // Vungle
					_interstitialAdUnit = "c87b1701e1084507bf8be89cd13b890c";
					_rewardedVideoAdUnit = "19a24d282ecb49c5bb43c65f501e33bf";
					break;
				case 5: // Facebook
					_bannerAdUnit = "fb759131fd7a40e6b9d324e637a4b299";
					_interstitialAdUnit = "27614fde27df488493327f2b952f9d21";
					break;
				case 6: // AdColony
					_bannerAdUnit = "";
					_interstitialAdUnit = "09fed773d1e34cba968d910b4fbdc850";
					_rewardedVideoAdUnit = "52aa460767374250a5aa5174c2345be3";
					break;
				case 7: // MoPub
					_bannerAdUnit = "23b49916add211e281c11231392559e4";
					_interstitialAdUnit = "3aba0056add211e281c11231392559e4";
					break;
				case 8: // Unity Ads
					_interstitialAdUnit = "4fab4888caa048e085a1dc5c78816061";
					_rewardedVideoAdUnit = "676a0fa97aca48cbbe489de5b2fa4cd1";
					break;
			}
		}
	}
#endif
}

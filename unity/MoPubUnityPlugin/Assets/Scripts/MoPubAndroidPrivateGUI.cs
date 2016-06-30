using UnityEngine;
using System.Collections.Generic;


public class MoPubAndroidPrivateGUI : MonoBehaviour
{
#if UNITY_ANDROID
	private int _selectedToggleIndex;
	private string _bannerAdUnit = "23b49916add211e281c11231392559e4";
	private string _interstitialAdUnit = "3aba0056add211e281c11231392559e4";
	private string _rewardedVideoAdUnit = null;
	private string[] _adVendorList = new string[] { "MoPub", "Millennial", "AdMob", "Chartboost", "Vungle", "Facebook", "AdColony", "Unity Ads" };


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
			if( _bannerAdUnit == "" )
			{
				Debug.LogWarning( "No banner ad unit ID is available for the currently selected platform" );
				return;
			}
			MoPub.createBanner( _bannerAdUnit, MoPubAdPosition.BottomRight );
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
			MoPub.reportApplicationOpen();
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
				adColonySettings.Add( "withConfirmationDialog", true );
				adColonySettings.Add( "withResultsDialog", true );

				var chartboostSettings = new MoPubMediationSetting( "Chartboost" );
				chartboostSettings.Add( "customId", "the-user-id" );

				var vungleSettings = new MoPubMediationSetting( "Vungle" );
				vungleSettings.Add( "userId", "the-user-id" );
				vungleSettings.Add( "cancelDialogBody", "Cancel Body" );
				vungleSettings.Add( "cancelDialogCloseButton", "Shut it Down" );
				vungleSettings.Add( "cancelDialogKeepWatchingButton", "Watch On" );
				vungleSettings.Add( "cancelDialogTitle", "Cancel Title" );

				var mediationSettings = new List<MoPubMediationSetting>();
				mediationSettings.Add( adColonySettings );
				mediationSettings.Add( chartboostSettings );
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
				case 0: // MoPub
					_bannerAdUnit = "23b49916add211e281c11231392559e4";
					_interstitialAdUnit = "3aba0056add211e281c11231392559e4";
					_rewardedVideoAdUnit = "db2ef0eb1600433a8cdc31c75549c6b1";
					break;
				case 1: // Millennial
					_bannerAdUnit = "1aa442709c9f11e281c11231392559e4";
					_interstitialAdUnit = "c6566f7bd85c40afb7afc4232a1cd463";
					break;
				case 2: // AdMob
					_bannerAdUnit = "173f4589c04a43b1b2e2e49d05f58e80";
					_interstitialAdUnit = "554e8baff8d84137941b5a55354105fc";
					break;
				case 3: // Chartboost
					_interstitialAdUnit = "376366b49d324dedae3d5edb360c27b4";
					_rewardedVideoAdUnit = "df605ab15b56400285c99e521ecc2cb1";
					break;
				case 4: // Vungle
					_interstitialAdUnit = "4f5e1e97f87c406cb7878b9eff1d2a77";
					_rewardedVideoAdUnit = "2d38f4e6881341369e9fc2c2d01ddc9d";
					break;
				case 5: // Facebook
					// Native: f97733db27f44defbeb39ce495047779
					_bannerAdUnit = "b40a96dd275e4ce5be2cdf5faa92007d";
					_interstitialAdUnit = "9792d876011f4359887d2d26380e8a84";
					break;
				case 6: // AdColony
					_interstitialAdUnit = "3aa79f11389540db8e250a80e4d16a46";
					_rewardedVideoAdUnit = "e258c916e659447d9d98256a3ab2979e";
					break;
				case 7: // Unity Ads
					_interstitialAdUnit = "079f9caa99eb429588c2c3633e1ce3e3";
					_rewardedVideoAdUnit = "4302e96be4584fa6b653a0668a845407";
					break;
			}
		}
	}
#endif
}

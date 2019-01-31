using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MoPubInternal.ThirdParty.MiniJSON;
using UnityEngine;

[SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
public class MoPubManager : MonoBehaviour
{
    public static MoPubManager Instance { get; private set; }

    // Fired when the SDK has finished initializing
    public static event Action<string> OnSdkInitializedEvent;

    // Fired when an ad loads in the banner. Includes the ad height.
    public static event Action<string, float> OnAdLoadedEvent;

    // Fired when an ad fails to load for the banner
    public static event Action<string, string> OnAdFailedEvent;

    // Android only. Fired when a banner ad is clicked
    public static event Action<string> OnAdClickedEvent;

    // Android only. Fired when a banner ad expands to encompass a greater portion of the screen
    public static event Action<string> OnAdExpandedEvent;

    // Android only. Fired when a banner ad collapses back to its initial size
    public static event Action<string> OnAdCollapsedEvent;

    // Fired when an interstitial ad is loaded and ready to be shown
    public static event Action<string> OnInterstitialLoadedEvent;

    // Fired when an interstitial ad fails to load
    public static event Action<string, string> OnInterstitialFailedEvent;

    // Fired when an interstitial ad is dismissed
    public static event Action<string> OnInterstitialDismissedEvent;

    // iOS only. Fired when an interstitial ad expires
    public static event Action<string> OnInterstitialExpiredEvent;

    // Android only. Fired when an interstitial ad is displayed
    public static event Action<string> OnInterstitialShownEvent;

    // Android only. Fired when an interstitial ad is clicked
    public static event Action<string> OnInterstitialClickedEvent;

    // Fired when a rewarded video finishes loading and is ready to be displayed
    public static event Action<string> OnRewardedVideoLoadedEvent;

    // Fired when a rewarded video fails to load. Includes the error message.
    public static event Action<string, string> OnRewardedVideoFailedEvent;

    // iOS only. Fired when a rewarded video expires
    public static event Action<string> OnRewardedVideoExpiredEvent;

    // Fired when an rewarded video is displayed
    public static event Action<string> OnRewardedVideoShownEvent;

    // Fired when an rewarded video is clicked
    public static event Action<string> OnRewardedVideoClickedEvent;

    // Fired when a rewarded video fails to play. Includes the error message.
    public static event Action<string, string> OnRewardedVideoFailedToPlayEvent;

    // Fired when a rewarded video completes. Includes all the data available about the reward.
    public static event Action<string, string, float> OnRewardedVideoReceivedRewardEvent;

    // Fired when a rewarded video closes
    public static event Action<string> OnRewardedVideoClosedEvent;

    // iOS only. Fired when a rewarded video event causes another application to open
    public static event Action<string> OnRewardedVideoLeavingApplicationEvent;

#if mopub_native_beta

    // Fired when a native ad is loaded
    public static event Action<string, AbstractNativeAd.Data> OnNativeLoadEvent;

    // Fired when a native ad is shown
    public static event Action<string> OnNativeImpressionEvent;

    // Fired when a native ad is clicked
    public static event Action<string> OnNativeClickEvent;

    // Fired when a native ad fails to load
    public static event Action<string, string> OnNativeFailEvent;

#endif

    // Fired when the SDK has been notified of a change in the user's consent status for data tracking.
    public static event Action<MoPub.Consent.Status, MoPub.Consent.Status, bool> OnConsentStatusChangedEvent;

    // Fired when the SDK has finished loading (retrieving from the web) the MoPub consent dialog interstitial.
    public static event Action OnConsentDialogLoadedEvent;

    // Fired when an error occurred while attempting to load the MoPub consent dialog.
    public static event Action<string> OnConsentDialogFailedEvent;

    // Fired when the MoPub consent dialog has been presented on screen.
    public static event Action OnConsentDialogShownEvent;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
            Destroy(this);
    }


    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }


    // Will return a non-null array of strings with at least 'min' non-null string values at the front.
    private string[] DecodeArgs(string argsJson, int min)
    {
        bool err = false;
        var args = Json.Deserialize(argsJson) as List<object>;
        if (args == null) {
            Debug.LogError("Invalid JSON data: " + argsJson);
            args = new List<object>();
            err = true;
        }
        if (args.Count < min) {
            if (!err)  // Don't double up the error messages for invalid JSON
                Debug.LogError("Missing one or more values: " + argsJson + " (expected " + min + ")");
            while (args.Count < min)
                args.Add("");
        }
        return args.Select(v => v.ToString()).ToArray();
    }


    public void EmitSdkInitializedEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];
        var logLevel = MoPub.LogLevel.MPLogLevelNone;
        if (args.Length > 1) {
            try {
                logLevel = (MoPub.LogLevel) Enum.Parse(typeof(MoPub.LogLevel), args[1]);
            } catch (ArgumentException) {
                Debug.LogWarning("Invalid LogLevel received: " + args[1]);
            }
        } else {
            Debug.LogWarning("No LogLevel received");
        }

        MoPubLog.Log("EmitSdkInitializedEvent", MoPubLog.SdkLogEvent.InitFinished, logLevel);
        var evt = OnSdkInitializedEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitConsentStatusChangedEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min:3);
        var oldConsent = MoPub.Consent.FromString(args[0]);
        var newConsent = MoPub.Consent.FromString(args[1]);
        var canCollectPersonalInfo = args[2].ToLower() == "true";

        MoPubLog.Log("EmitConsentStatusChangedEvent", MoPubLog.ConsentLogEvent.Updated, newConsent,
            canCollectPersonalInfo);
        var evt = OnConsentStatusChangedEvent;
        if (evt != null) evt(oldConsent, newConsent, canCollectPersonalInfo);
    }


    public void EmitConsentDialogLoadedEvent()
    {
        MoPubLog.Log("EmitConsentDialogLoadedEvent", MoPubLog.ConsentLogEvent.LoadSuccess);
        var evt = OnConsentDialogLoadedEvent;
        if (evt != null) evt();
    }


    public void EmitConsentDialogFailedEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 1);
        var err = args[0];

        MoPubLog.Log("EmitConsentDialogFailedEvent", MoPubLog.ConsentLogEvent.LoadFailed, err);
        var evt = OnConsentDialogFailedEvent;
        if (evt != null) evt(err);
    }


    public void EmitConsentDialogShownEvent()
    {
        MoPubLog.Log("EmitConsentDialogShownEvent", MoPubLog.ConsentLogEvent.ShowSuccess);
        var evt = OnConsentDialogShownEvent;
        if (evt != null) evt();
    }


    // Banner Listeners


    public void EmitAdLoadedEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 2);
        var adUnitId = args[0];
        var heightStr = args[1];

        MoPubLog.Log("EmitAdLoadedEvent", MoPubLog.AdLogEvent.LoadSuccess);
        MoPubLog.Log("EmitAdLoadedEvent", MoPubLog.AdLogEvent.ShowSuccess);
        var evt = OnAdLoadedEvent;
        if (evt != null) evt(adUnitId, float.Parse(heightStr));
    }


    public void EmitAdFailedEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 2);
        var adUnitId = args[0];
        var error = args[1];

        MoPubLog.Log("EmitAdFailedEvent", MoPubLog.AdLogEvent.LoadFailed, error);
        var evt = OnAdFailedEvent;
        if (evt != null) evt(adUnitId, error);
    }


    public void EmitAdClickedEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitAdClickedEvent", MoPubLog.AdLogEvent.Tapped);
        var evt = OnAdClickedEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitAdExpandedEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitAdExpandedEvent", MoPubLog.AdLogEvent.Expanded);
        var evt = OnAdExpandedEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitAdCollapsedEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitAdCollapsedEvent", MoPubLog.AdLogEvent.Collapsed);
        var evt = OnAdCollapsedEvent;
        if (evt != null) evt(adUnitId);
    }


    // Interstitial Listeners


    public void EmitInterstitialLoadedEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitInterstitialLoadedEvent", MoPubLog.AdLogEvent.LoadSuccess);
        var evt = OnInterstitialLoadedEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitInterstitialFailedEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 2);
        var adUnitId = args[0];
        var error = args[1];

        MoPubLog.Log("EmitInterstitialFailedEvent", MoPubLog.AdLogEvent.LoadFailed, error);
        var evt = OnInterstitialFailedEvent;
        if (evt != null) evt(adUnitId, error);
    }


    public void EmitInterstitialDismissedEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitInterstitialDismissedEvent", MoPubLog.AdLogEvent.Dismissed);
        var evt = OnInterstitialDismissedEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitInterstitialDidExpireEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitInterstitialDidExpireEvent", MoPubLog.AdLogEvent.Expired);
        var evt = OnInterstitialExpiredEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitInterstitialShownEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitInterstitialShownEvent", MoPubLog.AdLogEvent.ShowSuccess);
        var evt = OnInterstitialShownEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitInterstitialClickedEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitInterstitialClickedEvent", MoPubLog.AdLogEvent.Tapped);
        var evt = OnInterstitialClickedEvent;
        if (evt != null) evt(adUnitId);
    }


    // Rewarded Video Listeners


    public void EmitRewardedVideoLoadedEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitRewardedVideoLoadedEvent", MoPubLog.AdLogEvent.LoadSuccess);
        var evt = OnRewardedVideoLoadedEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitRewardedVideoFailedEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 2);
        var adUnitId = args[0];
        var error = args[1];

        MoPubLog.Log("EmitRewardedVideoFailedEvent", MoPubLog.AdLogEvent.LoadFailed, error);
        var evt = OnRewardedVideoFailedEvent;
        if (evt != null) evt(adUnitId, error);
    }


    public void EmitRewardedVideoExpiredEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitRewardedVideoExpiredEvent", MoPubLog.AdLogEvent.Expired);
        var evt = OnRewardedVideoExpiredEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitRewardedVideoShownEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitRewardedVideoShownEvent", MoPubLog.AdLogEvent.ShowSuccess);
        var evt = OnRewardedVideoShownEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitRewardedVideoClickedEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitRewardedVideoClickedEvent", MoPubLog.AdLogEvent.Tapped);
        var evt = OnRewardedVideoClickedEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitRewardedVideoFailedToPlayEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 2);
        var adUnitId = args[0];
        var error = args[1];

        var evt = OnRewardedVideoFailedToPlayEvent;
        if (evt != null) evt(adUnitId, error);
    }


    public void EmitRewardedVideoReceivedRewardEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 3);
        var adUnitId = args[0];
        var label = args[1];
        var amountStr = args[2];

        MoPubLog.Log("EmitRewardedVideoReceivedRewardEvent", MoPubLog.AdLogEvent.ShouldReward);
        var evt = OnRewardedVideoReceivedRewardEvent;
        if (evt != null) evt(adUnitId, label, float.Parse(amountStr));
    }


    public void EmitRewardedVideoClosedEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        MoPubLog.Log("EmitRewardedVideoClosedEvent", MoPubLog.AdLogEvent.Dismissed);
        var evt = OnRewardedVideoClosedEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitRewardedVideoLeavingApplicationEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 1);
        var adUnitId = args[0];

        var evt = OnRewardedVideoLeavingApplicationEvent;
        if (evt != null) evt(adUnitId);
    }


#if mopub_native_beta
    public void EmitNativeLoadEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 2);
        var adUnitId = args[0];
        var data = AbstractNativeAd.Data.FromJson(args[1]);

        MoPubLog.Log("EmitNativeLoadEvent", MoPubLog.AdLogEvent.LoadSuccess);
        EmitNativeLoadEvent(adUnitId, data);
    }


    public void EmitNativeFailEvent(string argsJson)
    {
        var args = DecodeArgs(argsJson, min: 2);
        var adUnitId = args[0];
        var error = args[1];

        MoPubLog.Log("EmitNativeFailEvent", MoPubLog.AdLogEvent.LoadFailed, error);
        var evt = OnNativeFailEvent;
        if (evt != null) evt(adUnitId, error);
    }


    public void EmitNativeLoadEvent(string adUnitId, AbstractNativeAd.Data nativeAdData)
    {
        var evt = OnNativeLoadEvent;
        if (evt != null) evt(adUnitId, nativeAdData);
    }


    public void EmitNativeImpressionEvent(string adUnitId)
    {
        MoPubLog.Log("EmitNativeImpressionEvent", MoPubLog.AdLogEvent.ShowSuccess);
        var evt = OnNativeImpressionEvent;
        if (evt != null) evt(adUnitId);
    }


    public void EmitNativeClickEvent(string adUnitId)
    {
        MoPubLog.Log("EmitNativeClickEvent", MoPubLog.AdLogEvent.Tapped);
        var evt = OnNativeClickEvent;
        if (evt != null) evt(adUnitId);
    }
#endif
}

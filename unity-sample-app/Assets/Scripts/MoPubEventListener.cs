using System.Diagnostics.CodeAnalysis;
using UnityEngine;

[SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
[SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
public class MoPubEventListener : MonoBehaviour
{
    [SerializeField]
    private MoPubDemoGUI _demoGUI;


    private void Awake()
    {
        if (_demoGUI == null)
            _demoGUI = GetComponent<MoPubDemoGUI>();

        if (_demoGUI != null) return;
        Debug.LogError("Missing reference to MoPubDemoGUI.  Please fix in the editor.");
        Destroy(this);
    }


    private void OnEnable()
    {
        MoPubManager.OnSdkInitalizedEvent += OnSdkInitializedEvent;

        MoPubManager.OnConsentStatusChangedEvent += OnConsentStatusChangedEvent;
        MoPubManager.OnConsentDialogLoadedEvent += OnConsentDialogLoadedEvent;
        MoPubManager.OnConsentDialogFailedEvent += OnConsentDialogFailedEvent;
        MoPubManager.OnConsentDialogShownEvent += OnConsentDialogShownEvent;

        MoPubManager.OnAdLoadedEvent += OnAdLoadedEvent;
        MoPubManager.OnAdFailedEvent += OnAdFailedEvent;
        MoPubManager.OnAdClickedEvent += OnAdClickedEvent;
        MoPubManager.OnAdExpandedEvent += OnAdExpandedEvent;
        MoPubManager.OnAdCollapsedEvent += OnAdCollapsedEvent;

        MoPubManager.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
        MoPubManager.OnInterstitialFailedEvent += OnInterstitialFailedEvent;
        MoPubManager.OnInterstitialShownEvent += OnInterstitialShownEvent;
        MoPubManager.OnInterstitialClickedEvent += OnInterstitialClickedEvent;
        MoPubManager.OnInterstitialDismissedEvent += OnInterstitialDismissedEvent;
        MoPubManager.OnInterstitialExpiredEvent += OnInterstitialExpiredEvent;

        MoPubManager.OnRewardedVideoLoadedEvent += OnRewardedVideoLoadedEvent;
        MoPubManager.OnRewardedVideoFailedEvent += OnRewardedVideoFailedEvent;
        MoPubManager.OnRewardedVideoExpiredEvent += OnRewardedVideoExpiredEvent;
        MoPubManager.OnRewardedVideoShownEvent += OnRewardedVideoShownEvent;
        MoPubManager.OnRewardedVideoClickedEvent += OnRewardedVideoClickedEvent;
        MoPubManager.OnRewardedVideoFailedToPlayEvent += OnRewardedVideoFailedToPlayEvent;
        MoPubManager.OnRewardedVideoReceivedRewardEvent += OnRewardedVideoReceivedRewardEvent;
        MoPubManager.OnRewardedVideoClosedEvent += OnRewardedVideoClosedEvent;
        MoPubManager.OnRewardedVideoLeavingApplicationEvent += OnRewardedVideoLeavingApplicationEvent;

#if mopub_native_beta
        MoPubManager.OnNativeLoadEvent += OnNativeLoadEvent;
        MoPubManager.OnNativeFailEvent += OnNativeFailEvent;
        MoPubManager.OnNativeImpressionEvent += OnNativeImpressionEvent;
        MoPubManager.OnNativeClickEvent += OnNativeClickEvent;
#endif
    }


    private void OnDisable()
    {
        // Remove all event handlers
        MoPubManager.OnSdkInitalizedEvent -= OnSdkInitializedEvent;

        MoPubManager.OnConsentStatusChangedEvent -= OnConsentStatusChangedEvent;
        MoPubManager.OnConsentDialogLoadedEvent -= OnConsentDialogLoadedEvent;
        MoPubManager.OnConsentDialogFailedEvent -= OnConsentDialogFailedEvent;
        MoPubManager.OnConsentDialogShownEvent -= OnConsentDialogShownEvent;

        MoPubManager.OnAdLoadedEvent -= OnAdLoadedEvent;
        MoPubManager.OnAdFailedEvent -= OnAdFailedEvent;
        MoPubManager.OnAdClickedEvent -= OnAdClickedEvent;
        MoPubManager.OnAdExpandedEvent -= OnAdExpandedEvent;
        MoPubManager.OnAdCollapsedEvent -= OnAdCollapsedEvent;

        MoPubManager.OnInterstitialLoadedEvent -= OnInterstitialLoadedEvent;
        MoPubManager.OnInterstitialFailedEvent -= OnInterstitialFailedEvent;
        MoPubManager.OnInterstitialShownEvent -= OnInterstitialShownEvent;
        MoPubManager.OnInterstitialClickedEvent -= OnInterstitialClickedEvent;
        MoPubManager.OnInterstitialDismissedEvent -= OnInterstitialDismissedEvent;
        MoPubManager.OnInterstitialExpiredEvent -= OnInterstitialExpiredEvent;

        MoPubManager.OnRewardedVideoLoadedEvent -= OnRewardedVideoLoadedEvent;
        MoPubManager.OnRewardedVideoFailedEvent -= OnRewardedVideoFailedEvent;
        MoPubManager.OnRewardedVideoExpiredEvent -= OnRewardedVideoExpiredEvent;
        MoPubManager.OnRewardedVideoShownEvent -= OnRewardedVideoShownEvent;
        MoPubManager.OnRewardedVideoClickedEvent -= OnRewardedVideoClickedEvent;
        MoPubManager.OnRewardedVideoFailedToPlayEvent -= OnRewardedVideoFailedToPlayEvent;
        MoPubManager.OnRewardedVideoReceivedRewardEvent -= OnRewardedVideoReceivedRewardEvent;
        MoPubManager.OnRewardedVideoClosedEvent -= OnRewardedVideoClosedEvent;
        MoPubManager.OnRewardedVideoLeavingApplicationEvent -= OnRewardedVideoLeavingApplicationEvent;

#if mopub_native_beta
        MoPubManager.OnNativeLoadEvent -= OnNativeLoadEvent;
        MoPubManager.OnNativeFailEvent -= OnNativeFailEvent;
        MoPubManager.OnNativeImpressionEvent -= OnNativeImpressionEvent;
        MoPubManager.OnNativeClickEvent -= OnNativeClickEvent;
#endif
    }


    private void AdFailed(string adUnitId, string action, string error)
    {
        var errorMsg = "Failed to " + action + " ad unit " + adUnitId;
        if (!string.IsNullOrEmpty(error))
            errorMsg += ": " + error;
        Debug.LogError(errorMsg);
        _demoGUI.UpdateStatusLabel("Error: " + errorMsg);
    }


    private void OnSdkInitializedEvent(string adUnitId)
    {
        Debug.Log("OnSdkInitializedEvent: " + adUnitId);
        _demoGUI.SdkInitialized();
    }


    private void OnConsentStatusChangedEvent(MoPub.Consent.Status oldStatus, MoPub.Consent.Status newStatus,
                                             bool canCollectPersonalInfo)
    {
        Debug.Log("OnConsetStatusChangedEvent: old=" + oldStatus + " new=" + newStatus + " personalInfoOk=" + canCollectPersonalInfo);
        _demoGUI.ConsentStatusChanged(newStatus, canCollectPersonalInfo);
    }


    private void OnConsentDialogLoadedEvent()
    {
        Debug.Log("OnConsentDialogLoadedEvent: dialog loaded");
        _demoGUI.ConsentDialogLoaded = true;
    }


    private void OnConsentDialogFailedEvent(string err)
    {
        Debug.Log("OnConsentDialogFailedEvent: " + err);
        _demoGUI.UpdateStatusLabel(err);
    }


    private void OnConsentDialogShownEvent()
    {
        Debug.Log("OnConsentDialogShownEvent: dialog shown");
        _demoGUI.ConsentDialogLoaded = false;
    }


    // Banner Events


    private void OnAdLoadedEvent(string adUnitId, float height)
    {
        Debug.Log("OnAdLoadedEvent: " + adUnitId + " height: " + height);
        _demoGUI.BannerLoaded(adUnitId, height);
    }


    private void OnAdFailedEvent(string adUnitId, string error)
    {
        AdFailed(adUnitId, "load banner", error);
    }


    private void OnAdClickedEvent(string adUnitId)
    {
        Debug.Log("OnAdClickedEvent: " + adUnitId);
    }


    private void OnAdExpandedEvent(string adUnitId)
    {
        Debug.Log("OnAdExpandedEvent: " + adUnitId);
    }


    private void OnAdCollapsedEvent(string adUnitId)
    {
        Debug.Log("OnAdCollapsedEvent: " + adUnitId);
    }


    // Interstitial Events


    private void OnInterstitialLoadedEvent(string adUnitId)
    {
        Debug.Log("OnInterstitialLoadedEvent: " + adUnitId);
        _demoGUI.AdLoaded(adUnitId);
    }


    private void OnInterstitialFailedEvent(string adUnitId, string error)
    {
        AdFailed(adUnitId, "load interstitial", error);
    }


    private void OnInterstitialShownEvent(string adUnitId)
    {
        Debug.Log("OnInterstitialShownEvent: " + adUnitId);
    }


    private void OnInterstitialClickedEvent(string adUnitId)
    {
        Debug.Log("OnInterstitialClickedEvent: " + adUnitId);
    }


    private void OnInterstitialDismissedEvent(string adUnitId)
    {
        Debug.Log("OnInterstitialDismissedEvent: " + adUnitId);
        _demoGUI.AdDismissed(adUnitId);
    }


    private void OnInterstitialExpiredEvent(string adUnitId)
    {
        Debug.Log("OnInterstitialExpiredEvent: " + adUnitId);
    }


    // Rewarded Video Events


    private void OnRewardedVideoLoadedEvent(string adUnitId)
    {
        Debug.Log("OnRewardedVideoLoadedEvent: " + adUnitId);

        var availableRewards = MoPub.GetAvailableRewards(adUnitId);
        _demoGUI.AdLoaded(adUnitId);
        _demoGUI.LoadAvailableRewards(adUnitId, availableRewards);
    }


    private void OnRewardedVideoFailedEvent(string adUnitId, string error)
    {
        AdFailed(adUnitId, "load rewarded video", error);
    }


    private void OnRewardedVideoExpiredEvent(string adUnitId)
    {
        Debug.Log("OnRewardedVideoExpiredEvent: " + adUnitId);
    }


    private void OnRewardedVideoShownEvent(string adUnitId)
    {
        Debug.Log("OnRewardedVideoShownEvent: " + adUnitId);
    }


    private void OnRewardedVideoClickedEvent(string adUnitId)
    {
        Debug.Log("OnRewardedVideoClickedEvent: " + adUnitId);
    }


    private void OnRewardedVideoFailedToPlayEvent(string adUnitId, string error)
    {
        AdFailed(adUnitId, "play rewarded video", error);
    }


    private void OnRewardedVideoReceivedRewardEvent(string adUnitId, string label, float amount)
    {
        Debug.Log("OnRewardedVideoReceivedRewardEvent for ad unit id " + adUnitId + " currency:" + label + " amount:" + amount);
    }


    private void OnRewardedVideoClosedEvent(string adUnitId)
    {
        Debug.Log("OnRewardedVideoClosedEvent: " + adUnitId);
        _demoGUI.AdDismissed(adUnitId);
    }


    private void OnRewardedVideoLeavingApplicationEvent(string adUnitId)
    {
        Debug.Log("OnRewardedVideoLeavingApplicationEvent: " + adUnitId);
    }


#if mopub_native_beta
    private void OnNativeLoadEvent(string adUnitId, AbstractNativeAd.Data nativeAdData)
    {
        Debug.Log("OnNativeLoadEvent: ad unit id: " + adUnitId + " data: " + nativeAdData);
        _demoGUI.NativeAdLoaded(adUnitId, nativeAdData);
    }


    private void OnNativeFailEvent(string adUnitId, string error)
    {
        AdFailed(adUnitId, "load native ad", error);
    }


    private void OnNativeImpressionEvent(string adUnitId)
    {
        Debug.Log("OnNativeImpressionEvent: " + adUnitId);
    }


    private void OnNativeClickEvent(string adUnitId)
    {
        Debug.Log("OnNativeClickEvent: " + adUnitId);
    }
#endif
}

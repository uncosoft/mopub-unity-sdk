#if mopub_native_beta && mopub_mediation
using System;
using AudienceNetwork;
using UnityEngine;
using UnityEngine.UI;

public class FacebookNativeAd : AbstractNativeAd
{
    private NativeAd _nativeAd;


    protected override void OptionalInit()
    {
        // Create a native ad request with a unique placement ID (generate your own on the Facebook app settings).
        // Use different ID for each ad placement in your app.
        _nativeAd = new NativeAd(AdUnitId);

        // Wire up GameObject with the native ad; the specified buttons will be clickable.
        _nativeAd.RegisterGameObjectForImpression(gameObject, new Button[] { });
    }


    protected override void AddEventHandlers()
    {
        // Set delegates to get notified on changes or when the user interacts with the ad.
        _nativeAd.NativeAdDidLoad = () => {
            MoPubManager.Instance.EmitNativeLoadEvent(_nativeAd.PlacementId, new Data {
                MainImageUrl = Data.ToUri(_nativeAd.CoverImageURL),
                IconImageUrl = Data.ToUri(_nativeAd.IconImageURL),

                CallToAction = _nativeAd.CallToAction,
                Title = _nativeAd.Title,
                Text = _nativeAd.SocialContext,

                PrivacyInformationIconClickThroughUrl = Data.ToUri(_nativeAd.AdChoicesLinkURL),
                PrivacyInformationIconImageUrl = Data.ToUri(_nativeAd.AdChoicesImageURL)
            });
        };
        _nativeAd.NativeAdDidFailWithError = error => { Debug.Log("Native ad failed to load with error: " + error); };
        _nativeAd.NativeAdWillLogImpression = () => { Debug.Log("Native ad logged impression."); };
        _nativeAd.NativeAdDidClick = () => { Debug.Log("Native ad clicked."); };
    }


    protected override void RemoveEventHandlers()
    {
        _nativeAd = null;
    }


    public override void LoadAd()
    {
        // Initiate a request to load an ad.
        _nativeAd.LoadAd();
        Debug.Log("Native ad loading...");
    }
}
#else
public class FacebookNativeAd : AbstractNativeAd {}
#endif

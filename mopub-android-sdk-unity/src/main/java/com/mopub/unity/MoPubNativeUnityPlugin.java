package com.mopub.unity;

import android.util.Log;
import android.view.View;

import com.mopub.nativeads.MoPubNative;
import com.mopub.nativeads.MoPubStaticNativeAdRenderer;
import com.mopub.nativeads.NativeAd;
import com.mopub.nativeads.NativeErrorCode;
import com.mopub.nativeads.StaticNativeAd;
import com.mopub.nativeads.ViewBinder;
import com.unity3d.player.UnityPlayer;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.Locale;


/**
 * Provides an API that bridges the Unity Plugin with the MoPub Native SDK.
 *
 * NOTE: This feature is still in Beta; if interested, please contact support@mopub.com
 */
public class MoPubNativeUnityPlugin extends MoPubUnityPlugin
        implements NativeAd.MoPubNativeEventListener, MoPubNative.MoPubNativeNetworkListener {

    private MoPubNative mMoPubNative;

    /**
     * Creates a {@link MoPubNativeUnityPlugin} for the given ad unit ID.
     *
     * @param adUnitId String for the ad unit ID to use for this native ad.
     */
    public MoPubNativeUnityPlugin(final String adUnitId) {
        super(adUnitId);
    }


    /* ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** *****
     * Native API                                                                              *
     * ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** *****/

    /**
     * Loads a native ad for the current ad unit ID and with the given keywords.
     */
    public void requestNativeAd() {
        // TODO: Add keywords to native ads
        runSafelyOnUiThread(new Runnable() {
            public void run() {
                mMoPubNative = new MoPubNative(getActivity(), mAdUnitId,
                        MoPubNativeUnityPlugin.this);
                mMoPubNative.registerAdRenderer(new MoPubStaticNativeAdRenderer(
                        new ViewBinder.Builder(-1).build()));
                mMoPubNative.makeRequest();
            }
        });
    }


    /* ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** *****
     * MoPubNativeEventListener implementation                                                 *
     * ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** *****/

    @Override
    public void onImpression(View view) {
        UnityEvent.NativeImpression.Emit(mAdUnitId);
    }

    @Override
    public void onClick(View view) {
        UnityEvent.NativeClick.Emit(mAdUnitId);
    }

    @Override
    public void onNativeLoad(NativeAd nativeAd) {
        StaticNativeAd ad = (StaticNativeAd) nativeAd.getBaseNativeAd();

        if (mAdUnitId.equals(nativeAd.getAdUnitId())) {
            try {
                JSONObject json = new JSONObject();
                json.put("mainImageUrl", ad.getMainImageUrl());
                json.put("iconImageUrl", ad.getIconImageUrl());
                json.put("clickDestinationUrl", ad.getClickDestinationUrl());
                json.put("callToAction", ad.getCallToAction());
                json.put("title", ad.getTitle());
                json.put("text", ad.getText());
                json.put("starRating", ad.getStarRating());
                json.put("privacyInformationIconClickThroughUrl", ad.getPrivacyInformationIconClickThroughUrl());
                json.put("privacyInformationIconImageUrl", ad.getPrivacyInformationIconImageUrl());

                UnityEvent.NativeLoad.Emit(mAdUnitId, json.toString());
            } catch (JSONException e) {
                e.printStackTrace();
            }
        }

    }

    @Override
    public void onNativeFail(NativeErrorCode errorCode) {
        UnityEvent.NativeFail.Emit(mAdUnitId, errorCode.toString());
    }
}

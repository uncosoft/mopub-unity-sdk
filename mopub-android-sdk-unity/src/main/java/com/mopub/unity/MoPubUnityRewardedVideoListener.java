package com.mopub.unity;

import android.util.Log;

import com.mopub.common.MoPubReward;
import com.mopub.mobileads.MoPubErrorCode;
import com.mopub.mobileads.MoPubRewardedVideoListener;
import com.unity3d.player.UnityPlayer;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.Set;


public class MoPubUnityRewardedVideoListener implements MoPubRewardedVideoListener {
    private static String TAG = "MoPub";
    private static MoPubUnityRewardedVideoListener sInstance;

    public static MoPubUnityRewardedVideoListener getInstance() {
        if (sInstance == null)
            sInstance = new MoPubUnityRewardedVideoListener();
        return sInstance;
    }

    /* ***** ***** ***** ***** ***** ***** ***** *****
     * RewardedVideoListener implementation
     */
    @Override
    public void onRewardedVideoLoadSuccess(String adUnitId) {
        UnityPlayer.UnitySendMessage("MoPubManager", "onRewardedVideoLoaded", adUnitId);
    }

    @Override
    public void onRewardedVideoLoadFailure(String adUnitId, MoPubErrorCode errorCode) {
        Log.i(TAG, "onRewardedVideoFailed: " + errorCode);
        UnityPlayer.UnitySendMessage("MoPubManager", "onRewardedVideoFailed", errorCode.toString());
    }

    @Override
    public void onRewardedVideoStarted(String adUnitId) {
        UnityPlayer.UnitySendMessage("MoPubManager", "onRewardedVideoShown", adUnitId);
    }

    @Override
    public void onRewardedVideoPlaybackError(String adUnitId, MoPubErrorCode errorCode) {
        UnityPlayer.UnitySendMessage("MoPubManager", "onRewardedVideoFailedToPlay", adUnitId);
    }

    @Override
    public void onRewardedVideoClosed(String adUnitId) {
        UnityPlayer.UnitySendMessage("MoPubManager", "onRewardedVideoClosed", adUnitId);
    }

    @Override
    public void onRewardedVideoCompleted(Set<String> adUnitIds, MoPubReward reward) {
        if (adUnitIds.size() == 0 || reward == null) {
            Log.i(TAG, "onRewardedVideoCompleted with no adUnitId and/or reward. Bailing out.");
            return;
        }

        try {
            JSONObject json = new JSONObject();
            json.put("adUnitId", adUnitIds.toArray()[0].toString());
            json.put("currencyType", "");
            json.put("amount", reward.getAmount());

            UnityPlayer.UnitySendMessage("MoPubManager", "onRewardedVideoReceivedReward", json.toString());
        } catch (JSONException e) {
            e.printStackTrace();
        }
    }
}

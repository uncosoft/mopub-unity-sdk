package com.mopub.unity;

import android.support.annotation.NonNull;
import android.util.Log;

import com.mopub.common.MoPubReward;
import com.mopub.mobileads.MoPubErrorCode;
import com.mopub.mobileads.MoPubRewardedVideoListener;

import java.util.Set;

/**
 * Singleton class to handle Rewarded Video events, as this is the one ad format to follow a
 * singleton pattern and we need to manage these events outside of the plugin lifecycle.
 */
public class MoPubRewardedVideoUnityPluginManager implements MoPubRewardedVideoListener {
    private static String TAG = "MoPub";
    private static volatile MoPubRewardedVideoUnityPluginManager sInstance;

    private MoPubRewardedVideoUnityPluginManager() {}

    static MoPubRewardedVideoUnityPluginManager getInstance() {
        if (sInstance == null) {
            synchronized (MoPubRewardedVideoUnityPluginManager.class) {
                if (sInstance == null) {
                    sInstance = new MoPubRewardedVideoUnityPluginManager();
                }
            }
        }
        return sInstance;
    }

    @Override
    public void onRewardedVideoLoadSuccess(@NonNull final String adUnitId) {
        MoPubUnityPlugin.UnityEvent.RewardedVideoLoaded.Emit(adUnitId);
    }

    @Override
    public void onRewardedVideoLoadFailure(@NonNull final String adUnitId,
                                           @NonNull final MoPubErrorCode errorCode) {
        if (errorCode == MoPubErrorCode.EXPIRED)
            MoPubUnityPlugin.UnityEvent.RewardedVideoExpired.Emit(adUnitId);
        else
            MoPubUnityPlugin.UnityEvent.RewardedVideoFailed.Emit(adUnitId, errorCode.toString());
    }

    @Override
    public void onRewardedVideoStarted(@NonNull final String adUnitId) {
        MoPubUnityPlugin.UnityEvent.RewardedVideoShown.Emit(adUnitId);
    }

    @Override
    public void onRewardedVideoClicked(@NonNull final String adUnitId) {
        MoPubUnityPlugin.UnityEvent.RewardedVideoClicked.Emit(adUnitId);
    }

    @Override
    public void onRewardedVideoPlaybackError(@NonNull final String adUnitId,
                                             @NonNull final MoPubErrorCode errorCode) {
        MoPubUnityPlugin.UnityEvent.RewardedVideoFailedToPlay.Emit(adUnitId, errorCode.toString());
    }

    @Override
    public void onRewardedVideoClosed(@NonNull final String adUnitId) {
        MoPubUnityPlugin.UnityEvent.RewardedVideoClosed.Emit(adUnitId);
    }

    @Override
    public void onRewardedVideoCompleted(@NonNull final Set<String> adUnitIds,
                                         @NonNull final MoPubReward reward) {
        if (adUnitIds.size() == 0 || reward == null) {
            Log.e(TAG, "Rewarded ad completed without ad unit ID and/or reward.");
            return;
        }

        for (String adUnitId : adUnitIds) {
            MoPubUnityPlugin.UnityEvent.RewardedVideoReceivedReward.Emit(adUnitId,
                    reward.getLabel(), String.valueOf(reward.getAmount()));
        }
    }
}

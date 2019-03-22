package com.mopub.unity;

import android.location.Location;
import android.support.annotation.NonNull;
import android.support.annotation.Nullable;
import android.util.Log;

import com.mopub.common.MoPubReward;
import com.mopub.common.Preconditions;
import com.mopub.mobileads.MoPubErrorCode;
import com.mopub.mobileads.MoPubRewardedVideoListener;
import com.mopub.mobileads.MoPubRewardedVideoManager;
import com.mopub.mobileads.MoPubRewardedVideos;

import java.util.Locale;
import java.util.Set;


/**
 * Provides an API that bridges the Unity Plugin with the MoPub Rewarded Ad SDK.
 */
public class MoPubRewardedVideoUnityPlugin extends MoPubUnityPlugin
        implements MoPubRewardedVideoListener {

    /**
     * Creates a {@link MoPubRewardedVideoUnityPlugin} for the given ad unit ID.
     *
     * @param adUnitId String for the ad unit ID to use for this rewarded video.
     */
    public MoPubRewardedVideoUnityPlugin(final String adUnitId) {
        super(adUnitId);
    }


    /* ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** *****
     * Rewarded Ads API                                                                        *
     * ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** *****/

    /**
     * Loads a rewarded ad for the current ad unit ID and the given mediation settings,
     * keywords, latitude, longitude and customer ID. Personally Identifiable Information (PII)
     * should ONLY be present in the userDataKeywords field.
     *
     * Options for mediation settings for each network are as follows on Android:
     *  {
     *      "adVendor": "AdColony",
     *      "withConfirmationDialog": false,
     *      "withResultsDialog": true
     *  }
     *  {
     *      "adVendor": "Chartboost",
     *      "customId": "the-user-id"
     *  }
     *  {
     *      "adVendor": "Vungle",
     *      "userId": "the-user-id",
     *      "cancelDialogBody": "Cancel Body",
     *      "cancelDialogCloseButton": "Shut it Down",
     *      "cancelDialogKeepWatchingButton": "Watch On",
     *      "cancelDialogTitle": "Cancel Title"
     *  }
     * See https://www.mopub.com/resources/docs/unity-engine-integration/#RewardedVideo for more
     * details and sample helper methods to generate mediation settings.
     *
     * @param json String with JSON containing third-party network specific settings.
     * @param keywords String with comma-separated key:value pairs of non-PII keywords.
     * @param userDataKeywords String with comma-separated key:value pairs of PII keywords.
     * @param latitude double with the desired latitude.
     * @param longitude double with the desired longitude.
     * @param customerId String with the customer ID.
     */
    public void requestRewardedVideo(final String json, final String keywords,
            @Nullable final String userDataKeywords, final double latitude, final double longitude,
            final String customerId) {
        runSafelyOnUiThread(new Runnable() {
            public void run() {
                Location location = new Location("");
                location.setLatitude(latitude);
                location.setLongitude(longitude);

                MoPubRewardedVideoManager.RequestParameters requestParameters =
                        new MoPubRewardedVideoManager.RequestParameters(
                                keywords, userDataKeywords, location, customerId);

                MoPubRewardedVideos.setRewardedVideoListener(MoPubRewardedVideoUnityPlugin.this);

                if (json != null) {
                    MoPubRewardedVideos.loadRewardedVideo(
                            mAdUnitId, requestParameters, extractMediationSettingsFromJson(json, true));
                } else {
                    MoPubRewardedVideos.loadRewardedVideo(mAdUnitId, requestParameters);
                }
            }
        });
    }

    /**
     * Whether there is a rewarded ad ready to play or not.
     *
     * @return true if there is a rewarded ad loaded and ready to play; false otherwise.
     */
    public boolean hasRewardedVideo() {
        return MoPubRewardedVideos.hasRewardedVideo(mAdUnitId);
    }

    /**
     * Takes over the screen and shows rewarded ad, if one is loaded and ready to play.
     *
     * @param customData String with optional custom data for the Rewarded Ad.
     */
    public void showRewardedVideo(@Nullable final String customData) {
        runSafelyOnUiThread(new Runnable() {
            public void run() {
                if (!MoPubRewardedVideos.hasRewardedVideo(mAdUnitId)) {
                    Log.i(TAG, String.format(Locale.US,
                            "No rewarded ad is available at this time."));
                    return;
                }

                MoPubRewardedVideos.setRewardedVideoListener(MoPubRewardedVideoUnityPlugin.this);
                MoPubRewardedVideos.showRewardedVideo(mAdUnitId, customData);
            }
        });
    }

    /**
     * Retrieves the list of available {@link MoPubReward}s for the current ad unit ID.
     *
     * @return an array with the available {@link MoPubReward}s.
     */
    public MoPubReward[] getAvailableRewards() {
        Set<MoPubReward> rewardsSet = MoPubRewardedVideos.getAvailableRewards(mAdUnitId);

        Log.i(TAG, String.format(Locale.US, "%d MoPub rewards available", rewardsSet.size()));

        return rewardsSet.toArray(new MoPubReward[rewardsSet.size()]);
    }

    /**
     * Specifies which reward should be given to the user on video completion.
     *
     * @param selectedReward a {@link MoPubReward} to reward the user with.
     */
    public void selectReward(@NonNull MoPubReward selectedReward) {
        Preconditions.checkNotNull(selectedReward);

        Log.i(TAG, String.format(Locale.US, "Selected reward \"%d %s\"",
                selectedReward.getAmount(),
                selectedReward.getLabel()));

        MoPubRewardedVideos.selectReward(mAdUnitId, selectedReward);
    }


    /* ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** *****
     * RewardedVideoListener implementation                                                    *
     * ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** *****/

    @Override
    public void onRewardedVideoLoadSuccess(String adUnitId) {
        if (mAdUnitId.equals(adUnitId)) {
            UnityEvent.RewardedVideoLoaded.Emit(adUnitId);
        }
    }

    @Override
    public void onRewardedVideoLoadFailure(String adUnitId, MoPubErrorCode errorCode) {
        if (mAdUnitId.equals(adUnitId)) {
            if (errorCode == MoPubErrorCode.EXPIRED)
                UnityEvent.RewardedVideoExpired.Emit(adUnitId);
            else
                UnityEvent.RewardedVideoFailed.Emit(adUnitId, errorCode.toString());
        }
    }

    @Override
    public void onRewardedVideoStarted(String adUnitId) {
        if (mAdUnitId.equals(adUnitId)) {
            UnityEvent.RewardedVideoShown.Emit(adUnitId);
        }
    }

    @Override
    public void onRewardedVideoClicked(@NonNull String adUnitId) {
        if (mAdUnitId.equals(adUnitId)) {
            UnityEvent.RewardedVideoClicked.Emit(adUnitId);
        }
    }

    @Override
    public void onRewardedVideoPlaybackError(String adUnitId, MoPubErrorCode errorCode) {
        if (mAdUnitId.equals(adUnitId)) {
            UnityEvent.RewardedVideoFailedToPlay.Emit(mAdUnitId, errorCode.toString());
        }
    }

    @Override
    public void onRewardedVideoClosed(String adUnitId) {
        if (mAdUnitId.equals(adUnitId)) {
            UnityEvent.RewardedVideoClosed.Emit(adUnitId);
        }
    }

    @Override
    public void onRewardedVideoCompleted(Set<String> adUnitIds, MoPubReward reward) {
        if (adUnitIds.size() == 0 || reward == null) {
            Log.e(TAG, String.format(Locale.US,
                    "Rewarded ad completed without ad unit ID and/or reward."));
            return;
        }

        String adUnitId = adUnitIds.toArray()[0].toString();
        if (mAdUnitId.equals(adUnitId)) {
            UnityEvent.RewardedVideoReceivedReward.Emit(adUnitId, reward.getLabel(),
                    String.valueOf(reward.getAmount()));
        }
    }
}







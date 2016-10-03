package com.mopub.unity;

import android.location.Location;
import android.util.Log;

import com.mopub.common.MediationSettings;
import com.mopub.common.MoPubReward;
import com.mopub.mobileads.MoPubErrorCode;
import com.mopub.mobileads.MoPubRewardedVideoListener;
import com.mopub.mobileads.MoPubRewardedVideoManager;
import com.mopub.mobileads.MoPubRewardedVideos;
import com.unity3d.player.UnityPlayer;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.lang.reflect.Constructor;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.Set;


public class MoPubRewardedVideoUnityPlugin extends MoPubUnityPlugin
        implements MoPubRewardedVideoListener {

    private static boolean sRewardedVideoInitialized;

    private static final String CHARTBOOST_MEDIATION_SETTINGS =
            "com.mopub.mobileads.ChartboostRewardedVideo$ChartboostMediationSettings";
    private static final String VUNGLE_MEDIATION_SETTINGS =
            "com.mopub.mobileads.VungleRewardedVideo$VungleMediationSettings$Builder";
    private static final String ADCOLONY_MEDIATION_SETTINGS =
            "com.mopub.mobileads.AdColonyRewardedVideo$AdColonyInstanceMediationSettings";


    public MoPubRewardedVideoUnityPlugin(final String adUnitId) {
        super(adUnitId);
    }


    /* ***** ***** ***** ***** ***** ***** ***** *****
     * Rewarded Videos API
     */
    private MediationSettings[] extractMediationSettingsFromJson(String json) {
        ArrayList<MediationSettings> settings = new ArrayList<MediationSettings>();

        try {
            JSONArray jsonArray = new JSONArray(json);
            for (int i = 0; i < jsonArray.length(); i++) {
                JSONObject jsonObj = jsonArray.getJSONObject(i);
                String adVendor = jsonObj.getString("adVendor");
                Log.i(TAG, "adding MediationSettings for ad vendor: " + adVendor);

                if (adVendor.equalsIgnoreCase("chartboost")) {
                    if (jsonObj.has("customId")) {
                        try {
                            Class<?> mediationSettingsClass =
                                    Class.forName(CHARTBOOST_MEDIATION_SETTINGS);
                            Constructor<?> mediationSettingsConstructor =
                                    mediationSettingsClass.getConstructor(String.class);
                            MediationSettings s =
                                    (MediationSettings) mediationSettingsConstructor
                                            .newInstance(jsonObj.getString("customId"));
                            settings.add(s);
                        } catch (ClassNotFoundException e) {
                            Log.i(TAG, "could not find ChartboostMediationSettings class. " +
                                    "Did you add Chartboost Network SDK to your Android folder?");
                            printExceptionStackTrace(e);
                        } catch (InstantiationException e) {
                            printExceptionStackTrace(e);
                        } catch (NoSuchMethodException e) {
                            printExceptionStackTrace(e);
                        } catch (IllegalAccessException e) {
                            printExceptionStackTrace(e);
                        } catch (IllegalArgumentException e) {
                            printExceptionStackTrace(e);
                        } catch (InvocationTargetException e) {
                            printExceptionStackTrace(e);
                        }
                    } else {
                        Log.i(TAG, "No customId key found in the settings object. " +
                                "Aborting adding Chartboost MediationSettings");
                    }
                } else if (adVendor.equalsIgnoreCase("vungle")) {
                    try {
                        Class<?> builderClass = Class.forName(VUNGLE_MEDIATION_SETTINGS);
                        Constructor<?> builderConstructor = builderClass.getConstructor();
                        Object b = builderConstructor.newInstance();

                        Method withUserId =
                                builderClass.getDeclaredMethod("withUserId",
                                        String.class);
                        Method withCancelDialogBody =
                                builderClass.getDeclaredMethod("withCancelDialogBody",
                                        String.class);
                        Method withCancelDialogCloseButton =
                                builderClass.getDeclaredMethod("withCancelDialogCloseButton",
                                        String.class);
                        Method withCancelDialogKeepWatchingButton =
                                builderClass.getDeclaredMethod("withCancelDialogKeepWatchingButton",
                                        String.class);
                        Method withCancelDialogTitle =
                                builderClass.getDeclaredMethod("withCancelDialogTitle",
                                        String.class);
                        Method build = builderClass.getDeclaredMethod("build");

                        if (jsonObj.has("userId")) {
                            withUserId.invoke(b, jsonObj.getString("userId"));
                        }

                        if (jsonObj.has("cancelDialogBody")) {
                            withCancelDialogBody.invoke(b, jsonObj.getString("cancelDialogBody"));
                        }

                        if (jsonObj.has("cancelDialogCloseButton")) {
                            withCancelDialogCloseButton
                                    .invoke(b, jsonObj.getString("cancelDialogCloseButton"));
                        }

                        if (jsonObj.has("cancelDialogKeepWatchingButton")) {
                            withCancelDialogKeepWatchingButton
                                    .invoke(b, jsonObj.getString("cancelDialogKeepWatchingButton"));
                        }

                        if (jsonObj.has("cancelDialogTitle")) {
                            withCancelDialogTitle.invoke(b, jsonObj.getString("cancelDialogTitle"));
                        }

                        settings.add((MediationSettings) build.invoke(b));

                    } catch (ClassNotFoundException e) {
                        Log.i(TAG, "could not find VungleMediationSettings class. " +
                                "Did you add Vungle Network SDK to your Android folder?");
                        printExceptionStackTrace(e);
                    } catch (InstantiationException e) {
                        printExceptionStackTrace(e);
                    } catch (NoSuchMethodException e) {
                        printExceptionStackTrace(e);
                    } catch (IllegalAccessException e) {
                        printExceptionStackTrace(e);
                    } catch (IllegalArgumentException e) {
                        printExceptionStackTrace(e);
                    } catch (InvocationTargetException e) {
                        printExceptionStackTrace(e);
                    }
                } else if (adVendor.equalsIgnoreCase("adcolony")) {
                    if (jsonObj.has("withConfirmationDialog") && jsonObj.has("withResultsDialog")) {
                        boolean withConfirmationDialog =
                                jsonObj.getBoolean("withConfirmationDialog");
                        boolean withResultsDialog =
                                jsonObj.getBoolean("withResultsDialog");

                        try {
                            Class<?> mediationSettingsClass =
                                    Class.forName(ADCOLONY_MEDIATION_SETTINGS);
                            Constructor<?> mediationSettingsConstructor =
                                    mediationSettingsClass
                                            .getConstructor(boolean.class, boolean.class);
                            MediationSettings s =
                                    (MediationSettings) mediationSettingsConstructor
                                            .newInstance(withConfirmationDialog, withResultsDialog);
                            settings.add(s);
                        } catch (ClassNotFoundException e) {
                            Log.i(TAG, "could not find AdColonyInstanceMediationSettings class. " +
                                    "Did you add AdColony Network SDK to your Android folder?");
                            printExceptionStackTrace(e);
                        } catch (InstantiationException e) {
                            printExceptionStackTrace(e);
                        } catch (NoSuchMethodException e) {
                            printExceptionStackTrace(e);
                        } catch (IllegalAccessException e) {
                            printExceptionStackTrace(e);
                        } catch (IllegalArgumentException e) {
                            printExceptionStackTrace(e);
                        } catch (InvocationTargetException e) {
                            printExceptionStackTrace(e);
                        }
                    }
                } else {
                    Log.e(TAG, "adVendor not available for custom mediation settings: " +
                            "[" + adVendor + "]");
                }
            }
        } catch (JSONException e) {
            printExceptionStackTrace(e);
        }

        return settings.toArray(new MediationSettings[settings.size()]);
    }

    public static void initializeRewardedVideo() {
        runSafelyOnUiThread(new Runnable() {
            public void run() {
                if (!sRewardedVideoInitialized) {
                    MoPubRewardedVideos.initializeRewardedVideo(getActivity());
                    sRewardedVideoInitialized = true;
                }
            }
        });
    }

    public void requestRewardedVideo(final String json, final String keywords,
            final double latitude, final double longitude, final String customerId) {
        runSafelyOnUiThread(new Runnable() {
            public void run() {
                Location location = new Location("");
                location.setLatitude(latitude);
                location.setLongitude(longitude);

                MoPubRewardedVideoManager.RequestParameters requestParameters =
                        new MoPubRewardedVideoManager.RequestParameters(
                                keywords, location, customerId);

                MoPubRewardedVideos.setRewardedVideoListener(MoPubRewardedVideoUnityPlugin.this);

                if (json != null) {
                    MoPubRewardedVideos.loadRewardedVideo(
                            mAdUnitId, requestParameters, extractMediationSettingsFromJson(json));
                } else {
                    MoPubRewardedVideos.loadRewardedVideo(mAdUnitId, requestParameters);
                }
            }
        });
    }

    public void showRewardedVideo() {
        if (!MoPubRewardedVideos.hasRewardedVideo(mAdUnitId)) {
            Log.i(TAG, "no rewarded video is available at this time");
            return;
        }

        runSafelyOnUiThread(new Runnable() {
            public void run() {
                MoPubRewardedVideos.setRewardedVideoListener(MoPubRewardedVideoUnityPlugin.this);
                MoPubRewardedVideos.showRewardedVideo(mAdUnitId);
            }
        });
    }


    /* ***** ***** ***** ***** ***** ***** ***** *****
     * RewardedVideoListener implementation
     */
    @Override
    public void onRewardedVideoLoadSuccess(String adUnitId) {
        if (mAdUnitId.equals(adUnitId)) {
            UnityPlayer.UnitySendMessage("MoPubManager", "onRewardedVideoLoaded", adUnitId);
        }
    }

    @Override
    public void onRewardedVideoLoadFailure(String adUnitId, MoPubErrorCode errorCode) {
        if (mAdUnitId.equals(adUnitId)) {
            String errorMsg =
                    String.format("adUnitId = %s, errorCode = %s", adUnitId, errorCode.toString());
            Log.i(TAG, "onRewardedVideoLoadFailure: " + errorMsg);
            UnityPlayer.UnitySendMessage("MoPubManager", "onRewardedVideoFailed", errorMsg);
        }
    }

    @Override
    public void onRewardedVideoStarted(String adUnitId) {
        if (mAdUnitId.equals(adUnitId)) {
            UnityPlayer.UnitySendMessage("MoPubManager", "onRewardedVideoShown", adUnitId);
        }
    }

    @Override
    public void onRewardedVideoPlaybackError(String adUnitId, MoPubErrorCode errorCode) {
        if (mAdUnitId.equals(adUnitId)) {
            String errorMsg =
                    String.format("adUnitId = %s, errorCode = %s", adUnitId, errorCode.toString());
            Log.i(TAG, "onRewardedVideoPlaybackError: " + errorMsg);
            UnityPlayer.UnitySendMessage("MoPubManager", "onRewardedVideoFailedToPlay", errorMsg);
        }
    }

    @Override
    public void onRewardedVideoClosed(String adUnitId) {
        if (mAdUnitId.equals(adUnitId)) {
            UnityPlayer.UnitySendMessage("MoPubManager", "onRewardedVideoClosed", adUnitId);
        }
    }

    @Override
    public void onRewardedVideoCompleted(Set<String> adUnitIds, MoPubReward reward) {
        if (adUnitIds.size() == 0 || reward == null) {
            Log.i(TAG, "onRewardedVideoCompleted with no adUnitId and/or reward. Bailing out.");
            return;
        }

        String adUnitId = adUnitIds.toArray()[0].toString();
        if (mAdUnitId.equals(adUnitId)) {
            try {
                JSONObject json = new JSONObject();
                json.put("adUnitId", adUnitId);
                json.put("currencyType", "");
                json.put("amount", reward.getAmount());

                UnityPlayer.UnitySendMessage(
                        "MoPubManager", "onRewardedVideoReceivedReward", json.toString());
            } catch (JSONException e) {
                e.printStackTrace();
            }
        }
    }
}







package com.mopub.unity;

import android.app.Activity;
import android.util.Log;

import com.mopub.common.MoPub;
import com.mopub.mobileads.MoPubConversionTracker;
import com.unity3d.player.UnityPlayer;

import java.io.PrintWriter;
import java.io.StringWriter;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;


public class MoPubUnityPlugin {
    protected static String TAG = "MoPub";
    protected final String mAdUnitId;

    public MoPubUnityPlugin(final String adUnitId) {
        mAdUnitId = adUnitId;
    }


    /* ***** ***** ***** ***** ***** ***** ***** *****
     * Helper Methods
     */
    protected static Activity getActivity() {
        return UnityPlayer.currentActivity;
    }

    protected static void runSafelyOnUiThread(final Runnable runner) {
        getActivity().runOnUiThread(new Runnable() {
            @Override
            public void run() {
                try {
                    runner.run();
                } catch (Exception e) {
                    e.printStackTrace();
                }
            }
        });
    }

    protected static void printExceptionStackTrace(Exception e) {
        StringWriter sw = new StringWriter();
        e.printStackTrace(new PrintWriter(sw));
        Log.i(TAG, sw.toString());
    }


    /* ***** ***** ***** ***** ***** ***** ***** *****
     * Test Settings API
     */
    public static void addFacebookTestDeviceId(String hashedDeviceId) {
        try {
            Class<?> cls = Class.forName("com.facebook.ads.AdSettings");
            Method method = cls.getMethod("addTestDevice", new Class[]{String.class});
            method.invoke(cls, hashedDeviceId);
            Log.i(TAG, "successfully added Facebook test device: " + hashedDeviceId);
        } catch (ClassNotFoundException e) {
            Log.i(TAG, "could not find Facebook AdSettings class. " +
                    "Did you add the Audience Network SDK to your Android folder?");
        }
        //AdSettings.addtestdevice
        catch (NoSuchMethodException e) {
            Log.i(TAG, "could not find Facebook AdSettings.addTestDevice method. " +
                    "Did you add the Audience Network SDK to your Android folder?");
        } catch (IllegalAccessException e) {
            e.printStackTrace();
        } catch (IllegalArgumentException e) {
            e.printStackTrace();
        } catch (InvocationTargetException e) {
            e.printStackTrace();
        }
    }


    /* ***** ***** ***** ***** ***** ***** ***** *****
     * App Open API
     */
    public static void reportApplicationOpen() {
        runSafelyOnUiThread(new Runnable() {
            public void run() {
                new MoPubConversionTracker().reportAppOpen(getActivity());
            }
        });
    }


    /* ***** ***** ***** ***** ***** ***** ***** *****
     * Location Awareness API
     */
    public static void setLocationAwareness(final String locationAwareness) {
        runSafelyOnUiThread(new Runnable() {
            public void run() {
                MoPub.setLocationAwareness(MoPub.LocationAwareness.valueOf(locationAwareness));
            }
        });
    }
}

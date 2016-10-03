package com.mopub.unity;

import android.util.DisplayMetrics;
import android.util.Log;
import android.view.Gravity;
import android.view.View;
import android.widget.FrameLayout;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;

import com.mopub.mobileads.MoPubErrorCode;
import com.mopub.mobileads.MoPubView;
import com.unity3d.player.UnityPlayer;


public class MoPubBannerUnityPlugin extends MoPubUnityPlugin implements MoPubView.BannerAdListener {
    private MoPubView mMoPubView;
    private RelativeLayout mLayout;

    public MoPubBannerUnityPlugin(final String adUnitId) {
        super(adUnitId);
    }

    private static float getScreenDensity() {
        DisplayMetrics metrics = new DisplayMetrics();
        getActivity().getWindowManager().getDefaultDisplay().getMetrics(metrics);

        return metrics.density;
    }


    /* ***** ***** ***** ***** ***** ***** ***** *****
     * Banners API
     */
    private void prepLayout(int alignment) {
        // create a RelativeLayout and add the ad view to it
        if (mLayout == null) {
            mLayout = new RelativeLayout(getActivity());
        } else {
            // remove the layout if it has a parent
            FrameLayout parentView = (FrameLayout) mLayout.getParent();
            if (parentView != null)
                parentView.removeView(mLayout);
        }

        int gravity = 0;

        switch (alignment) {
            case 0:
                gravity = Gravity.TOP | Gravity.LEFT;
                break;
            case 1:
                gravity = Gravity.TOP | Gravity.CENTER_HORIZONTAL;
                break;
            case 2:
                gravity = Gravity.TOP | Gravity.RIGHT;
                break;
            case 3:
                gravity = Gravity.CENTER_VERTICAL | Gravity.CENTER_HORIZONTAL;
                break;
            case 4:
                gravity = Gravity.BOTTOM | Gravity.LEFT;
                break;
            case 5:
                gravity = Gravity.BOTTOM | Gravity.CENTER_HORIZONTAL;
                break;
            case 6:
                gravity = Gravity.BOTTOM | Gravity.RIGHT;
                break;
        }

        mLayout.setGravity(gravity);
    }

    public void createBanner(final int alignment) {
        runSafelyOnUiThread(new Runnable() {
            public void run() {
                if (mMoPubView != null)
                    return;

                mMoPubView = new MoPubView(getActivity());
                mMoPubView.setAdUnitId(mAdUnitId);
                mMoPubView.setBannerAdListener(MoPubBannerUnityPlugin.this);
                mMoPubView.loadAd();

                prepLayout(alignment);

                mLayout.addView(mMoPubView);
                getActivity().addContentView(mLayout,
                        new LinearLayout.LayoutParams(
                                LinearLayout.LayoutParams.FILL_PARENT,
                                LinearLayout.LayoutParams.FILL_PARENT));

                mLayout.setVisibility(RelativeLayout.VISIBLE);
            }
        });
    }

    public void hideBanner(final boolean shouldHide) {
        if (mMoPubView == null)
            return;

        runSafelyOnUiThread(new Runnable() {
            public void run() {
                if (shouldHide) {
                    mMoPubView.setVisibility(View.GONE);
                } else {
                    mMoPubView.setVisibility(View.VISIBLE);
                }
            }
        });
    }

    public void setBannerKeywords(final String keywords) {
        runSafelyOnUiThread(new Runnable() {
            public void run() {
                if (mMoPubView == null)
                    return;

                mMoPubView.setKeywords(keywords);
                mMoPubView.loadAd();
            }
        });
    }

    public void destroyBanner() {
        runSafelyOnUiThread(new Runnable() {
            public void run() {
                if (mMoPubView == null || mLayout == null)
                    return;

                mLayout.removeAllViews();
                mLayout.setVisibility(LinearLayout.GONE);
                mMoPubView.destroy();
                mMoPubView = null;
            }
        });
    }


    /* ***** ***** ***** ***** ***** ***** ***** *****
     * BannerAdListener implementation
     */
    @Override
    public void onBannerLoaded(MoPubView banner) {
        UnityPlayer.UnitySendMessage(
                "MoPubManager", "onAdLoaded", String.valueOf(banner.getAdHeight()));

        // re-center the ad
        int height = mMoPubView.getAdHeight();
        int width = mMoPubView.getAdWidth();
        float density = getScreenDensity();

        RelativeLayout.LayoutParams params =
                (RelativeLayout.LayoutParams) mMoPubView.getLayoutParams();
        params.width = (int) (width * density);
        params.height = (int) (height * density);

        mMoPubView.setLayoutParams(params);
    }

    @Override
    public void onBannerFailed(MoPubView banner, MoPubErrorCode errorCode) {
        String errorMsg =
                String.format("adUnitId = %s, errorCode = %s", mAdUnitId, errorCode.toString());
        Log.i(TAG, "onAdFailed: " + errorMsg);
        UnityPlayer.UnitySendMessage("MoPubManager", "onAdFailed", errorMsg);
    }

    @Override
    public void onBannerClicked(MoPubView banner) {
        UnityPlayer.UnitySendMessage("MoPubManager", "onAdClicked", mAdUnitId);
    }

    @Override
    public void onBannerExpanded(MoPubView banner) {
        UnityPlayer.UnitySendMessage("MoPubManager", "onAdExpanded", mAdUnitId);
    }

    @Override
    public void onBannerCollapsed(MoPubView banner) {
        UnityPlayer.UnitySendMessage("MoPubManager", "onAdCollapsed", mAdUnitId);
    }
}

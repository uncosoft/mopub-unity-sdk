package com.mopub.unity;

import android.view.Gravity;
import android.view.View;
import android.widget.FrameLayout;
import android.widget.LinearLayout;
import android.widget.RelativeLayout;

import androidx.annotation.Nullable;

import com.mopub.mobileads.MoPubErrorCode;
import com.mopub.mobileads.MoPubView;
import com.mopub.mobileads.MoPubView.MoPubAdSize;

/**
 * Provides an API that bridges the Unity Plugin with the MoPub Banner SDK.
 */
public class MoPubBannerUnityPlugin extends MoPubUnityPlugin implements MoPubView.BannerAdListener {
    private MoPubView mMoPubView;
    private RelativeLayout mLayout;

    /**
     * Creates a {@link MoPubBannerUnityPlugin} for the given ad unit ID.
     *
     * @param adUnitId String for the ad unit ID to use for this banner.
     */
    public MoPubBannerUnityPlugin(final String adUnitId) {
        super(adUnitId);
    }

    @Override
    public boolean isPluginReady() {
        return mMoPubView != null;
    }

    /* ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** *****
     * Banners API                                                                             *
     * ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** *****/

    /**
     * Requests, loads and shows a banner with the given alignment and size for the current ad unit
     * ID, if it doesn't exist already.
     *
     * Valid alignment values are:
     *  0 - top left
     *  1 - top center
     *  2 - top right
     *  3 - center
     *  4 - bottom left
     *  5 - bottom center
     *  6 - bottom right
     *
     * @param width float for the maximum width, in dp, for the ad
     * @param height float for the maximum height, in dp, for the ad
     * @param alignment int for the desired alignment for the requested banner.
     * @param keywords String with comma-separated key:value pairs of non-PII keywords.
     * @param userDataKeywords String with comma-separated key:value pairs of PII keywords.
     */
    public void requestBanner(final float width, final float height, final int alignment,
                              final String keywords, final String userDataKeywords) {
        if (alreadyRequested())
            return;

        runSafelyOnUiThread(new Runnable() {
            public void run() {
                mMoPubView = new MoPubView(getActivity());
                mMoPubView.setAdUnitId(mAdUnitId);
                mMoPubView.setKeywords(keywords);
                mMoPubView.setUserDataKeywords(userDataKeywords);
                mMoPubView.setBannerAdListener(MoPubBannerUnityPlugin.this);

                mMoPubView.loadAd(MoPubAdSize.valueOf((int) height));

                prepLayout(alignment);
                mLayout.addView(mMoPubView);
                getActivity().addContentView(mLayout,
                        new LinearLayout.LayoutParams(
                                LinearLayout.LayoutParams.MATCH_PARENT,
                                LinearLayout.LayoutParams.MATCH_PARENT));

                mLayout.setVisibility(RelativeLayout.VISIBLE);
            }
        });
    }

    /**
     * Shows or hides the current banner.
     *
     * @param shouldHide hides the banner if true; shows the banner if false.
     */
    public void hideBanner(final boolean shouldHide) {
        if (!isPluginReady())
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

    /**
     * Sets the given keywords for the current banner and then reloads it. Personally
     * Identifiable Information (PII) should ONLY be passed via
     * {@link #refreshBanner(String)}
     *
     * @param keywords String with comma-separated key:value pairs of non-PII keywords.
     */
    public void refreshBanner(final String keywords) {
        refreshBanner(keywords, null);
    }

    /**
     * Sets the given keywords for the current banner and then reloads it. Personally
     * Identifiable Information (PII) should ONLY be present in the userDataKeywords field.
     *
     * @param keywords String with comma-separated key:value pairs of non-PII keywords.
     * @param userDataKeywords String with comma-separated key:value pairs of PII keywords.
     */
    public void refreshBanner(final String keywords, @Nullable final String userDataKeywords) {
        if (!isPluginReady())
            return;

        runSafelyOnUiThread(new Runnable() {
            public void run() {
                mMoPubView.setKeywords(keywords);
                mMoPubView.setUserDataKeywords(userDataKeywords);
                mMoPubView.loadAd();
            }
        });
    }

    /**
     * Removes the current banner from the view and destroys it.
     */
    public void destroyBanner() {
        if (!isPluginReady() || mLayout == null)
            return;

        runSafelyOnUiThread(new Runnable() {
            public void run() {
                mLayout.removeAllViews();
                mLayout.setVisibility(LinearLayout.GONE);
                mMoPubView.destroy();
                mMoPubView = null;
            }
        });
    }



    public void setAutorefreshEnabled(boolean enabled) {
        if (isPluginReady())
            mMoPubView.setAutorefreshEnabled(enabled);
    }


    public void forceRefresh() {
        if (isPluginReady())
            mMoPubView.forceRefresh();
    }


    /* ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** *****
     * BannerAdListener implementation                                                         *
     * ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** *****/

    @Override
    public void onBannerLoaded(MoPubView banner) {
        UnityEvent.AdLoaded.Emit(mAdUnitId, String.valueOf(banner.getAdWidth()),
                String.valueOf(banner.getAdHeight()));
    }

    @Override
    public void onBannerFailed(MoPubView banner, MoPubErrorCode errorCode) {
        UnityEvent.AdFailed.Emit(mAdUnitId, errorCode.toString());
    }

    @Override
    public void onBannerClicked(MoPubView banner) {
        UnityEvent.AdClicked.Emit(mAdUnitId);
    }

    @Override
    public void onBannerExpanded(MoPubView banner) {
        UnityEvent.AdExpanded.Emit(mAdUnitId);
    }

    @Override
    public void onBannerCollapsed(MoPubView banner) {
        UnityEvent.AdCollapsed.Emit(mAdUnitId);
    }


    /* ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** *****
     * Private helpers                                                                         *
     * ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** ***** *****/

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

    // Redundant method for better readability.
    private boolean alreadyRequested() {
        return isPluginReady();
    }
}

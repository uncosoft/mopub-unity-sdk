using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;


public class MoPubAndroidBanner
{
    private readonly AndroidJavaObject _bannerPlugin;


    public MoPubAndroidBanner(string adUnitId)
    {
        _bannerPlugin = new AndroidJavaObject("com.mopub.unity.MoPubBannerUnityPlugin", adUnitId);
    }


    [SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
    public void RequestBanner(float width, float height, MoPub.AdPosition position)
    {
        _bannerPlugin.Call("requestBanner", width, height, (int) position);
    }


    [SuppressMessage("ReSharper", "AccessToStaticMemberViaDerivedType")]
    [Obsolete("CreateBanner is deprecated and will be removed soon, please use RequestBanner instead.")]
    public void CreateBanner(MoPub.AdPosition position)
    {
        _bannerPlugin.Call("createBanner", (int) position);
    }


    public void ShowBanner(bool shouldShow)
    {
        _bannerPlugin.Call("hideBanner", !shouldShow);
    }


    public void RefreshBanner(string keywords, string userDataKeywords = "")
    {
        _bannerPlugin.Call("refreshBanner", keywords, userDataKeywords);
    }


    public void DestroyBanner()
    {
        _bannerPlugin.Call("destroyBanner");
    }


    public void SetAutorefresh(bool enabled)
    {
        _bannerPlugin.Call("setAutorefreshEnabled", enabled);
    }


    public void ForceRefresh()
    {
        _bannerPlugin.Call("forceRefresh");
    }
}

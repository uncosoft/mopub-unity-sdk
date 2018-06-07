using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MoPubInternal.ThirdParty.MiniJSON;
using UnityEngine;

/// <summary>
/// This class provides common classes and utitilies needed across platforms
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class MoPubBase
{
    public enum AdPosition
    {
        TopLeft,
        TopCenter,
        TopRight,
        Centered,
        BottomLeft,
        BottomCenter,
        BottomRight
    }


    public static class Consent
    {
        /// <summary>
        /// User's consent for providing personal tracking data for ad tailoring.
        /// </summary>
        /// <remarks>
        /// The enum values match the iOS SDK enum.
        /// </remarks>
        public enum Status
        {
            /// <summary>
            /// Status is unknown. Either the status is currently updating or the SDK initialization has not completed.
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// Consent is denied.
            /// </summary>
            Denied,

            /// <summary>
            /// Advertiser tracking is disabled.
            /// </summary>
            DoNotTrack,

            /// <summary>
            /// Your app has attempted to grant consent on the user's behalf, but your whitelist status is not verfied
            /// with the ad server.
            /// </summary>
            PotentialWhitelist,

            /// <summary>
            /// User has consented.
            /// </summary>
            Consented
        }


        // The Android SDK uses these strings to indicate consent status.
        private static class Strings
        {
            public const string ExplicitYes = "explicit_yes";
            public const string ExplicitNo = "explicit_no";
            public const string Unknown = "unknown";
            public const string PotentialWhitelist = "potential_whitelist";
            public const string Dnt = "dnt";
        }


        // Helper string to convert Android SDK consent status strings to our consent enum.
        // Also handles integer values.
        public static Status FromString(string status)
        {
            switch (status) {
                case Strings.ExplicitYes:
                    return Status.Consented;
                case Strings.ExplicitNo:
                    return Status.Denied;
                case Strings.Dnt:
                    return Status.DoNotTrack;
                case Strings.PotentialWhitelist:
                    return Status.PotentialWhitelist;
                case Strings.Unknown:
                    return Status.Unknown;
                default:
                    try {
                        return (Status) Enum.Parse(typeof(Status), status);
                    }
                    catch {
                        Debug.LogError("Unknown consent status string: " + status);
                        return Status.Unknown;
                    }
            }
        }
    }


    // Currently used only for iOS
    public enum BannerType
    {
        Size320x50,
        Size300x250,
        Size728x90,
        Size160x600
    }


    // Currently used only for iOS
    public enum LogLevel
    {
        MPLogLevelAll = 0,
        MPLogLevelTrace = 10,
        MPLogLevelDebug = 20,
        MPLogLevelInfo = 30,
        MPLogLevelWarn = 40,
        MPLogLevelError = 50,
        MPLogLevelFatal = 60,
        MPLogLevelOff = 70
    }


    /// <summary>
    /// Data object holding any SDK initialization parameters.
    /// </summary>
    public struct SdkConfiguration
    {
        /// <summary>
        /// Any ad unit that your app uses.
        /// </summary>
        public string AdUnitId;

        /// <summary>
        /// List of the class names of advanced bidders to initialize.
        /// </summary>
        public AdvancedBidder[] AdvancedBidders;

        /// <summary>
        /// Used for rewarded video initialization. This holds each custom event's unique settings.
        /// </summary>
        public MediationSetting[] MediationSettings;

        /// <summary>
        /// List of class names of rewarded video custom events to initialize. These classes must extend
        /// CustomEventRewardedVideo (in the respective Android/iOS native SDKs).
        /// </summary>
        public RewardedNetwork[] NetworksToInit;


        public string AdvancedBiddersString {
            get { return AdvancedBidders != null ?
                             string.Join(",", AdvancedBidders.Select(b => b.ToString()).ToArray()) : string.Empty; }
        }

        public string MediationSettingsJson {
            get { return MediationSettings != null ? Json.Serialize(MediationSettings) : string.Empty; }
        }

        public string NetworksToInitString {
            get { return NetworksToInit != null ?
                             string.Join(",", NetworksToInit.Select(b => b.ToString()).ToArray()) : string.Empty; }
        }
    }


    public class MediationSetting : Dictionary<string, object>
    {
        public MediationSetting(string adVendor) { Add("adVendor", adVendor); }

        // Shortcut class names so you don't have to remember the right ad vendor string (also to not misspell it).
        public class AdColony : MediationSetting { public AdColony() : base("AdColony") { } }
#if UNITY_IOS
        public class AdMob : MediationSetting { public AdMob() : base("MPGoogle") { } }
#else
        public class AdMob : MediationSetting { public AdMob() : base("GooglePlayServices") { } }
#endif
        public class Chartboost : MediationSetting { public Chartboost() : base("Chartboost") { } }
        public class Vungle : MediationSetting { public Vungle() : base("Vungle") { } }
    }


    public struct Reward
    {
        public string Label;
        public int Amount;


        public override string ToString()
        {
            return string.Format("\"{0} {1}\"", Amount, Label);
        }


        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Label) && Amount > 0;
        }
    }


    public abstract class ThirdPartyNetwork
    {
        private readonly string _name;


        protected ThirdPartyNetwork(string name)
        {
#if UNITY_ANDROID
            _name = "com.mopub.mobileads." + name;
#else
            _name = name;
#endif
        }


        public override string ToString()
        {
            return _name;
        }
    }


    public class AdvancedBidder : ThirdPartyNetwork
    {
        private AdvancedBidder(string name) : base(name) { }


        private static AdvancedBidder GetBidder(string network)
        {
            network += "AdvancedBidder";
            return new AdvancedBidder(network);
        }


        public static readonly AdvancedBidder AdColony = GetBidder("AdColony");
        public static readonly AdvancedBidder Facebook = GetBidder("Facebook");
    }


    public class RewardedNetwork : ThirdPartyNetwork
    {
        private readonly string _name;


        private RewardedNetwork(string name) : base(name) { }


        private static RewardedNetwork GetNetwork(string network)
        {
            network += "RewardedVideo";
#if UNITY_IOS
            network += "CustomEvent";
#endif
            return new RewardedNetwork(network);
        }


        public static readonly RewardedNetwork AdColony = GetNetwork("AdColony");
        public static readonly RewardedNetwork AdMob = // Names are different for Android vs iOS
#if UNITY_IOS
            GetNetwork("MPGoogleAdMob");
#else
            GetNetwork("GooglePlayServices");
#endif
        public static readonly RewardedNetwork Chartboost = GetNetwork("Chartboost");
        public static readonly RewardedNetwork Facebook = GetNetwork("Facebook");
        public static readonly RewardedNetwork Tapjoy = GetNetwork("Tapjoy");
        public static readonly RewardedNetwork Unity = // Names are different for Android vs iOS
#if UNITY_IOS
            GetNetwork("UnityAds");
#else
            GetNetwork("Unity");
#endif
        public static readonly RewardedNetwork Vungle = GetNetwork("Vungle");
    }


    /// <summary>
    /// Set this to an ISO language code (e.g., "en-US") if you wish the next two URL properties to point
    /// to a web resource that is localized to a specific language.
    /// </summary>
    public static string ConsentLanguageCode { get; set; }


    public const double LatLongSentinel = 99999.0;


    private static string _pluginName;

    public static string PluginName {
        get { return _pluginName ?? (_pluginName = "MoPub Unity Plugin v" + Application.version); }
    }


    protected static void ValidateAdUnitForSdkInit(string adUnitId)
    {
        if (string.IsNullOrEmpty(adUnitId))
            Debug.LogError("A valid ad unit ID is needed to initialize the MoPub SDK.");
    }


    protected static void ReportAdUnitNotFound(string adUnitId)
    {
        Debug.LogWarning(string.Format("AdUnit {0} not found: no plugin was initialized", adUnitId));
    }


    protected static Uri UrlFromString(string url)
    {
        if (String.IsNullOrEmpty(url)) return null;
        try {
            return new Uri(url);
        } catch {
            Debug.LogError("Invalid URL: " + url);
            return null;
        }
    }


    // Allocate the MoPubManager singleton, which receives all callback events from the native SDKs.
    protected static void InitManager()
    {
        var type = typeof(MoPubManager);
        var mgr = new GameObject("MoPubManager", type).GetComponent<MoPubManager>(); // Its Awake() method sets Instance.
        if (MoPubManager.Instance != mgr)
            Debug.LogWarning(
                "It looks like you have the " + type.Name
                + " on a GameObject in your scene. Please remove the script from your scene.");
    }


    protected MoPubBase() { }
}

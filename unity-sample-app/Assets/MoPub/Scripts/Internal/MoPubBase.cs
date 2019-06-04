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


    public enum LogLevel
    {
        MPLogLevelDebug = 20,
        MPLogLevelInfo = 30,
        MPLogLevelNone = 70
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
        /// Used for rewarded video initialization. This holds each custom event's unique settings.
        /// </summary>
        public MediatedNetwork[] MediatedNetworks;

        /// <summary>
        /// Allow supported SDK networks to collect user information on the basis of legitimate interest.
        /// </summary>
        public bool AllowLegitimateInterest;

        /// <summary>
        /// MoPub SDK log level. Defaults to MoPub.<see cref="MoPubBase.LogLevel.MPLogLevelNone"/>
        /// </summary>
        public LogLevel LogLevel
        {
            get { return _logLevel != 0 ? _logLevel : LogLevel.MPLogLevelNone; }
            set { _logLevel = value; }
        }

        private LogLevel _logLevel;


        public string AdditionalNetworksString
        {
            get {
                var cn = from n in MediatedNetworks ?? Enumerable.Empty<MediatedNetwork>()
                         where n is MediatedNetwork && !(n is SupportedNetwork)
                         where !string.IsNullOrEmpty(n.AdapterConfigurationClassName)
                         select n.AdapterConfigurationClassName;
                return string.Join(",", cn.ToArray());
            }
        }


        public string NetworkConfigurationsJson
        {
            get {
                var nc = from n in MediatedNetworks ?? Enumerable.Empty<MediatedNetwork>()
                         where n.NetworkConfiguration != null
                         where !string.IsNullOrEmpty(n.AdapterConfigurationClassName)
                         select n;
                return Json.Serialize(nc.ToDictionary(n => n.AdapterConfigurationClassName,
                                                      n => n.NetworkConfiguration));
            }
        }

        public string MediationSettingsJson
        {
            get {
                var ms = from n in MediatedNetworks ?? Enumerable.Empty<MediatedNetwork>()
                         where n.MediationSettings != null
                         where !string.IsNullOrEmpty(n.MediationSettingsClassName)
                         select n;
                return Json.Serialize(ms.ToDictionary(n => n.MediationSettingsClassName,
                                                      n => n.MediationSettings));
            }
        }


        public string MoPubRequestOptionsJson
        {
            get {
                var ro = from n in MediatedNetworks ?? Enumerable.Empty<MediatedNetwork>()
                         where n.MoPubRequestOptions != null
                         where !string.IsNullOrEmpty(n.AdapterConfigurationClassName)
                         select n;
                return Json.Serialize(ro.ToDictionary(n => n.AdapterConfigurationClassName,
                                                      n => n.MoPubRequestOptions));
            }
        }
    }


    public class LocalMediationSetting : Dictionary<string, object>
    {
        public string MediationSettingsClassName { get; set; }

        public LocalMediationSetting() { }

        public LocalMediationSetting(string adVendor)
        {
#if UNITY_IOS
            MediationSettingsClassName = adVendor + "InstanceMediationSettings";
#else
            MediationSettingsClassName = "com.mopub.mobileads." + adVendor + "RewardedVideo$" + adVendor + "MediationSettings";
#endif
        }

        public LocalMediationSetting(string android, string ios) :
#if UNITY_IOS
            this(ios)
#else
            this(android)
#endif
            {}


        public static string ToJson(IEnumerable<LocalMediationSetting> localMediationSettings)
        {
            var ms = from n in localMediationSettings ?? Enumerable.Empty<LocalMediationSetting>()
                     where n != null && !string.IsNullOrEmpty(n.MediationSettingsClassName)
                     select n;
            return Json.Serialize(ms.ToDictionary(n => n.MediationSettingsClassName, n => n));
        }


        // Shortcut class names so you don't have to remember the right ad vendor string (also to not misspell it).
        public class AdColony : LocalMediationSetting { public AdColony() : base("AdColony") {
#if UNITY_ANDROID
                MediationSettingsClassName = "com.mopub.mobileads.AdColonyRewardedVideo$AdColonyInstanceMediationSettings";
#endif
            }
        }
        public class AdMob      : LocalMediationSetting { public AdMob()      : base(android: "GooglePlayServices",
                                                                                     ios:     "MPGoogle") { } }
        public class Chartboost : LocalMediationSetting { public Chartboost() : base("Chartboost") { } }
        public class Vungle     : LocalMediationSetting { public Vungle()     : base("Vungle") { } }
    }


    // Data structure to register and initialize a mediated network.
    public class MediatedNetwork
    {
        public string AdapterConfigurationClassName { get; set; }
        public string MediationSettingsClassName    { get; set; }

        public Dictionary<string,object> NetworkConfiguration { get; set; }
        public Dictionary<string,object> MediationSettings    { get; set; }
        public Dictionary<string,object> MoPubRequestOptions  { get; set; }
    }


    // Networks that are supported by MoPub.
    public class SupportedNetwork : MediatedNetwork
    {
        protected SupportedNetwork(string adVendor)
        {
#if UNITY_IOS
            AdapterConfigurationClassName = adVendor + "AdapterConfiguration";
            MediationSettingsClassName    = adVendor + "GlobalMediationSettings";
#else
            AdapterConfigurationClassName = "com.mopub.mobileads." + adVendor + "AdapterConfiguration";
            MediationSettingsClassName    = "com.mopub.mobileads." + adVendor + "RewardedVideo$" + adVendor + "MediationSettings";
#endif
        }

        public class AdColony   : SupportedNetwork { public AdColony()   : base("AdColony") {
#if UNITY_ANDROID
               MediationSettingsClassName = "com.mopub.mobileads.AdColonyRewardedVideo$AdColonyGlobalMediationSettings";
#endif
            }
        }
        public class AdMob      : SupportedNetwork { public AdMob()      : base("GooglePlayServices") {
#if UNITY_IOS
               AdapterConfigurationClassName = "GoogleAdMobAdapterConfiguration";
               MediationSettingsClassName    = "MPGoogleGlobalMediationSettings";
#endif
            }
        }
        public class AppLovin   : SupportedNetwork { public AppLovin()   : base("AppLovin") { } }
        public class Chartboost : SupportedNetwork { public Chartboost() : base("Chartboost") { } }
        public class Facebook   : SupportedNetwork { public Facebook()   : base("Facebook") { } }
        public class IronSource : SupportedNetwork { public IronSource() : base("IronSource") { } }
        public class OnebyAOL   : SupportedNetwork { public OnebyAOL()   : base("Millennial") { } }
        public class Tapjoy     : SupportedNetwork { public Tapjoy()     : base("Tapjoy") { } }
        public class Unity      : SupportedNetwork { public Unity()      : base("UnityAds") { } }
        public class Verizon    : SupportedNetwork { public Verizon()    : base("Verizon") { } }
        public class Vungle     : SupportedNetwork { public Vungle()     : base("Vungle") { } }
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


    public struct ImpressionData
    {
        public string AdUnitId;
        public string AdUnitName;
        public string AdUnitFormat;
        public string ImpressionId;
        public string Currency;
        public double? PublisherRevenue;
        public string AdGroupId;
        public string AdGroupName;
        public string AdGroupType;
        public int? AdGroupPriority;
        public string Country;
        public string Precision;
        public string NetworkName;
        public string NetworkPlacementId;
        public string JsonRepresentation;

        public static ImpressionData FromJson(string json)
        {
            var impData = new ImpressionData();
            if (string.IsNullOrEmpty(json)) return impData;

            var fields = Json.Deserialize(json) as Dictionary<string, object>;
            if (fields == null) return impData;

            object obj;
            double parsedDouble;
            int parsedInt;

            if (fields.TryGetValue("adunit_id", out obj))
                impData.AdUnitId = obj.ToString();

            if (fields.TryGetValue("adunit_name", out obj))
                impData.AdUnitName = obj.ToString();

            if (fields.TryGetValue("adunit_format", out obj))
                impData.AdUnitFormat = obj.ToString();

            if (fields.TryGetValue("id", out obj))
                impData.ImpressionId = obj.ToString();

            if (fields.TryGetValue("currency", out obj))
                impData.Currency = obj.ToString();

            if (fields.TryGetValue("publisher_revenue", out obj)
                && double.TryParse(obj.ToString(), out parsedDouble))
                impData.PublisherRevenue = parsedDouble;

            if (fields.TryGetValue("adgroup_id", out obj))
                impData.AdGroupId = obj.ToString();

            if (fields.TryGetValue("adgroup_name", out obj))
                impData.AdGroupName = obj.ToString();

            if (fields.TryGetValue("adgroup_type", out obj))
                impData.AdGroupType = obj.ToString();

            if (fields.TryGetValue("adgroup_priority", out obj)
                && int.TryParse(obj.ToString(), out parsedInt))
                impData.AdGroupPriority = parsedInt;

            if (fields.TryGetValue("country", out obj))
                impData.Country = obj.ToString();

            if (fields.TryGetValue("precision", out obj))
                impData.Precision = obj.ToString();

            if (fields.TryGetValue("network_name", out obj))
                impData.NetworkName = obj.ToString();

            if (fields.TryGetValue("network_placement_id", out obj))
                impData.NetworkPlacementId = obj.ToString();

            impData.JsonRepresentation = json;

            return impData;
        }
    }


    /// <summary>
    /// Set this to an ISO language code (e.g., "en-US") if you wish the next two URL properties to point
    /// to a web resource that is localized to a specific language.
    /// </summary>
    public static string ConsentLanguageCode { get; set; }


    public const double LatLongSentinel = 99999.0;


    public static readonly string moPubSDKVersion = "5.7.1";
    private static string _pluginName;
    private static bool _allowLegitimateInterest;
    public static LogLevel logLevel { get; protected set; }

    public static string PluginName {
        get { return _pluginName ?? (_pluginName = "MoPub Unity Plugin v" + moPubSDKVersion); }
    }


    /// <summary>
    /// Compares two versions to see which is greater.
    /// </summary>
    /// <param name="a">Version to compare against second param</param>
    /// <param name="b">Version to compare against first param</param>
    /// <returns>-1 if the first version is smaller, 1 if the first version is greater, 0 if they are equal</returns>
    public static int CompareVersions(string a, string b)
    {
        var versionA = VersionStringToInts(a);
        var versionB = VersionStringToInts(b);
        for (var i = 0; i < Mathf.Max(versionA.Length, versionB.Length); i++) {
            if (VersionPiece(versionA, i) < VersionPiece(versionB, i))
                return -1;
            if (VersionPiece(versionA, i) > VersionPiece(versionB, i))
                return 1;
        }

        return 0;
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


    private static int VersionPiece(IList<int> versionInts, int pieceIndex)
    {
        return pieceIndex < versionInts.Count ? versionInts[pieceIndex] : 0;
    }


    private static int[] VersionStringToInts(string version)
    {
        int piece;
        return version.Split('.').Select(v => int.TryParse(v, out piece) ? piece : 0).ToArray();
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

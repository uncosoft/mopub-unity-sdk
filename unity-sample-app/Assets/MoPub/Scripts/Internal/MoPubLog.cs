using System;
using System.Collections.Generic;
using UnityEngine;
// ReSharper disable AccessToStaticMemberViaDerivedType

public static class MoPubLog
{
    public static class SdkLogEvent
    {
        public const string InitStarted = "SDK initialization started";
        public const string InitFinished = "SDK initialized and ready to display ads.  Log Level: {0}";
    }

    public static class ConsentLogEvent
    {
        public const string Updated = "Consent changed to {0} from {1}: PII can{2} be collected. Reason: {3}";
        public const string LoadAttempted = "Attempting to load consent dialog";
        public const string LoadSuccess = "Consent dialog loaded";
        public const string LoadFailed = "Consent dialog failed: ({0}) {1}";
        public const string ShowAttempted = "Consent dialog attempting to show";
        public const string ShowSuccess = "Sucessfully showed consent dialog";

    }

    public static class AdLogEvent
    {
        public const string LoadAttempted = "Attempting to load ad";
        public const string LoadSuccess = "Ad loaded";
        public const string LoadFailed = "Ad failed to load: ({0}) {1}";
        public const string ShowAttempted = "Attempting to show ad";
        public const string ShowSuccess = "Ad shown";
        public const string Tapped = "Ad tapped";
        public const string Expanded = "Ad expanded";
        public const string Collapsed = "Ad collapsed";
        public const string Dismissed = "Ad did disappear";
        public const string ShouldReward = "Ad should reward user with {0} {1}";
        public const string Expired = "Ad expired since it was not shown within {0} minutes of it being loaded";
    }

    private static readonly Dictionary<string, MoPubBase.LogLevel> logLevelMap =
        new Dictionary<string, MoPubBase.LogLevel>
    {
        { SdkLogEvent.InitStarted, MoPub.LogLevel.MPLogLevelDebug },
        { SdkLogEvent.InitFinished, MoPub.LogLevel.MPLogLevelInfo },
        { ConsentLogEvent.Updated, MoPub.LogLevel.MPLogLevelDebug },
        { ConsentLogEvent.LoadAttempted, MoPub.LogLevel.MPLogLevelDebug },
        { ConsentLogEvent.LoadSuccess, MoPub.LogLevel.MPLogLevelDebug },
        { ConsentLogEvent.LoadFailed, MoPub.LogLevel.MPLogLevelDebug },
        { ConsentLogEvent.ShowAttempted, MoPub.LogLevel.MPLogLevelDebug },
        { ConsentLogEvent.ShowSuccess, MoPub.LogLevel.MPLogLevelDebug },
        { AdLogEvent.LoadAttempted, MoPub.LogLevel.MPLogLevelInfo },
        { AdLogEvent.LoadSuccess, MoPub.LogLevel.MPLogLevelInfo },
        { AdLogEvent.LoadFailed, MoPub.LogLevel.MPLogLevelInfo },
        { AdLogEvent.ShowAttempted, MoPub.LogLevel.MPLogLevelInfo },
        { AdLogEvent.ShowSuccess, MoPub.LogLevel.MPLogLevelInfo },
        { AdLogEvent.Tapped, MoPub.LogLevel.MPLogLevelDebug },
        { AdLogEvent.Expanded, MoPub.LogLevel.MPLogLevelDebug },
        { AdLogEvent.Collapsed, MoPub.LogLevel.MPLogLevelDebug },
        { AdLogEvent.Dismissed, MoPub.LogLevel.MPLogLevelDebug },
        { AdLogEvent.ShouldReward, MoPub.LogLevel.MPLogLevelDebug },
        { AdLogEvent.Expired, MoPub.LogLevel.MPLogLevelDebug },
    };

    public static void Log(string callerMethod, string message, params object[] args)
    {
        MoPubBase.LogLevel messageLogLevel;
        if (!logLevelMap.TryGetValue(message, out messageLogLevel))
            messageLogLevel = MoPubBase.LogLevel.MPLogLevelDebug;

        if (MoPub.logLevel > messageLogLevel) return;

        var formattedMessage = "[MoPub-Unity] [" + callerMethod + "] " + message;
        try {
            Debug.LogFormat(formattedMessage, args);
        } catch (FormatException) {
            Debug.Log(formattedMessage);
        }
    }
}

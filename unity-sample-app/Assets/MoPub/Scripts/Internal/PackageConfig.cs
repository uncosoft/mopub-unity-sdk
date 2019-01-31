using System.Collections.Generic;

public abstract class PackageConfig
{


    public enum Platform
    {
        ANDROID, IOS
    }


    /// <summary>
    /// The name of the Network Adapter in this package.
    /// </summary>
    /// <remarks>Should never be null.</remarks>
    public abstract string Name { get; }


    /// <summary>
    /// A dot-separated version number such as "5", "5.0", or "5.0.5".
    /// </summary>
    /// <remarks>Should never be null.</remarks>
    public abstract string Version { get; }


    /// <summary>
    /// The Network SDK version this adapter supports for each platform.
    /// </summary>
    /// <remarks>Should never be null.</remarks>
    public abstract Dictionary<Platform, string> NetworkSdkVersions { get; }


    /// <summary>
    /// Class names for Adapter Custom Events for each platform.
    /// </summary>
    /// <remarks>Should never be null.</remarks>
    public abstract Dictionary<Platform, string> AdapterClassNames { get; }


    /// <summary>
    /// The Network SDK version this adapter supports for the current platform.
    /// </summary>
    public string NetworkSdkVersion
    {
        get {
            return NetworkSdkVersions[
#if UNITY_ANDROID
                Platform.ANDROID
#else
                Platform.IOS
#endif
            ];
        }
    }


    /// <summary>
    /// Class name for Adapter Custom Event for the current platform.
    /// </summary>
    public string AdapterClassName
    {
        get {
            return AdapterClassNames[
#if UNITY_ANDROID
                Platform.ANDROID
#else
                Platform.IOS
#endif
            ];
        }
    }
}

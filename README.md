# MoPub Unity SDK

Thanks for taking a look at MoPub! We take pride in having an easy-to-use, flexible monetization solution that works across multiple platforms.

Sign up for an account at [http://app.mopub.com/](http://app.mopub.com/).

## Need Help?

To get started visit our [Unity Engine Integration](https://www.mopub.com/resources/docs/unity-engine-integration/) guide and find additional help documentation on our [developer help site](http://dev.twitter.com/mopub).

To file an issue with our team please email [support@mopub.com](mailto:support@mopub.com).

## New in This Version (5.14.1 - October 5, 2020)

Please view the [MoPub Unity SDK changelog](https://github.com/mopub/mopub-unity-sdk/blob/master/CHANGELOG.md), [MoPub Android SDK changelog](https://github.com/mopub/mopub-android-sdk/blob/master/CHANGELOG.md), and [MoPub iOS SDK changelog](https://github.com/mopub/mopub-ios-sdk/blob/master/CHANGELOG.md) for a complete list of additions, fixes, and enhancements across releases and platforms.

- **Features**
  - The MoPub Unity Plugin now includes version `5.14.0` of the MoPub Android SDK and version `5.14.1` of the iOS SDK.
  - Added beta support for OMSDK version 1.3.4.
  - Added background event for real-time impression tracking: `OnImpressionTrackedEventBg`. Please see [our Publisher Docs](https://developers.mopub.com/publishers/unity/impression-data/#register-for-the-impression-event) for details.
  - Added ability to include `keywords` and `userDataKeywords` when requesting banners.
  - The Android SDK dependency is now managed by the External Dependency Manger. Please see [upgrade note below](https://github.com/mopub/mopub-unity-sdk#upgrading-to-sdk-514).
  - Added Pangle to `MoPub.SupportedNetwork` class.
  - Removed Mintegral from `MoPub.SupportedNetwork` class.

- **Bug Fixes**
  - Fixed an issue with interstitial loading causing crashes on Unity 2020. Please see [requirements note below](https://github.com/mopub/mopub-unity-sdk#additional-requirements-for-unity-2020) if you are using Unity 2020.
  - Fixed issue in SDK Manager when handling deprecated networks.

## Upgrading to SDK 5.14

After upgrading to the `5.14.1` Plugin, use the *Migrate* button in the *SDK Manager* dialog to remove the Android SDK, as this dependency is now managed by the External Dependency Manager. The removed components are:
* `mopub-sdk-base.aar`
* `mopub-sdk-banner.aar`
* `mopub-sdk-fullscreen.aar`
* `mopub-sdk-native-static.aar`
* `mopub-volley-2.0.0.jar`

Also, starting from version `5.14.1` the minimum supported Grade Tools version is `3.4.0`.

### Additional Requirements for Unity 2020+

To prevent issues with your MoPub integration on Android, please ensure your Unity 2020 application has the following gradle properties:
```
android.useAndroidX=true
android.enableDexingArtifactTransform=false
```
If you have further issues, ensure multidex has been enabled as well.

For more details on the above, please refer to the following documentation:
* [Gradle for Android on Unity Manual](https://docs.unity3d.com/2020.1/Documentation/Manual/android-gradle-overview.html)
* [Multidex on Android Studio User Guide](https://developer.android.com/studio/build/multidex)

## Upgrading to SDK 5.13

After upgrading to the `5.13` Plugin, use the *Migrate* button in the *SDK Manager* dialog to remove the deprecated Android SDK components: `mopub-sdk-interstitial.aar` and `mopub-sdk-rewardedvideo.aar`. These have been replaced by `mopub-sdk-fullscreen.aar`.

## Upgrading to SDK 5.8

After upgrading to the `5.8` Plugin, use the *Migrate* button in the SDK Manager dialog to remove the old `.jar` files for the Android SDK components. These are now included as `.aar` files, located one directory up in `Assets/MoPub/Plugins/Android`.

Starting in the `5.8` Plugin you can use the new `MoPubManager` prefab to set up the SDK initialization and GDPR consent status management in the Unity editor. Drag the prefab into your project's starting scene and then customize the fields the same way you would have filled in a `MoPub.SdkConfiguration` object in code. By default, the prefab will call `MoPub.SdkInitialize` in its `Start()` method. Therefore, you can delete this call from your own code. You can disable that automatic call if you need to control the time of the call time, by clearing the *Auto Initialize on Start* checkbox. In that case, you can still configure using the prefab, and access the resulting `MoPub.SdkConfiguration` object via the `MoPubManager.Instance.SdkConfiguration` property. Just call `MoPub.SdkInitialize()` with that value at the time of your choosing.

If you need to run custom runtime logic to add or edit the contents of the `SdkConfiguration` object before it is passed to `MoPub.SdkInitialize()`, you can add a script to the prefab that implements the `OnSdkConfiguration(SdkConfiguration config)` method. This function will be called via `BroadcastMessage` when the `SdkConfiguration` property is accessed, and you can edit the config object in place.

The latest versions of all of the supported networks' Unity adapters come with a `NetworkConfiguration` script that adds further UI to the `MoPubManager` prefab for setting network options (both adapter network config values and global mediation settings). The inspector panel for the `MoPubManager` script has a drop down menu that lists available scripts to add to the prefab. (If you don't see one of the networks in the menu, you may need to update to the latest unitypackage -- use the *SDK Manager* to do this.)  Each field of one of these scripts has an enable/disable checkbox that activates the field for entering a value. Note that values entered here take precedence over the same value that might come from the MoPub dashboard, so only activate the fields which are not available via the dashboard, or which are never changed, or which you want to override for testing purposes.

To support testing, the `MoPubManager` prefab comes with two `MoPubManager` scripts, one on the root game object and one on a child object. The root script is for production use, and the child object script is for QA/testing builds. This one is disabled by default. If you enable it, then it overrides the configuration of the prod script when the app is run. There is a button on the inspector for the test manager that copies settings from the prod manager, to save time getting started.

The `MoPubManager` prefab has `UnityEvent` fields you can use to hook into each of the SDK events, rather than using the underlying C# events directly via code. This supports using the editor to set up your callbacks.

The `MoPubConsent` script on the prefab manages the GDPR consent status and dialog. If the *Auto Show Consent Dialog checkbox* is enabled, the script will automatically load and show the GDPR consent dialog, so you don't need code to set up that logic anymore. (This only works with the stock MoPub consent dialog.)  It also contains a field for setting the *Location Awareness Usage* string that normally has to go in the Xcode project's `info.plist` file for iOS builds.

## Upgrading to SDK 5.6

Starting in MoPub Unity Plugin `5.6`, the MoPub iOS SDK is no longer included as a bundled framework and is instead specified as a dependency via podspecs. Upgrading from previous versions will leave the previously bundled iOS SDK framework, which needs to be removed to avoid collisions with the podspec dependency.

To address this, please delete the following directory from your project prior to building for iOS with the MoPub Unity Plugin `5.6`: `Assets/MoPub/Plugins/iOS/MoPubSDKFramework.framework`.

## Upgrading to SDK 5.4

Starting in MoPub Unity Plugin `5.4`, the SDK Manager (opened via the previously-beta MoPub menu) automatically detects if there are directories or files in the legacy plugin structure, and displays a *Migrate* button.
NOTE: Performing the migration is optional as it simply organizes all MoPub code within the same directory, and doing it (or not) should not have any adverse effect.

for more details, see https://developers.mopub.com/docs/unity/getting-started/#migrating-to-54

## Upgrading to SDK 5.0

Please see the [Getting Started Guide](https://developers.mopub.com/docs/unity/getting-started/) for instructions on upgrading from SDK `4.X` to SDK `5.0`.

For GDPR-specific upgrading instructions, also see the [GDPR Integration Guide](https://developers.mopub.com/docs/publisher/gdpr-guide/).

## Developing on the MoPub Unity Plugin

### Cloning the project
```
git clone https://github.com/mopub/mopub-unity-sdk
git submodule init
git submodule update
```

### Repository structure

* `mopub-android-sdk/` - Git submodule of the MoPub Android SDK
* `mopub-android-sdk-unity/` - Android wrapper, contains a project that adds Unity-specific files to the Android SDK
* `mopub-ios-sdk/` - Git submodule of the MoPub iOS SDK
* `unity-sample-app/Assets/MoPub/Plugins/iOS` - iOS wrapper code to interface with the iOS SDK
* `unity-sample-app/` - Contains MoPub Unity Plugin sample project and MoPub developer environment
* `mopub-unity-plugin/` - Where the Unity packages are exported after running `./unity-export-package.sh`

### Prerequisities
Before you can build the plugin per the instructions below, you must do the following:
* Place any third-party SDKs and dependencies in their corresponding directories, per README files in:
  * `mopub-android-sdk-unity/libs/` - Android wrapper dependencies
  * `unity/MoPubUnityPlugin/Assets/Plugins/Android/` - Android plugin dependencies
  * iOS loads dependencies at runtime, so there's no need to add them prior to building
* Set up the Unity IDE:
  * Make sure you are logged in to your Unity account
  * Open the Unity Plugin project (under the `unity/` directory), open Build Settings and Switch Platform to either Android or iOS
  * Close the Unity IDE

### How do I build?

NOTE: Building is only needed for development of the MoPub Unity SDK; for MoPub SDK integration, please see our [Unity Engine Integration](https://www.mopub.com/resources/docs/unity-engine-integration/) guide.

Simply run [`./scripts/build.sh`](https://github.com/mopub/mopub-unity-sdk/blob/master/scripts/build.sh) (make sure the Unity IDE is *not* running), which builds the Android wrapper, copies it to the sample project, and then exports a unity package with everything needed for a successful MoPub integration.

Exporting the unity package can also be done manually, by opening the `unity/` project in Unity, right-clicking the `Assets/` folder and chosing `Export Package...`.

### How do I run the sample unity project and test?

After building per instructions above, open the `unity-sample-app/` project in Unity, click `File > Build Settings...`, select iOS or Android, click `Build and Run`.

## License

The MoPub SDK License can be found at [http://www.mopub.com/legal/sdk-license-agreement/](http://www.mopub.com/legal/sdk-license-agreement/).

## Open Measurement License

We have partnered with the IAB to provide Viewability measurement via the Open Measurement SDK as of version 5.14.0. To view the full license, visit [https://www.mopub.com/en/omlv1](https://www.mopub.com/en/omlv1)

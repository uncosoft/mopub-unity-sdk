# MoPub Unity SDK

Thanks for taking a look at MoPub! We take pride in having an easy-to-use, flexible monetization solution that works across multiple platforms.

Sign up for an account at [http://app.mopub.com/](http://app.mopub.com/).

## Need Help?

To get started visit our [Unity Engine Integration](https://www.mopub.com/resources/docs/unity-engine-integration/) guide and find additional help documentation on our [developer help site](http://dev.twitter.com/mopub).

To file an issue with our team please email [support@mopub.com](mailto:support@mopub.com).

## New in This Version (5.4.0 - October 9, 2018)
## Version 5.4.0 (October 9, 2018)
- The MoPub Unity Plugin now includes versions 5.4.0 of the MoPub Android SDK and the MoPub iOS SDK.
- The MoPub menu now has more features: About, Documentation, Report Issue, and SDK Manager
- The SDK Manager dialog allows checking for and updating to new versions of the SDK.
- The SDK directory structure has changed:  All files are now collected under Assets/MoPub.  See below for notes on migration.

Please view the [MoPub Unity SDK changelog](https://github.com/mopub/mopub-unity-sdk/blob/master/CHANGELOG.md), [MoPub Android SDK changelog](https://github.com/mopub/mopub-android-sdk/blob/master/CHANGELOG.md), and [MoPub iOS SDK changelog](https://github.com/mopub/mopub-ios-sdk/blob/master/CHANGELOG.md) for a complete list of additions, fixes, and enhancements across releases and platforms.

## Upgrading to SDK 5.4

Starting in MoPub Unity Plugin 5.4, the SDK Manager (opened via the previously-beta MoPub menu) automatically detects if there are directories or files in the legacy plugin structure, and displays a “Migrate” button.
NOTE: Performing the migration is optional as it simply organizes all MoPub code within the same directory, and doing it (or not) should not have any adverse effect.

for more details, see https://developers.mopub.com/docs/unity/getting-started/#migrating-to-54

## Upgrading to SDK 5.0

Please see the [Getting Started Guide](https://developers.mopub.com/docs/unity/getting-started/) for instructions on upgrading from SDK 4.X to SDK 5.0.

For GDPR-specific upgrading instructions, also see the [GDPR Integration Guide](https://developers.mopub.com/docs/publisher/gdpr-guide/).

## License

The MoPub SDK License can be found at [http://www.mopub.com/legal/sdk-license-agreement/](http://www.mopub.com/legal/sdk-license-agreement/).

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
* `mopub-ios-sdk-unity/` - iOS wrapper, contains a project that adds Unity-specific files to the iOS SDK
* `unity-sample-app/` - Contains MoPub Unity Plugin sample project
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

Simply run [`./scripts/build.sh`](https://github.com/mopub/mopub-unity-sdk/blob/master/scripts/build.sh) (make sure the Unity IDE is *not* running), which runs `git submodule update` and then invokes the following scripts:

* [`scripts/mopub-android-sdk-unity-build.sh`](https://github.com/mopub/mopub-unity-sdk/blob/master/scripts/mopub-android-sdk-unity-build.sh) - builds the mopub-android-sdk-unity project and copies the resulting artifacts into `unity/`
* [`scripts/mopub-ios-sdk-unity-build.sh`](https://github.com/mopub/mopub-unity-sdk/blob/master/scripts/mopub-ios-sdk-unity-build.sh) - builds the mopub-ios-sdk-unity project and copies the resulting artifacts into `unity/`
* [`scripts/unity-export-package.sh`](https://github.com/mopub/mopub-unity-sdk/blob/master/scripts/unity-export-package.sh)  - exports the unity package into `mopub-unity-plugin/`

Each script can be invoked separately. Exporting the unity package can also be done manually, by opening the `unity/` project in Unity, right-clicking the `Assets/` folder and chosing `Export Package...`.

### How do I run the sample unity project and test?

After building per instructions above, open the `unity/` project in Unity, click `File > Build Settings...`, select iOS or Android, click `Build and Run`.

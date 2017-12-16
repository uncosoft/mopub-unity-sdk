# MoPub Unity SDK

Thanks for taking a look at MoPub! We take pride in having an easy-to-use, flexible monetization solution that works across multiple platforms.

Sign up for an account at [http://app.mopub.com/](http://app.mopub.com/).

## Need Help?

To get started visit our [Unity Engine Integration](https://www.mopub.com/resources/docs/unity-engine-integration/) guide and find additional help documentation on our [developer help site](http://dev.twitter.com/mopub).

To file an issue with our team please email [support@mopub.com](mailto:support@mopub.com).

## New in This Version (4.19.0 - December 15, 2017)
- Bug fixes.
- The MoPub Unity Plugin is now compatible with version 4.19.0 of the MoPub Android SDK and version 4.19.0 of the MoPub iOS SDK.

Please view the [changelog](https://github.com/mopub/mopub-unity-sdk/blob/master/CHANGELOG.md) for a complete list of additions, fixes, and enhancements in all releases.

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
* `unity/` - Contains the Unity Plugin
* `mopub-unity-plugin/` - Where the Unity packages are exported after running `./unity-export-package.sh`

### Prerequisities
Before you can build the plugin per the instructions below, you must do the following:
* Place any third-party SDKs and dependencies in their corresponding directories, per README files in:
  * `mopub-android-sdk-unity/libs/` - Android wrapper dependencies
  * `mopub-android-sdk-unity/[each network]-custom-events/libs/` - Android wrapper third-party networks
  * `unity/MoPubUnityPlugin/Assets/Plugins/Android/` - Android third-party AARs
  * `unity/MoPubUnityPlugin/Assets/Plugins/Android/mopub-support/libs/[each network]/` - Android third-party JARs
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

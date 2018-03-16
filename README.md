# MoPub Unity SDK

Thanks for taking a look at MoPub! We take pride in having an easy-to-use, flexible monetization solution that works across multiple platforms.

Sign up for an account at [http://app.mopub.com/](http://app.mopub.com/).

## Need Help?

To get started visit our [Unity Engine Integration](https://www.mopub.com/resources/docs/unity-engine-integration/) guide and find additional help documentation on our [developer help site](http://dev.twitter.com/mopub).

To file an issue with our team please email [support@mopub.com](mailto:support@mopub.com).

## New in This Version (4.20.2 - March 16, 2018)
- Fixed an issue with banners being occluded by the notch in iPhone X; banners (regardless of platform or positioning) are now restricted to the device's safe area.
- We are formally separating network adapters from our MoPub SDK. This is to enable an independent release cadence resulting in faster updates and certification cycles. New mediation location is accessible [here](https://github.com/mopub/mopub-unity-mediation).
We have also added an additional tool, making it easy for publishers to get up and running with the mediation integration. Check out https://developers.mopub.com/docs/mediation/integrate/ and integration instructions at https://developers.mopub.com/docs/unity/getting-started/.

Please view the [MoPub Unity SDK changelog](https://github.com/mopub/mopub-unity-sdk/blob/master/CHANGELOG.md), [MoPub Android SDK changelog](https://github.com/mopub/mopub-android-sdk/blob/master/CHANGELOG.md), and [MoPub iOS SDK changelog](https://github.com/mopub/mopub-ios-sdk/blob/master/CHANGELOG.md) for a complete list of additions, fixes, and enhancements across releases and platforms.

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
* `unity/` - Contains MoPub Unity Plugin sample project
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

### Cloning the project
```
git clone https://github.com/mopub/unity-mopub
git submodule init
git submodule update
```

### Repository structure

* `mopub-android-sdk/` - Git submodule of the MoPub Android SDK
* `mopub-android-sdk-unity/` - Contains a project that adds Unity-specific files to the Android SDK
* `mopub-ios-sdk/` - Git submodule of the MoPub iOS SDK
* `mopub-ios-sdk-unity/` - Contains a project that adds Unity-specific files to the iOS SDK
* `unity/` - Contains the Unity Plugin
* `out/` - Where the Unity packages are exported after running `./unity-export-package.sh`

### How do I build?

Simply run `./build.sh` (make sure the Unity IDE is *not* running), which runs `git submodule update` and then invokes the following scripts:

* `mopub-android-sdk-unity-build.sh` - builds the mopub-android-sdk-unity project and copies the resulting artifacts into `unity/`
* `mopub-ios-sdk-unity-build.sh` - builds the mopub-ios-sdk-unity project and copies the resulting artifacts into `unity/`
* `unity-export-package.sh`  - exports the unity package into `out/`

Each script can be invoked separately. Exporting the unity package can also be done manually, by opening the `unity/` project in Unity, right-clicking the `Assets/` folder and chosing `Export Package...`.

### How do I run the sample unity project and test?

After building per instructions above, open the `unity/` project in Unity, click `File > Build Settings...`, select iOS or Android, click `Build and Run`.

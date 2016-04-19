### The structure of this directory is as follows:

```
mopub-android-sdk/            # Submodule of the MoPub android sdk. This remains unchanged.
mopub-android-sdk-unity/      # Contains a project that adds Unity specific files to the above sdk. Namely
			      # MoPubUnityPlugin.java
mopub-ios-sdk/ 		      # Submodule of the MoPub ios sdk. This remains unchanged.
mopub-ios-sdk-unity/	      # Contains a project that adds Unity specific files to the above sdk.
unity/			      # Contains the Unity plugin.
out/			      # Where the Unity package is exported after running ./unity-export-package.sh
```
### How do I build?

Easy; `./build.sh`. This invokes the following scripts:

`mopub-android-sdk-unity-build.sh` - builds the mopub-android-sdk-unity project and copies the resulting atrifacts into unity/
`mopub-ios-sdk-unity-build.sh` - builds the mopub-ios-sdk-unity project and copies the resulting artifacts into unity/
`unity-export-package.sh`  - exports the unity package

Each script can be invoked separately. Exporting the unity package can also be done manually, by opening the unity/ project in Unity, right-clicking the Assets/ folder and chosing "Export Package...".

### How do I run the sample unity project and test?

Open unity/ in Unity, click File->Build Settings..., select iOS or Android, click "Build and Run". If changes have been made to mopub-android-sdk-unity or mopub-ios-sdk-unity projects, use the scripts above to build-and-copy-binaries into the unity project prior to running.

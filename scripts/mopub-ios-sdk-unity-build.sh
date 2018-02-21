#!/usr/bin/env bash
my_dir="$(dirname "$0")"
source "$my_dir/validate.sh"

if [[ -e mopub-ios-sdk-unity/bin/libmopub-ios-sdk-unity.a ]]; then
    echo "Backing up previous archives..."
    mv mopub-ios-sdk-unity/bin/libmopub-ios-sdk-unity.a mopub-ios-sdk-unity/bin/libmopub-ios-sdk-unity.a.backup
    mv mopub-ios-sdk-unity/bin/libMoPubSDK.a mopub-ios-sdk-unity/bin/libMoPubSDK.a.backup
fi

# remove viewability binaries since they are not supported for unity
rm -rf mopub-ios-sdk/MoPubSDK/Viewability/Avid
rm -rf mopub-ios-sdk/MoPubSDK/Viewability/MOAT

# remove unit tests since the viewability unit tests cause compile problems
rm -rf mopub-ios-sdk/MoPubSDKTests

# update version number to have unity suffix
sed -i.bak 's/^\(#define MP_SDK_VERSION\)\(.*\)"/\1\2+unity"/'  mopub-ios-sdk/MoPubSDK/MPConstants.h
validate

xcrun xcodebuild -project mopub-ios-sdk-unity/mopub-ios-sdk-unity.xcodeproj -scheme "MoPub for Unity" -destination generic/platform=iphoneos clean
validate
xcrun xcodebuild GCC_PREPROCESSOR_DEFINITIONS="MP_FABRIC=1" -project mopub-ios-sdk-unity/mopub-ios-sdk-unity.xcodeproj -scheme "MoPub for Unity" OTHER_CFLAGS="-fembed-bitcode -w" -destination generic/platform=iphoneos build
validate

# after build, undo the unity suffix
cd mopub-ios-sdk
git checkout MoPubSDK/MPConstants.h
validate
rm -f MoPubSDK/MPConstants.h.bak
validate
cd ..

# Copy three artifacts into the unity plugin; libMoPubSDK.a - the unchanged mopub ios sdk, libmopub-ios-sdk-unity.a - the unity specific components, and mraid.js.
# Due to the treatment of .js files as source code in unity, we must change the extension. The extension gets changed back by the ios post build script within
# the unity plugin. The end result is an xcode project that contains 'mraid.js'. This removes the need to change the hard-coded extension in the ios sdk.

cp mopub-ios-sdk-unity/bin/libmopub-ios-sdk-unity.a unity/MoPubUnityPlugin/Assets/Plugins/iOS/mopub/libmopub-ios-sdk-unity.a
validate
cp mopub-ios-sdk-unity/bin/libMoPubSDK.a unity/MoPubUnityPlugin/Assets/Plugins/iOS/mopub/libMoPubSDK.a
validate
cp -r mopub-ios-sdk-unity/bin/MoPub.bundle/ unity/MoPubUnityPlugin/Assets/Plugins/iOS/mopub/MoPub.bundle/
validate
cp -f mopub-ios-sdk/MoPubSDK/Resources/*.{html,png} unity/MoPubUnityPlugin/Assets/Plugins/iOS/mopub/MoPub.bundle/
validate
mv unity/MoPubUnityPlugin/Assets/Plugins/iOS/mopub/MoPub.bundle/MRAID.bundle/mraid.js unity/MoPubUnityPlugin/Assets/Plugins/iOS/mopub/MoPub.bundle/MRAID.bundle/mraid.js.prevent_unity_compilation

# Update the iOS resources used when packaging the unity assets.

echo "Cleaning the contents of unity/MoPubUnityPlugin/Assets/MoPub/Editor/Support/MoPubSDK/"
rm -f unity/MoPubUnityPlugin/Assets/MoPub/Editor/Support/MoPubSDK/*
validate

echo "Copying all .h files from mopub-ios-sdk/MoPubSDK/ to unity/MoPubUnityPlugin/Assets/MoPub/Editor/Support/MoPubSDK/"
find mopub-ios-sdk/MoPubSDK -name "*.h" -type f -exec cp {} unity/MoPubUnityPlugin/Assets/MoPub/Editor/Support/MoPubSDK \;
validate

# Clean up submodule
cd mopub-ios-sdk
git checkout .

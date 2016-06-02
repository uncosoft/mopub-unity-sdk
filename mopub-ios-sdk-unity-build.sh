#!/usr/bin/env bash

if [[ -e mopub-ios-sdk-unity/bin/libmopub-ios-sdk-unity.a ]]; then
    echo "Backing up previous archives..."
    mv mopub-ios-sdk-unity/bin/libmopub-ios-sdk-unity.a mopub-ios-sdk-unity/bin/libmopub-ios-sdk-unity.a.backup
    mv mopub-ios-sdk-unity/bin/libMoPubSDK.a mopub-ios-sdk-unity/bin/libMoPubSDK.a.backup
fi

# update version number to have unity suffix
sed -i.bak 's/^\(#define MP_SDK_VERSION\)\(.*\)"/\1\2+unity"/'  mopub-ios-sdk/MoPubSDK/MPConstants.h

xcrun xcodebuild -project mopub-ios-sdk-unity/mopub-ios-sdk-unity.xcodeproj -scheme "MoPub for Unity" -destination generic/platform=iphoneos clean
xcrun xcodebuild GCC_PREPROCESSOR_DEFINITIONS="MP_FABRIC=1" -project mopub-ios-sdk-unity/mopub-ios-sdk-unity.xcodeproj -scheme "MoPub for Unity" -destination generic/platform=iphoneos build

# after build, undo the unity suffix
cd mopub-ios-sdk
git checkout MoPubSDK/MPConstants.h
rm -f MoPubSDK/MPConstants.h.bak
cd ..

if [[ $? -ne 0 ]]; then
    echo "Building mopub-ios-sdk-unity failed, quitting..."
    exit 1
fi

# Copy three artifacts into the unity plugin; libMoPubSDK.a - the unchanged mopub ios sdk, libmopub-ios-sdk-unity.a - the unity specific components, and mraid.js.
# Due to the treatment of .js files as source code in unity, we must change the extension. The extension gets changed back by the ios post build script within
# the unity plugin. The end result is an xcode project that contains 'mraid.js'. This removes the need to change the hard-coded extension in the ios sdk.

cp mopub-ios-sdk-unity/bin/libmopub-ios-sdk-unity.a unity/MoPubUnityPlugin/Assets/Plugins/iOS/mopub/libmopub-ios-sdk-unity.a
cp mopub-ios-sdk-unity/bin/libMoPubSDK.a unity/MoPubUnityPlugin/Assets/Plugins/iOS/mopub/libMoPubSDK.a
cp mopub-ios-sdk/MoPubSDK/Resources/MRAID.bundle/mraid.js unity/MoPubUnityPlugin/Assets/Plugins/iOS/mopub/MoPub.bundle/MRAID.bundle/mraid.js.prevent_unity_compilation

if [[ $? -ne 0 ]]; then
    echo "Failed copying the mopub-ios-sdk-unity artifact into Unity, quitting..."
    exit 1
fi

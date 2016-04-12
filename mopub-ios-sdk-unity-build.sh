#!/usr/bin/env bash

if [[ -e mopub-ios-sdk-unity/bin/libmopub-ios-sdk-unity.a ]]; then
    echo "Backing up previous archives..."
    mv mopub-ios-sdk-unity/bin/libmopub-ios-sdk-unity.a mopub-ios-sdk-unity/bin/libmopub-ios-sdk-unity.a.backup
    mv mopub-ios-sdk-unity/bin/libMoPubSDK.a mopub-ios-sdk-unity/bin/libMoPubSDK.a.backup
fi

xcrun xcodebuild -project mopub-ios-sdk-unity/mopub-ios-sdk-unity.xcodeproj -scheme "MoPub for Unity" -destination generic/platform=iphoneos clean
xcrun xcodebuild -project mopub-ios-sdk-unity/mopub-ios-sdk-unity.xcodeproj -scheme "MoPub for Unity" -destination generic/platform=iphoneos build

if [[ $? -ne 0 ]]; then
    echo "Building mopub-ios-sdk-unity failed, quitting..."
    exit 1
fi

cp mopub-ios-sdk-unity/bin/libmopub-ios-sdk-unity.a unity/MoPubUnityPlugin/Assets/Plugins/iOS/mopub/libmopub-ios-sdk-unity.a
cp mopub-ios-sdk-unity/bin/libMoPubSDK.a unity/MoPubUnityPlugin/Assets/Plugins/iOS/mopub/libMoPubSDK.a
cp mopub-ios-sdk/MoPubSDK/Resources/MRAID.bundle/mraid.js unity/MoPubUnityPlugin/Assets/Plugins/iOS/mopub/MRAID.bundle/mraid.js.prevent_unity_compilation

if [[ $? -ne 0 ]]; then
    echo "Failed copying the mopub-ios-sdk-unity artifact into Unity, quitting..."
    exit 1
fi

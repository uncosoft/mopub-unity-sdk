#!/usr/bin/env bash

if [[ -e mopub-ios-sdk-unity/bin/libmopub-ios-sdk-unity.a ]]; then
    echo "Backing up previous archives..."
    mv mopub-ios-sdk-unity/bin/libmopub-ios-sdk-unity.a mopub-ios-sdk-unity/bin/libmopub-ios-sdk-unity.a.backup
    mv mopub-ios-sdk-unity/bin/libMoPubSDK.a mopub-ios-sdk-unity/bin/libMoPubSDK.a.backup
fi

cd mopub-ios-sdk-unity
xcodebuild -workspace mopub-ios-sdk.xcworkspace/ -scheme mopub-ios-sdk-unity clean
xcodebuild -workspace mopub-ios-sdk.xcworkspace/ -scheme mopub-ios-sdk-unity build

if [[ $? -ne 0 ]]; then
    echo "Building mopub-ios-sdk-unity failed, quitting..."
    exit 1
fi

cd ..
cp mopub-ios-sdk-unity/bin/libmopub-ios-sdk-unity.a unity/MoPubUnityPlugin/Assets/Plugins/iOS/mopub/libmopub-ios-sdk-unity.a
cp mopub-ios-sdk-unity/bin/libMoPubSDK.a unity/MoPubUnityPlugin/Assets/Plugins/iOS/mopub/libMoPubSDK.a

if [[ $? -ne 0 ]]; then
    echo "Failed copying the mopub-ios-sdk-unity artifact into Unity, quitting..."
    exit 1
fi

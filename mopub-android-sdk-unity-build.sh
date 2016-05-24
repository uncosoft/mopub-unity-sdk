#!/usr/bin/env bash

# Current SDK version
SDK_VERSION=4.6.1

# Append "+unity" suffix to SDK_VERSION in MoPub.java
sed -i.bak 's/^\(.*public static final String SDK_VERSION\)\(.*\)"/\1\2+unity"/' mopub-android-sdk/mopub-sdk/src/main/java/com/mopub/common/MoPub.java

# Build mopub-android-sdk-unity project
cd mopub-android-sdk-unity
./gradlew clean
./gradlew assembleRelease -x javadoc
cd ..

# Undo +unity suffix after build
cd mopub-android-sdk
git checkout mopub-sdk/src/main/java/com/mopub/common/MoPub.java
rm -f mopub-sdk/src/main/java/com/mopub/common/MoPub.java.bak
cd ..

if [[ $? -ne 0 ]]; then
    echo "Failed building the mopub-android-sdk-unity project, quitting..."
    exit 1
fi

# Copy the generated jars into the unity package. There are two jars:
#   * mopub-sdk.jar - the unchanged mopub android sdk
#   * mopub.jar - all unity specific components as well as all third party network adapters
cp mopub-android-sdk-unity/build/intermediates/bundles/release/classes.jar unity/MoPubUnityPlugin/Assets/Plugins/Android/mopub/libs/mopub.jar
cp mopub-android-sdk-unity/build/intermediates/exploded-aar/com.mopub/mopub-sdk/$SDK_VERSION/jars/classes.jar unity/MoPubUnityPlugin/Assets/Plugins/Android/mopub/libs/mopub-sdk.jar

# Copy MoPub SDK dependency jars
cp ~/Library/Android/sdk/extras/android/support/v4/android-support-v4.jar unity/MoPubUnityPlugin/Assets/Plugins/Android/mopub/libs/android-support-v4-23.1.1.jar
cp mopub-android-sdk-unity/build/intermediates/exploded-aar/com.android.support/recyclerview-v7/23.1.1/jars/classes.jar unity/MoPubUnityPlugin/Assets/Plugins/Android/mopub/libs/android-support-recyclerview-v7-23.1.1.jar
cp mopub-android-sdk-unity/build/intermediates/exploded-aar/com.google.android.exoplayer/exoplayer/r1.5.6/jars/classes.jar unity/MoPubUnityPlugin/Assets/Plugins/Android/mopub/libs/exoplayer-1.5.6.jar

# Copy MoPub Custom Events jars
cp mopub-android-sdk-unity/adcolony-custom-events/build/intermediates/bundles/release/classes.jar unity/MoPubUnityPlugin/Assets/Plugins/Android/mopub-support/libs/AdColony/mopub-adcolony-custom-events.jar
cp mopub-android-sdk-unity/admob-custom-events/build/intermediates/bundles/release/classes.jar unity/MoPubUnityPlugin/Assets/Plugins/Android/mopub-support/libs/AdMob/mopub-admob-custom-events.jar
cp mopub-android-sdk-unity/chartboost-custom-events/build/intermediates/bundles/release/classes.jar unity/MoPubUnityPlugin/Assets/Plugins/Android/mopub-support/libs/Chartboost/mopub-chartboost-custom-events.jar
cp mopub-android-sdk-unity/facebook-custom-events/build/intermediates/bundles/release/classes.jar unity/MoPubUnityPlugin/Assets/Plugins/Android/mopub-support/libs/Facebook/mopub-facebook-custom-events.jar
cp mopub-android-sdk-unity/millennial-custom-events/build/intermediates/bundles/release/classes.jar unity/MoPubUnityPlugin/Assets/Plugins/Android/mopub-support/libs/Millennial/mopub-millennial-custom-events.jar
cp mopub-android-sdk-unity/unityads-custom-events/build/intermediates/bundles/release/classes.jar unity/MoPubUnityPlugin/Assets/Plugins/Android/mopub-support/libs/UnityAds/mopub-unityads-custom-events.jar
cp mopub-android-sdk-unity/vungle-custom-events/build/intermediates/bundles/release/classes.jar unity/MoPubUnityPlugin/Assets/Plugins/Android/mopub-support/libs/Vungle/mopub-vungle-custom-events.jar

if [[ $? -ne 0 ]]; then
    echo "Couldn't copy the generated jar into the Unity project, quitting..."
    exit 1
fi

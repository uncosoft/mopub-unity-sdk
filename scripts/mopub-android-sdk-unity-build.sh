#!/usr/bin/env bash
my_dir="$(dirname "$0")"
source "$my_dir/validate.sh"

# Current SDK version
SDK_VERSION=4.20.0

# Append "+unity" suffix to SDK_VERSION in MoPub.java
sed -i.bak 's/^\(.*public static final String SDK_VERSION\)\(.*\)"/\1\2+unity"/' mopub-android-sdk/mopub-sdk/mopub-sdk-base/src/main/java/com/mopub/common/MoPub.java
validate

# Build mopub-android-sdk-unity project
cd mopub-android-sdk-unity
./gradlew clean
./gradlew assembleRelease
validate
cd ..

# Undo +unity suffix after build
cd mopub-android-sdk
git checkout mopub-sdk/mopub-sdk-base/src/main/java/com/mopub/common/MoPub.java
validate
rm -f mopub-sdk/mopub-sdk-base/src/main/java/com/mopub/common/MoPub.java.bak
validate
cd ..

# Copy the generated jars into the unity package:
#   * mopub-unity-plugins.jar: unity plugins for banner, interstitial, and rewarded video
#   * mopub-sdk-*.jar: modularized SDK jars (excluding native-static and native-video)
cp mopub-android-sdk-unity/build/intermediates/bundles/release/classes.jar unity/MoPubUnityPlugin/Assets/Plugins/Android/mopub/libs/mopub-unity-plugins.jar
validate
cp mopub-android-sdk/mopub-sdk/mopub-sdk-base/build/intermediates/bundles/release/classes.jar unity/MoPubUnityPlugin/Assets/Plugins/Android/mopub/libs/mopub-sdk-base.jar
validate
cp mopub-android-sdk/mopub-sdk/mopub-sdk-banner/build/intermediates/bundles/release/classes.jar unity/MoPubUnityPlugin/Assets/Plugins/Android/mopub/libs/mopub-sdk-banner.jar
validate
cp mopub-android-sdk/mopub-sdk/mopub-sdk-interstitial/build/intermediates/bundles/release/classes.jar unity/MoPubUnityPlugin/Assets/Plugins/Android/mopub/libs/mopub-sdk-interstitial.jar
validate
cp mopub-android-sdk/mopub-sdk/mopub-sdk-rewardedvideo/build/intermediates/bundles/release/classes.jar unity/MoPubUnityPlugin/Assets/Plugins/Android/mopub/libs/mopub-sdk-rewardedvideo.jar
validate

# Copy MoPub SDK dependency jars/aars
if [ -f $ANDROID_HOME/extras/android/support/v4/android-support-v4.jar ]; then
  # jars go under Plugins/Android/mopub/libs/
  cp $ANDROID_HOME/extras/android/support/v4/android-support-v4.jar unity/MoPubUnityPlugin/Assets/Plugins/Android/mopub/libs/android-support-v4-23.1.1.jar
else
  # aars go under Plugins/Android/
  cp $ANDROID_HOME/extras/android/m2repository/com/android/support/support-v4/23.1.1/support-v4-23.1.1.aar unity/MoPubUnityPlugin/Assets/Plugins/Android/android-support-v4-23.1.1.aar
fi
validate

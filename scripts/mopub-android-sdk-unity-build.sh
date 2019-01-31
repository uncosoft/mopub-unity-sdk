#!/usr/bin/env bash
my_dir="$(dirname "$0")"
source "$my_dir/validate.sh"
source "$my_dir/print_helpers.sh"

# Set this to 'true' to build with the internal Android SDK; set to 'false' to build with public Android SDK.
# May also be overriden from the command line as such: INTERNAL_SDK=false ./scripts/mopub-android-sdk-unity-build.sh
if [ -z "$INTERNAL_SDK" ]; then
	if [ -d mopub-android ] && grep -q 'submodule "mopub-android"' .gitmodules; then INTERNAL_SDK=true; else INTERNAL_SDK=false; fi
fi

# Set the SDK directory as an environment variable for mopub-android-sdk-unity/settings.gradle
export SDK_DIR=mopub-android-sdk

SDK_NAME="PUBLIC Android SDK"
SDK_VERSION_SUFFIX=unity
if [ "$INTERNAL_SDK" = true ]; then
  SDK_DIR=mopub-android
  SDK_VERSION_SUFFIX=$(cd $SDK_DIR; git rev-parse --short HEAD)
  SDK_NAME="INTERNAL Android SDK ("$SDK_VERSION_SUFFIX")"
fi
SDK_VERSION_HOST_FILE="$SDK_DIR"/mopub-sdk/mopub-sdk-base/src/main/java/com/mopub/common/MoPub.java

print_build_starting Android "$SDK_NAME"

# Append "+unity" (or the latest commit SHA, for internal SDK builds) suffix to SDK_VERSION in MoPub.java
sed -i.bak 's/^\(.*public static final String SDK_VERSION.*"\)\([^+"]*\).*"/\1\2+'$SDK_VERSION_SUFFIX'"/' $SDK_VERSION_HOST_FILE
validate

# Build mopub-android-sdk-unity project
mopub-android-sdk-unity/gradlew -p mopub-android-sdk-unity clean assembleRelease

# Undo +unity suffix after build
mv $SDK_VERSION_HOST_FILE.bak $SDK_VERSION_HOST_FILE
validate

CLASSES_JAR=build/intermediates/bundles/release/classes.jar
UNITY_DIR=unity-sample-app/Assets/MoPub/Plugins/Android/MoPub.plugin

# Copy the generated jars into the unity package:
#   * classes.jar: unity plugins for banner, interstitial, and rewarded video
#   * mopub-sdk-*.jar: modularized SDK jars (excluding native-video)
cp mopub-android-sdk-unity/"$CLASSES_JAR" "$UNITY_DIR"/libs/mopub-unity-wrappers.jar
validate
for lib in base banner interstitial rewardedvideo native-static; do
  cp "$SDK_DIR/mopub-sdk/mopub-sdk-$lib/$CLASSES_JAR" "$UNITY_DIR"/libs/mopub-sdk-$lib.jar
  validate
done

print_build_finished Android "$SDK_NAME"

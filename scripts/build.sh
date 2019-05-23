#!/usr/bin/env bash
my_dir="$(dirname $0)"
source "$my_dir/print_helpers.sh"
source "$my_dir/validate.sh"

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

print_blue_line "Building $SDK_NAME"

$my_dir/../mopub-android-sdk-unity/gradlew -p mopub-android-sdk-unity clean assembleRelease
validate "Android build failed, fix before continuing."

print_green_line "Done building $SDK_NAME"

print_blue_line "Copying jars from $SDK_NAME"

CLASSES_JAR=build/intermediates/packaged-classes/release/classes.jar
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

print_green_line "Done copying jars from $SDK_NAME"

$my_dir/unity-export-package.sh
validate "Exporting the package failed."

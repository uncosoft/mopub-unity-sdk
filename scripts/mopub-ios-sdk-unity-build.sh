#!/usr/bin/env bash
my_dir="$(dirname "$0")"
source "$my_dir/validate.sh"
source "$my_dir/print_helpers.sh"

# Set this to 'true' to build with the internal iOS SDK; set to 'false' to build with public iOS SDK.
# May also be overriden from the command line as such: INTERNAL_SDK=false ./scripts/mopub-ios-sdk-unity-build.sh
: "${INTERNAL_SDK:=false}"

SDK_DIR="mopub-ios-sdk"
SDK_NAME="PUBLIC iOS SDK"
SDK_VERSION_SUFFIX="unity"
XCODE_PROJECT_NAME="mopub-ios-sdk-unity.xcodeproj"
if [ $INTERNAL_SDK == true ]; then
  SDK_DIR="mopub-ios"
  SDK_VERSION_SUFFIX=$(cd $SDK_DIR; git rev-parse --short HEAD)
  SDK_NAME="INTERNAL iOS SDK ("$SDK_VERSION_SUFFIX")"
  XCODE_PROJECT_NAME="internal-"$XCODE_PROJECT_NAME
fi
SDK_VERSION_HOST_FILE=$SDK_DIR/MoPubSDK/MPConstants.h
XCODEBUILD_LOG_FILE="$my_dir/xcodebuildlog.txt"

# echo "Building the MoPub Unity plugin for iOS using the $SDK_NAME..."
print_build_starting "iOS" "$SDK_NAME"

# remove viewability binaries since they are not supported for unity
rm -rf $SDK_DIR/MoPubSDK/Viewability/{Avid,MOAT}

# remove unit tests since the viewability unit tests cause compile problems
rm -rf $SDK_DIR/MoPubSDKTests

# Append "+unity" (or the latest commit SHA, for internal SDK builds) suffix to SDK_VERSION in MPConstants.h
sed -i.bak 's/^\(#define MP_SDK_VERSION.*"\)\([^+"]*\).*"/\1\2+'$SDK_VERSION_SUFFIX'"/' $SDK_VERSION_HOST_FILE
validate

# make a clean build (copies build artifacts to mopub-ios-sdk-unity/bin directory)
echo -n "Running xcodebuild... "
xcrun xcodebuild -project mopub-ios-sdk-unity/$XCODE_PROJECT_NAME \
                 -scheme "MoPub for Unity" \
                 -configuration "Release" \
                 OTHER_CFLAGS="-fembed-bitcode -w" \
                 BITCODE_GENERATION_MODE=bitcode \
                 clean \
                 build > $XCODEBUILD_LOG_FILE
validate "Building the iOS wrapper has failed, please check $XCODEBUILD_LOG_FILE"
echo "done"

# after build, undo the unity suffix
mv $SDK_VERSION_HOST_FILE.bak $SDK_VERSION_HOST_FILE
validate

UNITY_DIR=unity-sample-app/Assets/MoPub/Plugins/iOS

# copy build artifacts to unity project, deleting any now-missing files from the destination 
# to account for file moves and renames.  (.meta files excluded, unity will handle them.)
echo -n "Copying build artifacts into unity project... "
rsync -r -v --delete --exclude='*.meta' mopub-ios-sdk-unity/bin/* $UNITY_DIR >> $XCODEBUILD_LOG_FILE
validate "Copying iOS wrapper build artifacts has failed, please check $XCODEBUILD_LOG_FILE"
echo "done"

# copy in the html and png files from the original source
# TODO (ADF-3528): not clear why this is needed, as the framework already has these files?
echo -n "Copying additional artifacts into unity project... "
rsync -r -v --delete --exclude='*.meta' $SDK_DIR/MoPubSDK/Resources/*.{html,png} $UNITY_DIR/MoPubSDKFramework.framework >> $XCODEBUILD_LOG_FILE
validate "Copying iOS wrapper build artifacts has failed, please check $XCODEBUILD_LOG_FILE"
echo "done"

# Due to the treatment of .js files as source code in unity, we must change the extension to something it won't try to compile. 
# The extension gets changed back by the ios post build script within the unity plugin. 
mv $UNITY_DIR/MoPubSDKFramework.framework/MRAID.bundle/mraid.js \
   $UNITY_DIR/MoPubSDKFramework.framework/MRAID.bundle/mraid.js.prevent_unity_compilation

# Clean up submodule
cd $SDK_DIR
git checkout .

print_build_finished "iOS" "$SDK_NAME"

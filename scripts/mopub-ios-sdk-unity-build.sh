#!/usr/bin/env bash
my_dir="$(dirname "$0")"
source "$my_dir/validate.sh"

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

# TODO(ADF-3403): use rsync to update Assets/Plugins/iOS/MoPub* to account for renames and deletions
cp mopub-ios-sdk-unity/bin/MoPub*.{h,m,mm} unity-sample-app/Assets/Plugins/iOS
validate
rm -rf unity-sample-app/Assets/Plugins/iOS/MoPubSDKFramework.framework/
validate
cp -r mopub-ios-sdk-unity/bin/MoPubSDKFramework.framework/ unity-sample-app/Assets/Plugins/iOS/MoPubSDKFramework.framework/
validate
cp -f mopub-ios-sdk/MoPubSDK/Resources/*.{html,png} unity-sample-app/Assets/Plugins/iOS/MoPubSDKFramework.framework/
validate
mv unity-sample-app/Assets/Plugins/iOS/MoPubSDKFramework.framework/MRAID.bundle/mraid.js unity-sample-app/Assets/Plugins/iOS/MoPubSDKFramework.framework/MRAID.bundle/mraid.js.prevent_unity_compilation

# Clean up submodule
cd mopub-ios-sdk
git checkout .

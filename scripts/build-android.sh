#!/usr/bin/env bash
my_dir="$(dirname $0)"
source "$my_dir/print_helpers.sh"
source "$my_dir/validate.sh"

# Set this to 'true' to build with the internal Android SDK; set to 'false' to build with public Android SDK.
# May also be overriden from the command line as such: INTERNAL_SDK=false ./scripts/mopub-android-sdk-unity-build.sh
if [[ -z "$INTERNAL_SDK" ]]; then
  if [[ -d mopub-android ]] && grep -q 'submodule "mopub-android"' .gitmodules; then INTERNAL_SDK=true; else INTERNAL_SDK=false; fi
fi

# Set the SDK directory as an environment variable for mopub-android-sdk-unity/settings.gradle
export SDK_DIR=mopub-android-sdk

SDK_NAME="PUBLIC Android SDK"
SDK_VERSION_SUFFIX=unity
if [[ "$INTERNAL_SDK" = true ]]; then
  SDK_DIR=mopub-android
  SDK_VERSION_SUFFIX=$(cd ${SDK_DIR}; git rev-parse --short HEAD)
  SDK_NAME="INTERNAL Android SDK ("${SDK_VERSION_SUFFIX}")"
fi

print_blue_line "Building $SDK_NAME"

${my_dir}/../mopub-android-sdk-unity/gradlew -p mopub-android-sdk-unity clean assembleRelease
validate "Android build failed, fix before continuing."

print_green_line "Done building $SDK_NAME"

print_blue_line "Copying wrappers aar"

AAR_DIR=build/outputs/aar
UNITY_DIR=unity-sample-app/Assets/MoPub/Plugins/Android
MOPUB_DEPENDENCIES_XML=unity-sample-app/Assets/MoPub/Scripts/Editor/MoPubDependencies.xml
VOLLEY_DEPENDENCY='<androidPackage spec="com.mopub.volley:mopub-volley:2.1.0"\/>'

# Copy the generated mopub-unity-wrappers*.aar into the unity sample app
cp mopub-android-sdk-unity/"$AAR_DIR"/mopub-unity-wrappers-release.aar "$UNITY_DIR"/mopub-unity-wrappers.aar
validate

# Internal builds use the built Android SDK aars instead of the released ones
# NOTE: the iOS dependency is handled by the release script (via update_xml_dependencies) since iOS does not have this
# extra build step which needs to be internal/external-aware.
if [[ "$INTERNAL_SDK" = true ]]; then

  print_blue_line "Copying internal Android SDK aars"
  # Copy the generated mopub-sdk-*.aar into the unity sample app
  for lib in base banner fullscreen native-static; do
    cp "$SDK_DIR/mopub-sdk/mopub-sdk-$lib/$AAR_DIR/mopub-sdk-$lib-release.aar" "$UNITY_DIR"/mopub-sdk-${lib}.aar
    validate
  done

  print_blue_line "Disabling Android SDK EDM Dependency"
  # Comment out SDK and add Volley in MoPubDependencies.xml
  sed -i "" -Ee "s/^  .*com.mopub:mopub-sdk.*/<!--&-->${VOLLEY_DEPENDENCY}/" ${MOPUB_DEPENDENCIES_XML}
  validate
else

  print_blue_line "Removing internal Android SDK aars"
  # Remove previously copied internal aars
  rm "$UNITY_DIR"/mopub-sdk-*.aar* >& /dev/null

  print_blue_line "Enabling Android SDK EDM Dependency"
  # Uncomment SDK and remove Volley dependency in MoPubDependencies.xml
  sed -i "" -Ee '/<!--  .*com.mopub:mopub-sdk.*/s/^<!--//' \
            -Ee "/.*com.mopub:mopub-sdk.*-->${VOLLEY_DEPENDENCY}/s/-->${VOLLEY_DEPENDENCY}//" ${MOPUB_DEPENDENCIES_XML}
  validate
fi

print_green_line "Done building Android wrapper!"

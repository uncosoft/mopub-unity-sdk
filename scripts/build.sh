#!/usr/bin/env bash
my_dir="$(dirname "$0")"
source "$my_dir/validate.sh"

$my_dir/../mopub-android-sdk-unity/gradlew -p mopub-android-sdk-unity clean assembleRelease
validate "Android build failed, fix before continuing."

$my_dir/unity-export-package.sh
validate "Exporting the package failed."

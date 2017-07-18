#!/usr/bin/env bash
my_dir="$(dirname "$0")"
source "$my_dir/validate.sh"

git submodule update
validate "Updating git submodules failed, fix before continuing."

./mopub-android-sdk-unity-build.sh
validate "Android build failed, fix before continuing."

./mopub-ios-sdk-unity-build.sh
validate "iOS build failed, fix before continuing."

./unity-export-package.sh
validate "Exporting the package failed."

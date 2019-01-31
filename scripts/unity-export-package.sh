#!/usr/bin/env bash
my_dir="$(dirname "$0")"
source "$my_dir/validate.sh"
source "$my_dir/print_helpers.sh"

PACKAGE_NAME=MoPubUnity
UNITY_BIN=/Applications/Unity/Unity.app/Contents/MacOS/Unity
PROJECT_PATH="`pwd`/unity-sample-app"
OUT_DIR="`pwd`/mopub-unity-plugin"
DEST_PACKAGE="$OUT_DIR/$PACKAGE_NAME.unitypackage"
EXPORT_FOLDERS_MAIN="Assets/MoPub Assets/PlayServicesResolver"
EXPORT_LOG="$my_dir/exportlog.txt"

print_export_starting

# Programatically export MoPub.unitypackage.
# This will export all folders under Assets/MoPub as well as the Google Jar Resolver (the -gvh_disable flag belongs to that, and per their
# README on github, you should use this flag when exporting a unitypackage in which you are redistributing Jar Resolver).
echo -n "Exporting the MoPub Unity package... "
$UNITY_BIN -gvh_disable -projectPath $PROJECT_PATH -quit -batchmode -logFile $EXPORT_LOG -exportPackage $EXPORT_FOLDERS_MAIN $DEST_PACKAGE
validate "Building the unity package has failed, please check $EXPORT_LOG\nMake sure Unity isn't running when invoking this script!"
echo "done"

print_export_finished "$DEST_PACKAGE"

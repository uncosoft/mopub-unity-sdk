#!/usr/bin/env bash
my_dir="$(dirname "$0")"
source "$my_dir/validate.sh"
source "$my_dir/print_helpers.sh"

PACKAGE_NAME=MoPubUnity
UNITY_BIN=/Applications/Unity/Unity.app/Contents/MacOS/Unity
PROJECT_PATH="`pwd`/unity-sample-app"
OUT_DIR="`pwd`/mopub-unity-plugin"
DEST_PACKAGE="$OUT_DIR/$PACKAGE_NAME.unitypackage"
EXPORT_FOLDERS_MAIN="Assets/MoPub Assets/Plugins Assets/Scripts Assets/Scenes"
EXPORT_LOG="$my_dir/exportlog.txt"

print_export_starting

echo -n "Stashing git-ignored files... "
git add .
git stash save --keep-index --all
validate "Stashing failed!"
echo "done"

# Programatically export MoPub.unitypackage.
# This will export all folders under Assets/MoPub and Assets/Plugins
echo -n "Exporting the MoPub Unity package... "
$UNITY_BIN -projectPath $PROJECT_PATH -quit -batchmode -logFile $EXPORT_LOG -exportPackage $EXPORT_FOLDERS_MAIN $DEST_PACKAGE
validate "Building the unity package has failed, please check $EXPORT_LOG\nMake sure Unity isn't running when invoking this script!"
echo "done"

echo -n "Cleaning project after export... "
git clean -xdf
echo "done"

echo -n "Recovering stashed files... "
git stash pop
echo "done"

print_export_finished "$DEST_PACKAGE"

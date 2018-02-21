#!/usr/bin/env bash
my_dir="$(dirname "$0")"
source "$my_dir/validate.sh"

PACKAGE_NAME=MoPubUnity
UNITY_BIN=/Applications/Unity/Unity.app/Contents/MacOS/Unity
PROJECT_PATH="`pwd`/unity/MoPubUnityPlugin"
OUT_DIR="`pwd`/mopub-unity-plugin"
DEST_PACKAGE="$OUT_DIR/$PACKAGE_NAME.unitypackage"
EXPORT_FOLDERS_MAIN="Assets/MoPub Assets/Plugins Assets/Scripts Assets/Scenes"
EXPORT_LOG="$OUT_DIR/exportlog.txt"

echo "Attempting to export the main package..."

# Programatically export MoPub.unitypackage. This will export all folders under Assets/MoPub and Assets/Plugins, including
# all third-party network adapters. This is ok, in the next step, we remove those folders.

$UNITY_BIN -projectPath $PROJECT_PATH -quit -batchmode -logFile $EXPORT_LOG -exportPackage $EXPORT_FOLDERS_MAIN $DEST_PACKAGE
validate "Building the unity package has failed, please check $EXPORT_LOG\nMake sure Unity isn't running when invoking this script!"

echo "Exported $DEST_PACKAGE"

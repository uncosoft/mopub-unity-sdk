#!/usr/bin/env bash

PACKAGE_NAME=MoPubUnity
UNITY_BIN=/Applications/Unity/Unity.app/Contents/MacOS/Unity
PROJECT_PATH="`pwd`/unity/MoPubUnityPlugin"
OUT_DIR="`pwd`/out"
DEST_PACKAGE="$OUT_DIR/$PACKAGE_NAME.unitypackage"
EXPORT_FOLDERS="Assets/MoPub Assets/Plugins/Android/mopub"
EXPORT_LOG="$OUT_DIR/exportlog.txt"

rm -rf $OUT_DIR
mkdir -p $OUT_DIR

$UNITY_BIN -projectPath $PROJECT_PATH -quit -batchmode -logFile $EXPORT_LOG -exportPackage $EXPORT_FOLDERS $DEST_PACKAGE

if [[ $? -ne 0 ]]; then
    echo "Building the unity package has failed, please check $EXPORT_LOG"
    exit 1
fi

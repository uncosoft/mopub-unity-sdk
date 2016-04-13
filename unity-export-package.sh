#!/usr/bin/env bash

PACKAGE_NAME=MoPubUnity
UNITY_BIN=/Applications/Unity/Unity.app/Contents/MacOS/Unity
PROJECT_PATH="`pwd`/unity/MoPubUnityPlugin"
OUT_DIR="`pwd`/out"
DEST_PACKAGE="$OUT_DIR/$PACKAGE_NAME.unitypackage"
EXPORT_FOLDERS_MAIN="Assets/MoPub Assets/Plugins"
EXPORT_LOG="$OUT_DIR/exportlog.txt"

rm -rf $OUT_DIR
mkdir -p $OUT_DIR

$UNITY_BIN -projectPath $PROJECT_PATH -quit -batchmode -logFile $EXPORT_LOG -exportPackage $EXPORT_FOLDERS_MAIN $DEST_PACKAGE

if [[ $? -ne 0 ]]; then
    echo "Building the unity package has failed, please check $EXPORT_LOG"
    echo "Make sure Unity isn't running when invoking this script!"
    exit 1
fi

# We need to remove the Support folders from the main package

mkdir -p $OUT_DIR/trim
mv $DEST_PACKAGE $OUT_DIR/trim/mopub.tar.gz
cd $OUT_DIR/trim
tar -xf mopub.tar.gz
rm mopub.tar.gz

find . -name pathname -print0 | xargs -0 awk '{print $1, FILENAME}' | while read pathname filename; do
    parent="$(dirname "$filename")"

    if [[ $pathname == *"Support"* ]]; then
	echo "Removing $filename ($pathname), and parent dir $parent from main package"
	rm -rf $filename
	rm -rf $parent
    fi
done

tar -zcf MoPubUnityPlugin.unitypackage *
mv MoPubUnityPlugin.unitypackage ../
cd ..
rm -rf trim

echo "Exported $DEST_PACKAGE"

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

echo "Attempting to export the main package..."

# Programatically export MaPub.unitypackage. This will export all folders under Assets/MoPub and Assets/Plugins, including
# all third-party network adapters. This is ok, in the next step, we remove those folders.

$UNITY_BIN -projectPath $PROJECT_PATH -quit -batchmode -logFile $EXPORT_LOG -exportPackage $EXPORT_FOLDERS_MAIN $DEST_PACKAGE

if [[ $? -ne 0 ]]; then
    echo "Building the unity package has failed, please check $EXPORT_LOG"
    echo "Make sure Unity isn't running when invoking this script!"
    exit 1
fi

# We need to remove the third-party network adapters from the main package. They will be exported separately.

echo "Done, removing third-party support folders from the main package..."

# Unpack the generated package.

mkdir -p $OUT_DIR/trim
mv $DEST_PACKAGE $OUT_DIR/trim/mopub.tar.gz
cd $OUT_DIR/trim
tar -xf mopub.tar.gz
rm mopub.tar.gz

find . -name pathname -print0 | xargs -0 awk '{print $1, FILENAME}' | while read pathname filename; do
    parent="$(dirname "$filename")"

    # Remove any path that contains "Support", but leave the MoPubSDK headers in place.

    if [[ $pathname == *"Support"* ]] && [[ $pathname != *"Support/MoPubSDK"* ]]; then
	echo "Removing $filename ($pathname), and parent dir $parent from main package"
	rm -rf $filename
	rm -rf $parent
    fi

    if [[ $pathname == *"Android/assets"* ]] || [[ $pathname == *"google-play-services"* ]] || [[ $pathname == *"mopub-support"* ]] || [[ $pathname == *"mm-activity"* ]]; then
	echo "Removing $filename ($pathname), and parent dir $parent from main package"
	rm -rf $filename
	rm -rf $parent
    fi
done

# Repack the package.

tar -zcf "$PACKAGE_NAME.unitypackage" *
mv "$PACKAGE_NAME.unitypackage" ../
cd ..
rm -rf trim

echo "Exported $DEST_PACKAGE"

# Now, export each of the third-party network adapters.

SUPPORT_LIBS=( "AdColony" "AdMob" "Chartboost" "Facebook" "Millennial" "UnityAds" "Vungle" )

for SUPPORT_LIB in "${SUPPORT_LIBS[@]}"
do
    EXPORT_FOLDERS_SUPPORT="Assets/MoPub/Editor/Support/$SUPPORT_LIB"
    DEST_PACKAGE="$OUT_DIR/${SUPPORT_LIB}Support.unitypackage"

    echo "Exporting $SUPPORT_LIB ($EXPORT_FOLDERS_SUPPORT) to $DEST_PACKAGE"
    $UNITY_BIN -projectPath $PROJECT_PATH -quit -batchmode -logFile $EXPORT_LOG -exportPackage $EXPORT_FOLDERS_SUPPORT $DEST_PACKAGE
done

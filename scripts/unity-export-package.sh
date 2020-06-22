#!/usr/bin/env bash
my_dir="$(dirname $0)"
source "$my_dir/validate.sh"
source "$my_dir/print_helpers.sh"

ensure_unity_bin

PACKAGE_NAME=MoPubUnity
PROJECT_PATH="$PWD/unity-sample-app"
OUT_DIR="$PWD/mopub-unity-plugin"
DEST_PACKAGE="$OUT_DIR/$PACKAGE_NAME.unitypackage"
EXPORT_FOLDERS_MAIN=$(cd $PROJECT_PATH; find Assets/MoPub/* Assets/PlayServicesResolver Assets/ExternalDependencyManager -type d -prune ! -name Mediation)
EXPORT_LOG="$my_dir/exportlog.txt"

print_export_starting

# Programatically export MoPub.unitypackage.
# This exports all directories under Assets/MoPub except Mediation (in case the project has any network adapters installed).
# It also imports and includes the Google Play Services Resolver using the gvh_disable, per their README on github, to 
# prevent a version clash in a pub's project when importing a newer version of it.

# TODO (ADF-4383): Replace the res/ dir (and this workaround) with in-editor adaptive icons once
# Unity 2018 is the minimum supported version.
echo -e "Excluding Android resources to avoid overriding Publisher icon...\n"
mv unity-sample-app/Assets/MoPub/Plugins/Android/MoPub.plugin/res* unity-sample-app/
validate

$UNITY_BIN -gvh_disable -projectPath $PROJECT_PATH -force-free -quit -batchmode -logFile $EXPORT_LOG \
           -importPackage $PROJECT_PATH/external-dependency-manager-*.unitypackage \
           -exportPackage $EXPORT_FOLDERS_MAIN $DEST_PACKAGE >& /dev/null
validate_without_exit "Building the unity package has failed, please check $EXPORT_LOG\nMake sure Unity isn't running when invoking this script!"
return_code=$?

echo -e "Cleaning any changes to PlayServicesResolver/ and ExternalDependencyManager/...\n"
git checkout $PROJECT_PATH/Assets/PlayServicesResolver/Editor/ \
             $PROJECT_PATH/Assets/ExternalDependencyManager/Editor/

echo -e "Putting Android resources back..."
mv unity-sample-app/res* unity-sample-app/Assets/MoPub/Plugins/Android/MoPub.plugin/
validate

if [[ $return_code -ne 0 ]]; then
  print_red_line "Exporting failed!"
  exit $return_code
else
  print_export_finished "$DEST_PACKAGE"
fi

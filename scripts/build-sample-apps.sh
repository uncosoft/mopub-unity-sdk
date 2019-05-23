#!/usr/bin/env bash
my_dir="$(dirname $0)"
source "$my_dir/validate.sh"
source "$my_dir/print_helpers.sh"

ensure_unity_bin

PROJECT_PATH="$PWD/unity-sample-app"
OUT_DIR="$PROJECT_PATH/Build"
BUILD_LOG_NAME="buildlog.txt"

function build_sample_app
{
  platform=$1
  last_commit=`git rev-parse --short HEAD`
  build_log=$my_dir/$platform$BUILD_LOG_NAME
  $UNITY_BIN -buildTarget $platform -executeMethod MoPubSampleBuild.PerformBuild lastCommit=$last_commit -projectPath $PROJECT_PATH -gvh_disable -force-free -quit -batchmode -logFile $build_log >& /dev/null
  ls $OUT_DIR/*$platform*$last_commit* >& /dev/null
  validate_without_exit "Building the $platform sample app has failed, please check $build_log\nMake sure Unity isn't running when invoking this script!"
}

print_blue_line "Building sample apps..."

build_sample_app Android
build_sample_app iOS

print_green_line "Done building sample apps!"

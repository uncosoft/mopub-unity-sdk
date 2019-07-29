#!/usr/bin/env bash
my_dir="$(dirname $0)"
source "$my_dir/build-android.sh"

$my_dir/unity-export-package.sh
validate "Exporting the package failed."

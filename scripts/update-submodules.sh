#!/usr/bin/env bash
my_dir="$(dirname $0)"
source "$my_dir/validate.sh"

PROJECTS=( "mopub-android-sdk" "mopub-ios-sdk" "mopub-android" "mopub-ios" )

for PROJECT in "${PROJECTS[@]}"
do
  if [ -d "$PROJECT" ]; then
    echo "Updating ${PROJECT}..."
    git --git-dir ${PROJECT}/.git checkout master
    git --git-dir ${PROJECT}/.git pull
    echo ""
  fi
done

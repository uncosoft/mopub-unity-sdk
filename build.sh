#!/usr/bin/env bash

git submodule update
if [[ $? -ne 0 ]]; then
    echo "Updating git submodules failed, fix before continuing"
    exit 1
fi

./mopub-android-sdk-unity-build.sh
if [[ $? -ne 0 ]]; then
    echo "Android build failed, fix before continuing"
    exit 1
fi

./mopub-ios-sdk-unity-build.sh
if [[ $? -ne 0 ]]; then
    echo "iOS build failed, fix before continuing"
    exit 1
fi

./unity-export-package.sh
if [[ $? -ne 0 ]]; then
    echo "Exporting the package failed"
    exit 1
fi

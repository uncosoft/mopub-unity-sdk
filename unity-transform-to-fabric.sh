#!/usr/bin/env bash

if [[ ! -d "out" || ! -f "out/MoPubUnity.unitypackage" ]]; then
    echo "Please export the unity package using 'unity-export-package.sh'"
    exit 1
fi

if [[ -d out/transform ]]; then
    rm -rf out/transform
fi

mkdir -p out/transform

cd out/transform
cp ../MoPubUnity.unitypackage mopub.tar.gz
tar -xf mopub.tar.gz
rm mopub.tar.gz

find . -name pathname -print0 | xargs -0 awk '{print $1, FILENAME}' | while read pathname filename; do
    parent="$(dirname "$filename")"

    if [[ $pathname == *"ThirdParty"* ]] || [[ $pathname == *"/PostBuildiOS"* ]]; then
	echo "Removing $filename ($pathname), and parent dir $parent"
	rm -rf $filename
	rm -rf $parent
    fi

    if [[ $pathname == *"/MoPubPostBuildiOS"* ]]; then
	echo "Modifying MoPubPostBuildiOS"
	sed -i -e 's/using MoPubInternal\.Editor\.Postbuild/using Fabric\.Internal\.Editor\.Postbuild/' $parent/asset
	sed -i -e 's/using MoPubInternal\.Editor\.ThirdParty\.xcodeapi/using Fabric\.Internal\.Editor\.ThirdParty\.xcodeapi/' $parent/asset
    fi

    if [[ $pathname == *"/MoPubManager"* ]]; then
	echo "Modifying MoPubManager"
	sed -i -e 's/MoPubInternal\.ThirdParty\.MiniJSON\.Json\.Deserialize/Fabric\.Internal\.ThirdParty\.MiniJSON\.Json\.Deserialize/' $parent/asset
    fi

    if [[ $pathname == *"/MoPubAndroid"* ]] || [[ $pathname == *"/MoPubBinding"* ]]; then
	echo "Modifying MoPubAndroid, MoPubBinding"
	sed -i -e 's/MoPubInternal\.ThirdParty\.MiniJSON\.Json\.Serialize/Fabric\.Internal\.ThirdParty\.MiniJSON\.Json\.Serialize/' $parent/asset
    fi
done

tar -zcf MoPubUnityFabric.unitypackage *
mv MoPubUnityFabric.unitypackage ../

cd ..
rm -rf transform

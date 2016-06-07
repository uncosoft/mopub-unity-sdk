#!/usr/bin/env bash

if [[ ! -d "out" || ! -f "out/MoPubUnity.unitypackage" ]]; then
    echo "Please export the unity package using 'unity-export-package.sh'"
    exit 1
fi

if [[ -d out/transform ]]; then
    rm -rf out/transform
fi

mkdir -p out/transform

# Unpack the mopub package.

cd out/transform
cp ../MoPubUnity.unitypackage mopub.tar.gz
tar -xf mopub.tar.gz
rm mopub.tar.gz

# Remove all files that exist in Fabric; namely MiniJSON and the PBX manipulator.

find . -name pathname -print0 | xargs -0 awk '{print $1, FILENAME}' | while read pathname filename; do
    parent="$(dirname "$filename")"

    # Remove third-party dependencies that exist in Fabric, and remove the PostBuildiOS.cs base file, it also exists in
    # Fabric.

    if [[ $pathname == *"ThirdParty"* ]] || [[ $pathname == *"/PostBuildiOS"* ]]; then
	echo "Removing $filename ($pathname), and parent dir $parent"
	rm -rf $filename
	rm -rf $parent
    fi

    # Replace the references to MiniJSON and PBX with Fabric-namespced versions.

    if [[ $pathname == *"/MoPubPostBuildiOS"* ]]; then
	echo "Modifying MoPubPostBuildiOS"
	sed -i -e 's/using MoPubInternal\.Editor\.Postbuild/using Fabric\.Internal\.Editor\.Postbuild/' $parent/asset
	sed -i -e 's/using MoPubInternal\.Editor\.ThirdParty\.xcodeapi/using Fabric\.Internal\.Editor\.ThirdParty\.xcodeapi/' $parent/asset
	sed -i -e 's/: PostBuildiOS/: Fabric\.Internal\.Editor\.Postbuild\.PostBuildiOS/' $parent/asset
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

# Repack.

tar -zcf MoPubUnityFabric.unitypackage *
mv MoPubUnityFabric.unitypackage ../

cd ..
rm -rf transform

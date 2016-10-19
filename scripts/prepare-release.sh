#!/bin/bash

set -e

VERSION=$1
XAMARIN_VERSION=$2

EXPECTED_ARGS=2

if [ $# -ne $EXPECTED_ARGS ]
then
echo "usage: scripts/prepare-release.sh <version> <plugin-version>"
exit
fi
echo "new Xamarin Plugin version: ${XAMARIN_VERSION}"
echo "using SDK version   : ${VERSION}"


remove_version_suffix() {
	version=$1
	# some of the versions can not contain a version suffix such as SNAPSHOT/BETA. this 
	# will strip off any such suffix.
    echo $version | sed 's/^\([0-9]\.[0-9][0-9]*\.[0-9]\).*$/\1/'
}

update_assembly_info() {
	xamarin_version_without_suffix=$(remove_version_suffix $XAMARIN_VERSION})
	find . -name AssemblyInfo.cs \
    | xargs sed -i.bak \
         "s/AssemblyVersion(\"[0-9]\.[0-9][0-9]*\.[0-9].*\")/AssemblyVersion(\"${xamarin_version_without_suffix}.0\")/g"
	find . -name AssemblyInfo.cs \
    | xargs sed -i.bak \
         "s/AssemblyFileVersion(\"[0-9]\.[0-9][0-9]*\.[0-9].*\")/AssemblyFileVersion(\"${xamarin_version_without_suffix}.0\")/g"
    find . -name AssemblyInfo.cs.bak | xargs rm --
}

update_package_config_versions() {
	xamarin_version_without_suffix=$(remove_version_suffix $XAMARIN_VERSION})
	find . -name '*.config' \
    | xargs sed -i.bak -E \
        "s/(<package id=\"?Scandit\..* version=\"?)[0-9]\.[0-9][0-9]*\.[0-9]*\.[0-9][0-9]*(\"?) /\1${xamarin_version_without_suffix}.0\2 /g"
    find . -name '*.config.bak' | xargs rm --
}

update_project_json_versions() {
	xamarin_version_without_suffix=$(remove_version_suffix $XAMARIN_VERSION})
	find . -name '*.json' \
    | xargs sed -i.bak -E \
        "s/(\"Scandit\.(BarcodePicker|Recognition|BarcodePicker\.Unified)\" *: *)\"(.*)\"/\1\"${xamarin_version_without_suffix}\"/g"
	find . -name '*.json' \
    | xargs sed -i.bak -E \
        "s/(\"Scandit\.(BarcodePicker|Recognition|BarcodePicker\.Unified))\/(.*)\"/\1${xamarin_version_without_suffix}\"/g"
    find . -name '*.json.bak' | xargs rm --
}

update_nuget_targets() {
	xamarin_version_without_suffix=$(remove_version_suffix $XAMARIN_VERSION})
	find . -name '*.nuget.targets' \
    | xargs sed -i.bak -E \
        "s/(Scandit\.(BarcodePicker|Recognition)\\\\)([^\\\\]*)(\\\\)/\1${xamarin_version_without_suffix}\4/g"
    find . -name '*.nuget.targets.bak' | xargs rm --
}

update_windows_csproj_files() {
	xamarin_version_without_suffix=$(remove_version_suffix $XAMARIN_VERSION})
    all_csproj_files=$(find . -name '*.csproj')
    sed -i.bak -E \
        "s/Scandit\.BarcodePicker\.[0-9]\.[0-9][0-9]*\.[0-9]*\.[0-9][0-9]*/Scandit.BarcodePicker.${xamarin_version_without_suffix}.0/g" \
        $all_csproj_files
    sed -i.bak -E \
        "s/Scandit\.Recognition\.[0-9]\.[0-9][0-9]*\.[0-9]*\.[0-9][0-9]*/Scandit.Recognition.${xamarin_version_without_suffix}.0/g" \
        $all_csproj_files
    sed -i.bak -E \
        "s/(<Reference Include=\"Scandit[^,]*,.*Version=)[0-9](\.[0-9][0-9]*)*/\1${xamarin_version_without_suffix}.0/g" \
        $all_csproj_files
    sed -i.bak -E \
        "s/(<Reference Include=\"VideoInputRT.Interop,.*Version=)[0-9](\.[0-9][0-9]*)*/\1${xamarin_version_without_suffix}.0/g" \
        $all_csproj_files
    sed -i.bak -E \
        "s/(<HintPath>.*Scandit\.BarcodePicker\.Xamarin\.)[0-9](\.[0-9][0-9]*)*/\1${xamarin_version_without_suffix}/g" \
        $all_csproj_files
    sed -i.bak -E \
        "s/(<HintPath>.*Scandit\.BarcodePicker\.Unified\.)[0-9](\.[0-9][0-9]*)*/\1${xamarin_version_without_suffix}/g" \
        $all_csproj_files
    sed -i.bak -E \
        "s/<ReleaseVersion>[0-9](\.[0-9][0-9]*)*<\/ReleaseVersion>/<ReleaseVersion>${xamarin_version_without_suffix}.0<\/ReleaseVersion>/g" \
        $all_csproj_files
    find . -name '*.csproj.bak' | xargs rm --
}

update_sln() {
    xamarin_version_without_suffix=$(remove_version_suffix $XAMARIN_VERSION})
    find . -name '*\.sln' | xargs \
        sed -i.bak -E "s/( *version *= *)[0-9]*(\.[0-9]*)*/\1${xamarin_version_without_suffix}/"
    find . -name '*\.sln\.bak' | xargs rm --
}

update_assembly_info
update_package_config_versions
update_windows_csproj_files
update_project_json_versions
update_nuget_targets
update_sln


echo 'updated project version and iOS/Android SDK versions for Xamarin Plugin'
echo 'Please verify that the changes are correct'
git diff
echo ''
echo ''
echo 'You may commit the changes now with:'
echo "   git commit -a -m 'bump version to ${XAMARIN_VERSION} (using ScanditSDK=${VERSION})'"


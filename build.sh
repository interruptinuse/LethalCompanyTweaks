#!/usr/bin/env bash

set -ex
shopt -s nullglob

OUTPUT_DIR=bin/Debug/netstandard2.1
DLLS=(KeepTerminalOn TimeMultiplier)
SUBDIRS=() ; for DLL in "${DLLS[@]}" ; do SUBDIRS+=("$(git rev-parse --show-toplevel)/${DLL}") ; done

RELEASE_NAME="LethalCompanyTweaks"

MOD_UPDATE="$(git log -1 --pretty='%cd' --date=format:'%Y%m%d' -- "${SUBDIRS[@]}")"
LC_VER="40"
BEPINEX_VER="5.4.21"
ARCHIVE_NAME="${RELEASE_NAME}-${MOD_UPDATE}-v${LC_VER}-${BEPINEX_VER}"

BEPINEX_ZIP="BepInEx_x64_${BEPINEX_VER}.0.zip"
BEPINEX_URL="https://github.com/BepInEx/BepInEx/releases/download/v${BEPINEX_VER}/${BEPINEX_ZIP}"

ROOT_DIR=$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" &>/dev/null && pwd)

dotnet build

mkdir -p "release"
pushd "release"
RELEASE_DIR="$(pwd)"
wget --no-config --no-clobber -- "${BEPINEX_URL}"

mkdir -p "$ARCHIVE_NAME"
pushd "$ARCHIVE_NAME"
ARCHIVE_DIR="$(pwd)"
unzip "${RELEASE_DIR}/${BEPINEX_ZIP}" -x changelog.txt
mkdir -p BepInEx/{plugins,config}
for DLL in "${DLLS[@]}" ; do cp "${ROOT_DIR}/${OUTPUT_DIR}/${DLL}."{dll,pdb} BepInEx/plugins ; done
cp -t BepInEx/config -- "${ROOT_DIR}"/config/* || true
popd

zip -9r "$ARCHIVE_NAME.zip" "$ARCHIVE_NAME"

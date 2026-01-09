#!/bin/bash

BUILD_DIR="$1"
APP_NAME="BatchProcess3"
APP_BUNDLE="${BUILD_DIR}/../${APP_NAME}.app"

# Remove old app
rm -rf "${APP_BUNDLE}"

# Create app folders
mkdir -p "${APP_BUNDLE}/Contents/MacOS"
mkdir -p "${APP_BUNDLE}/Contents/Resources"

# Copy entire build folder
cp -r "${BUILD_DIR}" "${APP_BUNDLE}/Contents/MacOS"

# Copy Icon
cp "${BUILD_DIR}/Assets/Images/icon.icns" "${APP_BUNDLE}/Contents/Resources/app.icns"

# Copy Info.plist
cp "${BUILD_DIR}/Assets/Info.plist" "${APP_BUNDLE}/Contents/Info.plist"

echo "${BUILD_DIR} ${APP_NAME}"


#!/bin/bash

# Definir las rutas relativas
RELATIVE_ROOT=$(pwd)
RELATIVE_APP_NAME="$RELATIVE_ROOT/App/GeneracionNumeracion.app"
RELATIVE_PUBLISH_OUTPUT_DIRECTORY="$RELATIVE_ROOT/GeneracionNumeracionAvalonia/bin/Release/net8.0/osx-arm64/publish/."
RELATIVE_PROJECT="$RELATIVE_ROOT/GeneracionNumeracionAvalonia/GeneracionNumeracionAvalonia.csproj"
INFO_PLIST="$RELATIVE_ROOT/package/Macos/Info.plist"
ICON_FILE="$RELATIVE_ROOT/package/Macos/Counter.icns"


echo "$ABSOLUTE_PATH"
echo "$RELATIVE_APP_NAME"
echo "$RELATIVE_PUBLISH_OUTPUT_DIRECTORY"
echo "$RELATIVE_PROJECT"
echo "$INFO_PLIST"
echo "$ICON_FILE"

# Limpiar carpetas
rm -rf "$RELATIVE_APP_NAME/Contents/MacOS/"
rm -rf "$RELATIVE_APP_NAME/Contents/CodeResources"
rm -rf "$RELATIVE_APP_NAME/Contents/_CodeSignature"
rm -rf "$RELATIVE_APP_NAME/Contents/embedded.provisionprofile"
mkdir -p "$RELATIVE_APP_NAME/Contents/Frameworks/"
mkdir "$RELATIVE_APP_NAME/Contents/Resources"

# Compilar la aplicación
dotnet publish "$RELATIVE_PROJECT" -c Release -f net8.0 -r osx-arm64 --self-contained=true -p:PublishSingleFile=true -p:UseAppHost=true

# Mover la aplicación
mkdir -p "$RELATIVE_APP_NAME/Contents/MacOS/"
cp -R -f "$RELATIVE_PUBLISH_OUTPUT_DIRECTORY"/* "$RELATIVE_APP_NAME/Contents/MacOS/"
rm -r "$RELATIVE_APP_NAME/Contents/MacOS"/*.pdb
rm -rf "$RELATIVE_APP_NAME/Contents/MacOS/publish"
cp "$INFO_PLIST" "$RELATIVE_APP_NAME/Contents/"
cp "$ICON_FILE" "$RELATIVE_APP_NAME/Contents/Resources/"

# echo "[INFO] Creando GeneracionNumeracion.pkg"
productbuild --component "$RELATIVE_APP_NAME" /Applications "$RELATIVE_ROOT/GeneracionNumeracion.pkg"
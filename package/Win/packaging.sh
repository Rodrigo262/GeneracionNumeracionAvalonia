#!/bin/bash

# Definir las rutas relativas
RELATIVE_ROOT=$(pwd)
RELATIVE_PUBLISH_OUTPUT_DIRECTORY="$RELATIVE_ROOT/GeneracionNumeracionAvalonia/bin/Release/net8.0/win-x64/publish/."
RELATIVE_PROJECT="$RELATIVE_ROOT/GeneracionNumeracionAvalonia/GeneracionNumeracionAvalonia.csproj"

echo "$RELATIVE_ROOT"
echo "$RELATIVE_PUBLISH_OUTPUT_DIRECTORY"
echo "$RELATIVE_PROJECT"

# Limpiar carpetas
rm -rf "$RELATIVE_ROOT/*.exe"

# Compilar la aplicación
dotnet publish "$RELATIVE_PROJECT" -c Release -f net8.0 -r win-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true /p:IncludeNativeLibrariesForSelfExtract=true

# Mover la aplicación
cp -R -f "$RELATIVE_PUBLISH_OUTPUT_DIRECTORY"/*.exe "$RELATIVE_ROOT"

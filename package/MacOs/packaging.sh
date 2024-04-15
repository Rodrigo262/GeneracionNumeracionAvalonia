#cleanup folders
APP_NAME="../../GeneracionNumeracionAvalonia/App/GeneracionNumeracion.app"
PUBLISH_OUTPUT_DIRECTORY="../../GeneracionNumeracionAvalonia/GeneracionNumeracionAvalonia/bin/Release/net8.0/osx-arm64/publish/."
PROJECT="../../GeneracionNumeracionAvalonia/GeneracionNumeracionAvalonia.csproj"
INFO_PLIST="Info.plist"
ICON_FILE="Counter.icns"

#cleanup folders
rm -rf "$APP_NAME/Contents/MacOS/" 
rm -rf "$APP_NAME/Contents/CodeResources" 
rm -rf "$APP_NAME/Contents/_CodeSignature" 
rm -rf "$APP_NAME/Contents/embedded.provisionprofile" 
mkdir -p "$APP_NAME/Contents/Frameworks/"
mkdir "$APP_NAME/Contents/Resources"

#Build app
dotnet publish "$PROJECT" -c Release -f net8.0 -r osx-arm64 --self-contained=true -p:PublishSingleFile=true -p:UseAppHost=true


#Move app
mkdir -p "$APP_NAME/Contents/MacOS/"
cp -R -f "../../GeneracionNumeracionAvalonia/GeneracionNumeracionAvalonia/bin/Release/net8.0/osx-arm64/publish/"/* "$APP_NAME/Contents/MacOS/"
echo "++++++++"
echo "$PUBLISH_OUTPUT_DIRECTORY"
rm -rf "$APP_NAME/Contents/MacOS/publish"
cp "$INFO_PLIST" "$APP_NAME/Contents/Info.plist"
cp "$ICON_FILE" "$APP_NAME/Contents/Resources/$ICON_FILE"

echo "[INFO] Creating GeneracionNumeracion.pkg"
productbuild --component "$APP_NAME" /Applications "../../GeneracionNumeracionAvalonia/GeneracionNumeracion.pkg"
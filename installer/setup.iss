[Setup]
AppName=GeneracionNumeracion
AppVersion=1.0.0
DefaultDirName={autopf}\GeneracionNumeracion
DefaultGroupName=GeneracionNumeracion
OutputDir=output
OutputBaseFilename=GeneracionNumeracionSetup
SetupIconFile=..\GeneracionNumeracionAvalonia\Assets\counter.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Files]
Source: "..\GeneracionNumeracionAvalonia\bin\Release\net8.0\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\GeneracionNumeracion"; Filename: "{app}\GeneracionNumeracion.exe"
Name: "{commondesktop}\GeneracionNumeracion"; Filename: "{app}\GeneracionNumeracion.exe"

[Run]
Filename: "{app}\GeneracionNumeracion.exe"; Description: "Launch GeneracionNumeracion"; Flags: nowait postinstall skipifsilent
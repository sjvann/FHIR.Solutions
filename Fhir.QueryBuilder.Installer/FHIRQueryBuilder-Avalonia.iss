; Inno Setup 6 — Avalonia 桌面版（離線自包含）。
; 建置：py build_installer_avalonia.py（會先 dotnet publish 至 staging）

#define MyAppName "FHIR Query Builder Desktop"
; 預設值僅供手動 ISCC；build_installer_avalonia.py 會傳入 /DMyAppVersion=x.y.z 覆寫
#ifndef MyAppVersion
#define MyAppVersion "1.0.0"
#endif
#define MyAppPublisher "FHIR.Solutions"
#define MyAppExeName "Fhir.QueryBuilder.App.Avalonia.exe"
#define StagingRelativePath "..\artifacts\FHIRQueryBuilderAvaloniaInstallerStaging\app"

[Setup]
AppId={{B7C4D1E2-3F5A-4B6C-9D8E-0F1A2B3C4D5E}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=no
OutputDir=..\artifacts
OutputBaseFilename=FHIRQueryBuilder-Avalonia-Setup-{#MyAppVersion}
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequired=admin
SetupIconFile=..\Fhir.QueryBuilder.App.Blazor\wwwroot\favicon.ico
UninstallDisplayIcon={app}\{#MyAppExeName},0

[Languages]
Name: "chinesetraditional"; MessagesFile: "compiler:Languages\ChineseTraditional.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#StagingRelativePath}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "安裝手冊-Avalonia.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "使用手冊-Avalonia.md"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

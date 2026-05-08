; Inno Setup 6 — 建置前請先執行 build_installer.py 產生 staging 目錄。
; 編譯：ISCC.exe FHIRQueryBuilder.iss（或由 build_installer.py 呼叫）

#define MyAppName "FHIR Query Builder"
; 預設值僅供手動 ISCC；build_installer.py 會傳入 /DMyAppVersion=x.y.z 覆寫
#ifndef MyAppVersion
#define MyAppVersion "1.0.0"
#endif
#define MyAppPublisher "FHIR.Solutions"
#define MyAppExeName "FHIRQueryBuilder.exe"
#define StagingRelativePath "..\artifacts\FHIRQueryBuilderInstallerStaging\app"

[Setup]
AppId={{E8F3B2C1-9D4A-4E7F-B6C5-123456789ABC}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=no
OutputDir=..\artifacts
OutputBaseFilename=FHIRQueryBuilder-Setup-{#MyAppVersion}
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequired=admin
; 與主程式相同圖示（另需 exe 內嵌 ApplicationIcon，否則「已安裝的應用程式」可能無圖）
SetupIconFile=..\Fhir.QueryBuilder.App.Blazor\wwwroot\favicon.ico
UninstallDisplayIcon={app}\{#MyAppExeName},0

[Languages]
; Inno Setup 安裝目錄下 Languages\ChineseTraditional.isl（完整安裝即內建）
Name: "chinesetraditional"; MessagesFile: "compiler:Languages\ChineseTraditional.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#StagingRelativePath}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "安裝手冊.md"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

; GPU Fan Controller - Inno Setup Installer Script
; This script creates a Windows installer with shortcuts, Start Menu entries, and uninstaller

#define MyAppName "GPU Fan Controller"
#define MyAppVersion "2.3.2"
#define MyAppPublisher "GPU Fan Controller"
#define MyAppURL "https://github.com/yourusername/GPUFanController"
#define MyAppExeName "GPUFanController.exe"
#define MyAppExeNameConsole "GPUFanControllerConsole.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
AppId={{A7B8C9D0-E1F2-4A5B-9C8D-7E6F5A4B3C2D}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=LICENSE.txt
OutputDir=installer_output
OutputBaseFilename=GPUFanController_Setup_v{#MyAppVersion}
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
PrivilegesRequiredOverridesAllowed=dialog
SetupIconFile=app.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
UninstallDisplayName={#MyAppName}
VersionInfoVersion={#MyAppVersion}
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "desktopiconconsole"; Description: "Create Desktop Icon for Console Version"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 6.1; Check: not IsAdminInstallMode

[Files]
; Main GUI application files
Source: "bin\Release\net6.0-windows\win-x64\publish\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\net6.0-windows\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "*.pdb"

; Console application files
Source: "bin\Release\net6.0\win-x64\publish\{#MyAppExeNameConsole}"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\net6.0\win-x64\publish\*"; DestDir: "{app}\console"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "*.pdb"

; Documentation
Source: "README.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "FEATURES.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "QUICKSTART.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "PROJECT_SUMMARY.md"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
; Start Menu icons
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Comment: "Control GPU fan speed with visual interface"
Name: "{group}\{#MyAppName} Console"; Filename: "{app}\console\{#MyAppExeNameConsole}"; Comment: "Control GPU fan speed with console interface"
Name: "{group}\Quick Start Guide"; Filename: "{app}\QUICKSTART.md"
Name: "{group}\Documentation"; Filename: "{app}\README.md"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"

; Desktop icons (optional)
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon; Comment: "Control GPU fan speed"
Name: "{autodesktop}\{#MyAppName} Console"; Filename: "{app}\console\{#MyAppExeNameConsole}"; Tasks: desktopiconconsole; Comment: "Control GPU fan speed (Console)"

; Quick Launch icon (optional, for older Windows)
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
; Option to run the application after installation
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent runascurrentuser shellexec

[UninstallDelete]
Type: filesandordirs; Name: "{app}"

[Code]
function InitializeSetup(): Boolean;
var
  ResultCode: Integer;
  DotNetInstalled: Boolean;
begin
  Result := True;
  
  // Check if .NET 6.0 Runtime is installed (optional check)
  // Note: The published executables are self-contained, so .NET is not required
  // This is just informational
  
  MsgBox('GPU Fan Controller Setup' + #13#10 + #13#10 + 
         'This application requires Administrator privileges to access GPU hardware.' + #13#10 + #13#10 +
         'Features:' + #13#10 +
         '• Multi-GPU support (NVIDIA, AMD, Intel)' + #13#10 +
         '• Automatic fan curves (4 profiles)' + #13#10 +
         '• Manual fan control with slider' + #13#10 +
         '• Real-time temperature monitoring' + #13#10 +
         '• GUI and Console versions included' + #13#10 + #13#10 +
         'IMPORTANT: Always monitor GPU temperatures!', 
         mbInformation, MB_OK);
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    // Any post-installation tasks can go here
  end;
end;

function InitializeUninstall(): Boolean;
begin
  // Return True immediately - Windows will show its own confirmation
  // No need for custom confirmation dialog
  Result := True;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep = usPostUninstall then
  begin
    MsgBox('GPU Fan Controller has been successfully uninstalled.' + #13#10 + #13#10 +
           'Thank you for using GPU Fan Controller!', 
           mbInformation, MB_OK);
  end;
end;

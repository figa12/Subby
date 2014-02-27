!define APP_NAME "Subby"

; The name of the installer
Name "Subby Installer"

; The file to write
OutFile "Setup.exe"

; The default installation directory
InstallDir $PROGRAMFILES\Subby

; Request application privileges for Windows Vista
RequestExecutionLevel admin

;--------------------------------

; Pages

Page directory
Page instfiles

;--------------------------------

; The stuff to install
Section "" ;No components page, name is not important

  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put file there
  File "Subby.exe"
  File "subby.ico"
  
  ;write uninstall information to the registry
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}" "DisplayName" "${APP_NAME} (remove only)"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}" "UninstallString" "$INSTDIR\Uninstall.exe"
  WriteRegStr HKCR "*\shell\${APP_NAME}\command\" "" "$INSTDIR\${APP_NAME}.exe %1"
  
  ;Delete old entries (from older version, before name change)
  DeleteRegKey HKEY_LOCAL_MACHINE "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\DatSub"
  DeleteRegKey HKEY_CLASSES_ROOT "*\shell\DatSub"
 
  WriteUninstaller "$INSTDIR\Uninstall.exe"
  
SectionEnd ; end the section


;--------------------------------    
;Uninstaller Section  
Section "Uninstall"
 
;Delete Files 
  RMDir /r "$INSTDIR\*.*"    
 
;Remove the installation directory
  RMDir "$INSTDIR"
 
;Delete Uninstaller And Unistall Registry Entries
  DeleteRegKey HKEY_LOCAL_MACHINE "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${APP_NAME}"
  DeleteRegKey HKEY_CLASSES_ROOT "*\shell\${APP_NAME}"
;Delete old entries (from older version, before name change)
  DeleteRegKey HKEY_LOCAL_MACHINE "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\DatSub"
  DeleteRegKey HKEY_CLASSES_ROOT "*\shell\DatSub"
 
SectionEnd
 
;--------------------------------  

;Function that calls a messagebox when installation finished correctly
Function .onInstSuccess
  MessageBox MB_OK "You have successfully installed ${APP_NAME}. Right-click a movie to get started."
FunctionEnd
 
Function un.onUninstSuccess
  MessageBox MB_OK "You have successfully uninstalled ${APP_NAME}."
FunctionEnd  
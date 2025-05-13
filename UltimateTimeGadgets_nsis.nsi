# define the name of the installer
Outfile "UltimateTimeGadgets.exe"
Icon "UltimateTimeGadgets\Resources\UltimateTimeGadgets.ico"

InstallDir $TEMP\UltimateTimeGadgetsSetup

AutoCloseWindow true

# default section
Section

HideWindow

SetOutPath $INSTDIR

File /r UltimateTimeGadgetsSetup\Release\*.*
ExecWait "$INSTDIR\setup.exe"

RMDir /r "$INSTDIR"

SectionEnd

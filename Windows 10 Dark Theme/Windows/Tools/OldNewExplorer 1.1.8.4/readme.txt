THE OLDNEWEXPLORER FOR WINDOWS 8.1 / WINDOWS 8 (X86/X64)

Installation:
---------------
Extract files into directory which all users can access.
Run OldNewExplorerCfg.exe, click Install

Uninstallation:
---------------
Run OldNewExplorerCfg.exe, click Uninstall

Skinning Notes:
---------------
OldNewExplorer can override UIFILES from most files loaded by explorer.exe.
To take advantage of this, add UIFILES to your ShellStyle.dll named the following way:
Example:
SHELL32.DLL UIFILE 23 -> SHELLSTYLE.DLL SHELL32_UIFILE 23
EXPLORERFRAME.DLL UIFILE 20482 -> SHELLSTYLE.DLL EXPLORERFRAME_UIFILE 20482

Note that OldNewExplorer can load other types of resources loaded by DirectUI, for example TWINUI.DLL TILETEMPLATE
Overriding all resources may require explorer.exe restart.

Note that OldNewExplorer's style, details, statusbar settings override shellstyle resources.

WARNING: custom resource types are not loaded from ShellStyle.dll if it has MUI resource pair.
Merge it with mui file and remove MUI resource.

Turn debug on:
---------------
[HKEY_CURRENT_USER\Software\Tihiy\OldNewExplorer]
"Debug"=dword:00000001

launch DbgView.exe (DebugView) to monitor which UIFILES are loaded


Version History
---------------
v1.1.8.2 26.07.2016
* Fix possible crash on process exit

v1.1.8.1 21.07.2016
* Support for build 14393

v1.1.8 03.04.2016
* Support for build 14295

v1.1.7.1 13.09.2015
* Fixed rare crash

v1.1.7 09.08.2015
* Fixed crashes

v1.1.6 03.08.2015
* Fixed deadlocking

v1.1.5.1 19.04.2015
* Fix for Windows 10 build 10049+

v1.1.5 21.03.2015
* Support for Windows 10 TP
* Option to replace navigation buttons

v1.1.0 20.07.2014
* Improved injection mechanisms to solve issues with some options not working in some cases

v1.0.7 14.02.2014
* Enabling glass on navigation bar removes padding around search box and reduces travel buttons padding
* New setting to remove Up button

v1.0.6 10.02.2014
* New settings to show navigation bar on glass (without ribbon only)

v1.0.5 02.02.2014
* O-N-E is now registered as BHO to initialize in all explorer windows
* New settings to hide caption text and icon

v1.0.4 05.12.2013
* Fixed: Folders can't be renamed when 'Use libraries; hide folders' is on (1.0.3 bug)

v1.0.3 29.11.2013
* 'Use libraries; hide folders' hides folders even with subfolders (explorer.exe restart no longer required)

v1.0.2 26.11.2013
* Removed 'Group network locations separately' - always on
* All options are off by default
* 'Use libraries; hide folders' hides folders even with subfolders (explorer.exe restart required)
* Fixed installation with spaces in path
* Fixed ONE crashing SIB

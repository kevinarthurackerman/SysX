@ECHO OFF
ECHO Deleting bin and obj subfolders from Source...
CD /D ..\src
FOR /D /R %%a IN ("*bin","*obj") DO echo "%%a" && rmdir /s /q "%%a"
PAUSE
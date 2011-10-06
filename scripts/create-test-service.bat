@echo off
sc.exe create TestService binPath= "%~dp0..\src\RunAsService\bin\Debug\RunAsService.exe"

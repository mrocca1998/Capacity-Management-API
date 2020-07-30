@echo off

robocopy %~1 %~2 /mir
IF ERRORLEVEL 8 exit 1
IF ERRORLEVEL 1 exit 0


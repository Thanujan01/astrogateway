@echo off
rem Windows counterpart of "Run Prototype.command". Starts local MongoDB, the FastAPI
rem account/mission API, the web portal, and the Unity prototype if one is available.
setlocal
set "TASK_ROOT=%~dp0"
where py >nul 2>nul
if %ERRORLEVEL%==0 (
  py -3 "%TASK_ROOT%tools\prototype\start.py" %*
) else (
  python "%TASK_ROOT%tools\prototype\start.py" %*
)
set "RESULT=%ERRORLEVEL%"
if not "%RESULT%"=="0" pause
exit /b %RESULT%

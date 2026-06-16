@echo off
setlocal

set "ROOT=C:\emsyne\Intern\AngularProject"
set "BACKEND_DIR=%ROOT%\backend\JustCRM.API"
set "FRONTEND_DIR=%ROOT%\Customer"

echo Starting JustCRM backend on http://localhost:5180 ...
start "JustCRM Backend" cmd /k "cd /d %BACKEND_DIR% && dotnet run --no-restore --launch-profile http"

echo Starting JustCRM frontend on http://127.0.0.1:4200 ...
start "JustCRM Frontend" cmd /k "cd /d %FRONTEND_DIR% && npm run start"

echo.
echo JustCRM is launching in two windows.
echo Frontend: http://127.0.0.1:4200
echo Backend:  http://localhost:5180
echo.
echo Close the opened command windows to stop the services.

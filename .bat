@echo off
cd /d C:\Users\SEBAS_DEV\source\repos\inventory-hub-api-async\publish

:: Ejecutar la API
start "" cmd /k dotnet InventoryHub.dll

:: Esperar unos segundos a que levante
timeout /t 3 >nul

:: Abrir navegador
start http://localhost:5000/index.html
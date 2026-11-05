@echo off
setlocal EnableDelayedExpansion
rem =====================================================================
rem  PharmaStock - Mise a jour d'une base existante (sans perte de donnees)
rem  Applique, dans l'ordre, tous les scripts du dossier migrations\.
rem  Usage : double-clic, ou   mettre_a_jour_base.bat [utilisateur] [port]
rem =====================================================================

cd /d "%~dp0"

set "MYSQL_USER=%~1"
if "%MYSQL_USER%"=="" set "MYSQL_USER=root"
set "MYSQL_PORT=%~2"
if "%MYSQL_PORT%"=="" set "MYSQL_PORT=3306"

rem --- Recherche du client mysql.exe ---------------------------------
set "MYSQL_EXE="
for %%M in (mysql.exe) do if not "%%~$PATH:M"=="" set "MYSQL_EXE=%%~$PATH:M"
if not defined MYSQL_EXE if exist "C:\xampp\mysql\bin\mysql.exe" set "MYSQL_EXE=C:\xampp\mysql\bin\mysql.exe"
if not defined MYSQL_EXE (
    for /d %%D in ("C:\Program Files\MySQL\MySQL Server *") do (
        if exist "%%~D\bin\mysql.exe" set "MYSQL_EXE=%%~D\bin\mysql.exe"
    )
)
if not defined MYSQL_EXE (
    for /d %%D in ("C:\wamp64\bin\mysql\mysql*") do (
        if exist "%%~D\bin\mysql.exe" set "MYSQL_EXE=%%~D\bin\mysql.exe"
    )
)
if not defined MYSQL_EXE (
    echo [ERREUR] mysql.exe introuvable.
    echo Ajoutez le dossier bin de MySQL au PATH, ou installez XAMPP / MySQL Server.
    goto :fin_erreur
)

echo Client MySQL : %MYSQL_EXE%
echo Utilisateur  : %MYSQL_USER%   Port : %MYSQL_PORT%
echo.
rem Mot de passe : variable MYSQL_PWD si deja definie, sinon demande une seule fois
if not defined MYSQL_PWD (
    echo Avec XAMPP le mot de passe root est vide : appuyez simplement sur Entree.
    set /p "MYSQL_PWD=Mot de passe MySQL : "
)
echo.

set /a NB=0
for %%F in (migrations\*.sql) do (
    echo --- Application de %%~nxF
    "%MYSQL_EXE%" -u %MYSQL_USER% "--password=!MYSQL_PWD!" -h 127.0.0.1 -P %MYSQL_PORT% --default-character-set=utf8mb4 pharmastock < "%%F"
    if errorlevel 1 (
        echo [ERREUR] Echec sur %%~nxF : la mise a jour est interrompue.
        goto :fin_erreur
    )
    set /a NB+=1
)

echo.
echo [OK] !NB! script^(s^) de migration applique^(s^). La base pharmastock est a jour.
pause
exit /b 0

:fin_erreur
echo.
pause
exit /b 1

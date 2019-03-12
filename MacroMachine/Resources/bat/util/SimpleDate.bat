@echo off

set Year=%date:~0,4%
set Month=%date:~5,2%
set Day=%date:~8,2%

exit /b %Year%%Month%%Day%
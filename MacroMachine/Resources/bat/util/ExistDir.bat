::---- ���C�� ----
@echo off

::�������`�F�b�N
if "%1" == "" goto ArgError

::���݂̃`�F�b�N
if exist %1 exit /b 1
if not exist %1 exit /b 0

exit /b %IsExist%

::---- �G���[���� ----
:ArgError
echo ArgumentError
echo �f�B���N�g�����w�肵�Ă��������B
exit /b -1
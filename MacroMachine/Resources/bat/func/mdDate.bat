::---- ���C�� ----
@echo off

::�f�B���N�g���̑��݂��`�F�b�N
set	DstDir=%1
call util/ExistDir %DstDir%
if %ErrorLevel% == 0 goto DirError

::���t���擾
call util/SimpleDate
set SimpleDate=%ErrorLevel%

::�t�@�C�����̎w����`�F�b�N
set Name=Noname
if "%2" == "" (
	set Name=
) else (
	set Name=%2
)

::�t�@�C���̑��݂��`�F�b�N
set DirPath=%DstDir%\%SimpleDate%_%Name%
call util/ExistDir %DirPath%
if %ErrorLevel% == 1 goto ExistError

::�t�@�C�����쐬
md %DirPath%

::�I��
exit /b 0

::---- �G���[���� ----
:DirError
echo DirError
echo �w�肵���f�B���N�g�������݂��܂���B
exit /b 0

:ExistError
echo FileExistError
echo �t�@�C�������ɑ��݂��܂��B
exit /b 0


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
set FilePath=%DstDir%\%SimpleDate%_%Name%.txt
call util/ExistDir %FilePath%
if %ErrorLevel% == 1 goto FileExistError

::�t�@�C�����쐬
type nul > %FilePath%

::�I��
exit /b 0

::---- �G���[���� ----
:DirError
echo DirError
echo �w�肵���f�B���N�g�������݂��܂���B
exit /b 0

:FileExistError
echo FileExistError
echo �t�@�C�������ɑ��݂��܂��B
exit /b 0


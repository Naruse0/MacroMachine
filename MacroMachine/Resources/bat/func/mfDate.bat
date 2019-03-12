::---- メイン ----
@echo off

::ディレクトリの存在をチェック
set	DstDir=%1
call util/ExistDir %DstDir%
if %ErrorLevel% == 0 goto DirError

::日付を取得
call util/SimpleDate
set SimpleDate=%ErrorLevel%

::ファイル名の指定をチェック
set Name=Noname
if "%2" == "" (
	set Name=
) else (
	set Name=%2
)

::ファイルの存在をチェック
set FilePath=%DstDir%\%SimpleDate%_%Name%.txt
call util/ExistDir %FilePath%
if %ErrorLevel% == 1 goto FileExistError

::ファイルを作成
type nul > %FilePath%

::終了
exit /b 0

::---- エラー処理 ----
:DirError
echo DirError
echo 指定したディレクトリが存在しません。
exit /b 0

:FileExistError
echo FileExistError
echo ファイルが既に存在します。
exit /b 0


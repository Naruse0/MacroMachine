::---- メイン ----
@echo off

::引数をチェック
if "%1" == "" goto ArgError

::存在のチェック
if exist %1 exit /b 1
if not exist %1 exit /b 0

exit /b %IsExist%

::---- エラー処理 ----
:ArgError
echo ArgumentError
echo ディレクトリを指定してください。
exit /b -1
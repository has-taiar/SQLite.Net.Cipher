rmdir /s /q "SQLite.Net.Cipher"
rmdir /s /q "output" 

mkdir SQLite.Net.Cipher
copy /y ..\src\SQLite.Net.Cipher\bin\Release\SQLite.Net.Cipher.dll SQLite.Net.Cipher

pause
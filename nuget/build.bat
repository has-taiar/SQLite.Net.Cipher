rmdir /s /q "output" 

@mkdir output
..\.nuget\nuget pack SQLite.Net.Cipher.nuspec -o output

pause

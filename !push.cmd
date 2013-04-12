for /r %%x in (src\DelegateDecompiler\bin\Release\*.nupkg) do .nuget\NuGet.exe push %%x -ApiKey %1

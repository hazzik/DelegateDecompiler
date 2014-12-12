for /r %%x in (src\DelegateDecompiler\bin\Release\*.nupkg) do .nuget\NuGet.exe push "%%x" -ApiKey %1
for /r %%x in (src\DelegateDecompiler.EntityFramework\bin\Release\*.nupkg) do .nuget\NuGet.exe push "%%x" -ApiKey %1


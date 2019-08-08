for %%x in (src\DelegateDecompiler\bin\Release\*.nupkg) do dotnet nuget push "%%x" -s https://nuget.org -k %1 


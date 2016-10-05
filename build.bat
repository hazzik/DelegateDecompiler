set EnableNuGetPackageRestore=true 
set msbuild="%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild"
%msbuild% /p:Configuration=Release /p:BuildPackage=True

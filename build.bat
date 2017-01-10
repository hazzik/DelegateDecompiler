set msbuild="%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild"
.nuget\nuget.exe restore
%msbuild% /p:Configuration=Release
.nuget\nuget.exe pack src/DelegateDecompiler/DelegateDecompiler.csproj -Properties Configuration=Release
.nuget\nuget.exe pack src/DelegateDecompiler.EntityFramework/DelegateDecompiler.EntityFramework.csproj -Properties Configuration=Release

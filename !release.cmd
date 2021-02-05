dotnet pack -c Release -o Release DelegateDecompiler.sln
@for %%x in (Release\*.nupkg) do (
	dotnet nuget push "%%x" -s https://nuget.org -k %1
)
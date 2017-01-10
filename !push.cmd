for %%x in (*.nupkg) do .nuget\NuGet.exe push "%%x" %1 -Source https://nuget.org


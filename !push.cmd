for %%x in (*.nupkg) do dotnet nuget push "%%x" %1 -Source https://nuget.org


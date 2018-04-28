nunit3-console ^
 src\DelegateDecompiler.Tests\bin\Release\net40\DelegateDecompiler.Tests.dll ^
 src\DelegateDecompiler.Tests.VB\bin\Release\net40\DelegateDecompiler.Tests.VB.dll ^
 src\DelegateDecompiler.Tests\bin\Release\net45\DelegateDecompiler.Tests.dll ^
 src\DelegateDecompiler.Tests.VB\bin\Release\net45\DelegateDecompiler.Tests.VB.dll ^
 src\DelegateDecompiler.EntityFramework.Tests\bin\Release\net45\DelegateDecompiler.EntityFramework.Tests.dll ^
 --result=DelegateDecompiler.testsresults.xml;format=AppVeyor

dotnet test --no-build -c Release -f netcoreapp2.0 src\DelegateDecompiler.Tests 
dotnet test --no-build -c Release -f netcoreapp2.0 src\DelegateDecompiler.Tests.VB
dotnet test --no-build -c Release -f netcoreapp2.0 src\DelegateDecompiler.EntityFrameworkCore.Tests
exit 0;

FROM microsoft/dotnet:2.0-runtime-nanoserver-1709 AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk-nanoserver-1709 AS build
WORKDIR /src
COPY DelegateDecompiler.sln ./
COPY src/DelegateDecompiler.EntityFrameworkCore.Tests/DelegateDecompiler.EntityFrameworkCore.Tests.csproj src/DelegateDecompiler.EntityFrameworkCore.Tests/
COPY src/DelegateDecompiler.Tests/DelegateDecompiler.Tests.csproj src/DelegateDecompiler.Tests/
COPY src/DelegateDecompiler/DelegateDecompiler.csproj src/DelegateDecompiler/
COPY src/DelegateDecompiler.EntityFrameworkCore/DelegateDecompiler.EntityFrameworkCore.csproj src/DelegateDecompiler.EntityFrameworkCore/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/src/DelegateDecompiler.EntityFrameworkCore.Tests
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DelegateDecompiler.EntityFrameworkCore.Tests.dll"]

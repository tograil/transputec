#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CrisesControl/CrisesControl.Api.csproj", "CrisesControl/"]
COPY ["CrisesControl.Infrastructure/CrisesControl.Infrastructure.csproj", "CrisesControl.Infrastructure/"]
COPY ["CrisesControl.Core/CrisesControl.Core.csproj", "CrisesControl.Core/"]
COPY ["CrisesControl.SharedKernel/CrisesControl.SharedKernel.csproj", "CrisesControl.SharedKernel/"]
RUN dotnet restore "CrisesControl/CrisesControl.Api.csproj"
COPY . .
WORKDIR "/src/CrisesControl"
RUN dotnet build "CrisesControl.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CrisesControl.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CrisesControl.Api.dll"]
#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FuFuShop/FuFuShop.csproj", "FuFuShop/"]
COPY ["FuFuShop.WeChat/FuFuShop.WeChat.csproj", "FuFuShop.WeChat/"]
COPY ["FuFuShop.Common/FuFuShop.Common.csproj", "FuFuShop.Common/"]
COPY ["FuFuShop.Model/FuFuShop.Model.csproj", "FuFuShop.Model/"]
COPY ["FuFuShop.Tasks/FuFuShop.Tasks.csproj", "FuFuShop.Tasks/"]
COPY ["FuFuShop.Services/FuFuShop.Services.csproj", "FuFuShop.Services/"]
COPY ["FuFuShop.Repository/FuFuShop.Repository.csproj", "FuFuShop.Repository/"]
RUN dotnet restore "FuFuShop/FuFuShop.csproj"
COPY . .
WORKDIR "/src/FuFuShop"
RUN dotnet build "FuFuShop.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FuFuShop.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FuFuShop.dll"]
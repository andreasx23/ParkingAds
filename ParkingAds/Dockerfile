#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ParkingAds/ParkingAds.csproj", "ParkingAds/"]
RUN dotnet restore "ParkingAds/ParkingAds.csproj"
COPY . .
WORKDIR "/src/ParkingAds"
RUN dotnet build "ParkingAds.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ParkingAds.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ParkingAds.dll"]
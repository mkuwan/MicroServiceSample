#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
# ターミナルを使用する場合は以下で行います
# docker build -t mkuwan/platformservice .
# docker run -p 8080:80 -d mkuwan/platformservice
# docker ps
# docker stop <container id>
# docker start <container id>
# docker push mkuwan/platformservice
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PlatformService.csproj", "PlatformService/"]
RUN dotnet restore "PlatformService/PlatformService.csproj"
#COPY . .
COPY . PlatformService/
WORKDIR "/src/PlatformService"
RUN dotnet build "PlatformService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PlatformService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PlatformService.dll"]



#FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
#WORKDIR /app
#
#COPY *.csproj ./
#RUN dotnet restore
#
#COPY . ./
#RUN dotnet publish -c Release -o out
#
#FROM mcr.microsoft.com/dotnet/aspnet:6.0
#WORKDIR /app
#COPY --from=build /app/out .
#ENTRYPOINT ["dotnet", "PlatformService.dll"]
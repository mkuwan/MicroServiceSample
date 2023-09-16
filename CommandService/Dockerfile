#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
# ターミナルを使用する場合は以下で行います
# docker build -t mkuwan/commandservice .
# docker run -p 8080:80 -d mkuwan/commandservice
# docker ps
# docker stop <container id>
# docker start <container id>
# docker push mkuwan/commandservice
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CommandService.csproj", "CommandService/"]
RUN dotnet restore "CommandService/CommandService.csproj"
#COPY . .
COPY . CommandService/
WORKDIR "/src/CommandService"
RUN dotnet build "CommandService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CommandService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CommandService.dll"]
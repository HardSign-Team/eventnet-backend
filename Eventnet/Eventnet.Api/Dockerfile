FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Eventnet.Api/Eventnet.Api.csproj", "Eventnet.Api/"]
COPY ["Eventnet.DataAccess/Eventnet.DataAccess.csproj", "Eventnet.DataAccess/"]
COPY ["Eventnet.Domain/Eventnet.Domain.csproj", "Eventnet.Domain/"]
COPY ["Eventnet.Infrastructure/Eventnet.Infrastructure.csproj", "Eventnet.Infrastructure/"]
RUN dotnet restore "Eventnet.Api/Eventnet.Api.csproj"
COPY . .
WORKDIR "/src/Eventnet.Api"
RUN dotnet build "Eventnet.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Eventnet.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Eventnet.Api.dll"]

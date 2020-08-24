FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["MakeMagic/MakeMagic.csproj", "MakeMagic/"]
RUN dotnet restore "MakeMagic/MakeMagic.csproj"
COPY . .
WORKDIR "/src/MakeMagic"
RUN dotnet build "MakeMagic.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MakeMagic.csproj" -c Release -o /app/publish

FROM base AS final
ARG MAKE_MAGIC_API_KEY
WORKDIR /app
COPY --from=publish /app/publish .
RUN sed -i "s/<<make_magic_api_key>>/$MAKE_MAGIC_API_KEY/g" appsettings.json
ENTRYPOINT ["dotnet", "MakeMagic.dll"]
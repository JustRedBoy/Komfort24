FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
RUN apt-get update && apt-get install -y curl
RUN curl -sL https://deb.nodesource.com/setup_14.x | bash -
RUN apt-get install -y nodejs
COPY ["Web/Web.csproj", "Web/"]
COPY ["Models/Models.csproj", "Models/"]
COPY ["Tools/Tools.csproj", "Tools/"]
COPY ["SheetsEF/SheetsEF.csproj", "SheetsEF/"]
COPY ["GoogleLib/GoogleLib.csproj", "GoogleLib/"]
RUN dotnet restore "Web/Web.csproj"
COPY . .

WORKDIR "/src/Web/ClientApp"
RUN npm install

WORKDIR "/src/Web"
RUN dotnet build "Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.dll"]
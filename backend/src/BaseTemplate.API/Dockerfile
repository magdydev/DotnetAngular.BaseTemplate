FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Directory.Build.props", "."]
COPY ["src/BaseTemplate.Domain/BaseTemplate.Domain.csproj", "src/BaseTemplate.Domain/"]
COPY ["src/BaseTemplate.Application/BaseTemplate.Application.csproj", "src/BaseTemplate.Application/"]
COPY ["src/BaseTemplate.Infrastructure/BaseTemplate.Infrastructure.csproj", "src/BaseTemplate.Infrastructure/"]
COPY ["src/BaseTemplate.API/BaseTemplate.API.csproj", "src/BaseTemplate.API/"]
RUN dotnet restore "src/BaseTemplate.API/BaseTemplate.API.csproj"

COPY . .
WORKDIR /src/src/BaseTemplate.API
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "BaseTemplate.API.dll"]

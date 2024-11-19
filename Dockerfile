# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copiar los archivos del proyecto
COPY *.csproj .
RUN dotnet restore

# Copiar todo y compilar
COPY . .
RUN dotnet publish -c Release -o out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out .

# Exponer el puerto en el contenedor
EXPOSE 80
EXPOSE 443

# Ejecutar la aplicación
ENTRYPOINT ["dotnet", "TuProyecto.dll"]

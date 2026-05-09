#############################################
##ETAPA 1: El Taller de Construcción (SDK)
#############################################

# 1. Traemos la imagen pesada que tiene todas las herramientas de compilación
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# 2. Definimos nuestra carpeta de trabajo interna
WORKDIR /src

# 3. COPIAR CADA .CSPROJ INDIVIDUALMENTE
# Esto es necesario para que 'dotnet restore' funcione.
COPY ["src/ApiSolutionTestVentas.Api/ApiSolutionTestVentas.Api.csproj", "ApiSolutionTestVentas.Api/"]
COPY ["src/ApiSolutionTestVentas.Dto/ApiSolutionTestVentas.Dto.csproj", "ApiSolutionTestVentas.Dto/"]
COPY ["src/ApiSolutionTestVentas.Entities/ApiSolutionTestVentas.Entities.csproj", "ApiSolutionTestVentas.Entities/"]
COPY ["src/ApiSolutionTestVentas.Persistencia/ApiSolutionTestVentas.Persistencia.csproj", "ApiSolutionTestVentas.Persistencia/"]
COPY ["src/ApiSolutionTestVentas.Repositories/ApiSolutionTestVentas.Repositories.csproj", "ApiSolutionTestVentas.Repositories/"]
COPY ["src/ApiSolutionTestVentas.Services/ApiSolutionTestVentas.Services.csproj", "ApiSolutionTestVentas.Services/"]

# 4. Restauramos dependenicas descargando las librerías de NuGet
RUN dotnet restore "ApiSolutionTestVentas.Api/ApiSolutionTestVentas.Api.csproj"

# 5. Copiamos el resto del código y compilamos
# Ahora Docker ya tiene los archivos .cs de todas las capas.
COPY ./src .

# 6. COMPILAR Y PUBLICAR EL PROYECTO
# Nos movemos a la carpeta del proyecto principal y lo compilamos
WORKDIR "/src/ApiSolutionTestVentas.Api"
RUN dotnet publish "ApiSolutionTestVentas.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

#############################################
##ETAPA 2: El Paquete Ligero (Runtime)
#############################################

# 7. Cambiamos a una imagen mucho más pequeña (solo el motor para correr)
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# 8. Traemos solo el resultado final de la etapa anterior
COPY --from=build /app/publish .

# 9. Definimos cómo arranca el contenedor
ENTRYPOINT ["dotnet", "ApiSolutionTestVentas.Api.dll"]



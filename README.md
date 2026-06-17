# README — Backend Solution Test Ventas

## 📌 Descripción General

Solution Test Ventas es una solución backend desarrollada con .NET 10, siguiendo principios de arquitectura limpia y buenas prácticas de desarrollo de APIs REST empresariales.
El proyecto implementa un sistema de gestión de empleados, departamentos, puestos de trabajo y categorías, incluyendo:
CRUDs completos

- Autenticación y autorización basada en roles
- Paginación
- Soft Delete
- Validaciones de negocio
- Infraestructura como código (IaC) con Terraform
- Estrategia GitFlow para control de versiones
- Preparación para despliegues CI/CD y ambientes separados
- La solución está orientada a escenarios reales empresariales y preparada para escalar en entornos cloud.


## 🚀 Tecnologías Utilizadas

Backend
-   .NET 10
-   ASP.NET Core Web API
-   Entity Framework Core
-   SQL Server
-   JWT Authentication
-   Swagger / OpenAPI
-   LINQ
-   Clean Architecture
-   Repository Pattern
-   Dependency Injection

Infraestructura / DevOps
-   Terraform
-   Azure SQL Server
-   Azure SQL Database
-   Azure Storage Account
-   GitFlow
-   GitHub
-   IaC Modular

## 🏗️ Arquitectura del Proyecto

La solución sigue una arquitectura desacoplada basada en capas:
```
API Layer
│
├── Controllers
├── DTOs
├── Middlewares
├── Filters
│
Application Layer
│
├── Services
├── Interfaces
├── Validators
│
Domain Layer
│
├── Entities
├── Enums
├── Business Rules
│
Infrastructure Layer
│
├── EF Core
├── Repositories
├── Persistence
├── Authentication
└── External Services
```

## 📂 Funcionalidades Implementadas

👥 Empleados 

- Obtener empleado por ID 
- Obtener empleado por Email
- Obtener empleado por Número de Documento 
- Búsquedas paginadas 
- Búsqueda por múltiples parámetros 
- Registro de empleados  
- Actualización de empleados
- Soft Delete 

🏢 Departamentos

- CRUD completo 
- Búsquedas por nombre 
- Búsquedas por ID
- Soft Delete 
- Seguridad basada en roles

💼 Puestos de Trabajo

- CRUD completo 
- Búsquedas por nombre 
- Búsquedas por ID 
- Soft Delete 
- Seguridad basada en roles

🗂️ Categorías

- CRUD de categorías 
- Soft Delete
- Validaciones de existencia 
- Manejo de respuestas estándar
- Seguridad basada en roles

🔐 Seguridad
La API implementa autenticación y autorización mediante JWT:
- Usuarios autenticados 
- Roles:
  - Administrator 
  - Customer 
  - Los endpoints sensibles requieren permisos específicos. 
  - Manejos de errores por permisos:
    - 401 Unauthorized 
    - 403 Forbidden

## 📑 Estándar de Respuesta

La API utiliza respuestas homogéneas:
```
{
  "data": {},
  "success": true,
  "message": "Operación exitosa",
  "errorMessage": null
}
```

Esto facilita:
- Integración frontend 
- Trazabilidad 
- Manejo uniforme de errores

## 🔎 Funcionalidades Técnicas Destacadas

✅ Soft Delete : Las eliminaciones son lógicas

```
status = false
```
Los registros permanecen en base de datos para auditoría e historial.

✅ Paginación : Implementación de paginación para búsquedas:

```
GET /api/Empleados?nombre=juan&Page=1&PageSize=50
```

✅ Validaciones de Negocio

- Validación de existencia de:
  - Puesto 
  - Departamento 
Antes de registrar empleados.

- Registro de ventas:
  - Se valida primero que el cliente tenga una cuenta.
  - Se valida que la cabecera del registro se haya registrad en la BD.
  - Se valida si los productos solicitados existen en la base de datos y hay sotck.
  - Se valida que el detalle se haya registrado en la BD con la misma cantidad de elementos solicitados.
  - Se reduce el stock de los productos adquiridos en la venta.

## 🌿 Estrategia GitFlow - Modelo de ramas (git branching)

**main**
- Código estable en producción 
- Un workflow de CI/CD para producción proviene de un merge/push en esta rama.
- Versionado

**develop**
- Rama de integración para desarrollo
- Un workflow de CI/CD para dev proviene de un merge/push en esta rama.
- Pull Request a `qa` cuando están listos los cambios.

**qa**
- Rama de integración para qa
- Un workflow de CI/CD para qa proviene de un merge/push en esta rama.
- Pull Request a `main` cuando están listos los cambios.
  
**feature/nombre**
- Nuevas funcionalidades 
- Nacen desde develop (git pull origin develop)
- Pull Request a `develop` cuando están listos los cambios.
  
**release/version**
- Cuando `develop` está listo para release.
- Preparación de releases 
- QA final 
- Correcciones menores

**hotfix/nombre**
- Correcciones urgentes en producción 
- Nacen desde main 
- Merge hacia:
    - main 
    - develop

## 📘 Swagger

**En Local**
```
https://localhost:8080/swagger/index.html

```

**Desplegado a Nube**
```
https://endpoint_server:8080/swagger/index.html

```

## ⚙️ Configuración y ejecución en Local (sin docker)

**1. Clonar repositorio**
```
git clone <repo-url>
```

**2. Configurar appsettings**
Esto solo para local, no para contenerizar.

```
"ConnectionStrings": {
"DefaultConnection": "Server=...;"
}
```

**3. Compilar la solución**
Ubicarse en la carpeta /src/ApiSolutionTestVentas.Api y ejecuta el siguiente comando

```
dotnet build

```

**4. Ejecutar migraciones**
Ubicarse en la carpeta /src/ApiSolutionTestVentas.Persistencia y ejecuta el siguiente comando

```
dotnet ef database update --startup-project /src/ApiSolutionTestVentas.Api
```

**5. Ejecutar la aplicación**
Ubìcarse en la raíz del proyecto (donde se encuentra el archivo docker-compose.yaml) ejecutar:
```
dotnet run
```

## 🐳 **Configuración y ejecución con Docker**

**1. Clonar repositorio**
```
git clone <repo-url>
```

**2. Crear y configurar archivo .env**

Seguir ejemplo de archivo .env.example. Asignar los valores a las variables

```
/api-backend-solution-test-ventas/.env

# --- Variables para Docker
SQL_USER=USUARIO_ADMIN_BD
SQL_PASSWORD=PASSWORD_BD
DB_NAME=NOMBRE_BD
STORAGE_KEY=CADENA_CNX_AZURE_STORAGE_ACCOUNT
JWT_SECRET=LLAVE_SECRETA_JWT
ENDPOINT_SERVER_SQL_AZURE=ENDPOINT_SQL_AZURE
}
```

**3. Compilar la solución**

Ubicarse en la carpeta /src/ApiSolutionTestVentas.Api y ejecuta el siguiente comando
```
dotnet build

```

**4. Ejecutar docker**

En la carpeta raiz del proyecto (donde se encuentra el archivo docker-compose.yaml) ejecutar:

```
docker compose up -d --build
}
```

**5. Validar endpoint - Swagger**
```
https://localhost:8080/swagger/index.html

```


# BackendDASALUD

Descripción

Proyecto backend para la aplicación DASALUD. Proporciona una API REST construida con .NET 8. La solución está separada en capas:
- `Domain`: modelos de dominio.
- `Application`: casos de uso y DTOs.
- `Infrastructure`: persistencia y servicios.
- `Web.API`: proyecto de la API y configuración de arranque.

Requisitos

- .NET 8 SDK (https://dotnet.microsoft.com/)
- SQL Server / SQL Server Express / LocalDB
- (Opcional) `dotnet-ef` para ejecutar migraciones

Clonar el repositorio

```bash
git clone https://github.com/MiltonMV2025/BackendDASALUD.git
cd BackendDASALUD
```

Configuración

1. Actualizar `Web.API/appsettings.json` con la cadena de conexión de su base de datos y la configuración JWT. Ejemplo:

```json
{
  "ConnectionStrings": {
    "Default": "Server=TU_SERVIDOR;Database=dasalud-core-db;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "SuperSecretKeyForDevelopmentOnlyChangeMe1234!",
    "Issuer": "dasalud.local",
    "Audience": "dasalud.local",
    "ExpiresMinutes": 60
  }
}
```

2. Para mayor seguridad, use variables de entorno en lugar de dejar secretos en `appsettings.json`. Por ejemplo `ConnectionStrings__Default`, `Jwt__Key`, `Jwt__Issuer`, etc.

Compilar y ejecutar

1. Restaurar paquetes y compilar:

```bash
dotnet restore
dotnet build
```

2. (Opcional) Instalar herramienta EF Core si necesita ejecutar migraciones:

```bash
dotnet tool install --global dotnet-ef
dotnet add Infrastructure package Microsoft.EntityFrameworkCore.Design
```

3. Ejecutar migraciones (si aplica):

```bash
dotnet ef database update --project Infrastructure --startup-project Web.API
```

4. Ejecutar la API:

```bash
dotnet run --project Web.API
```

La API quedará disponible en `https://localhost:{puerto}` o `http://localhost:{puerto}`; el puerto se muestra al iniciar la aplicación.

Ejecutar desde Visual Studio

- Abrir la solución en Visual Studio con soporte para .NET 8.
- Establecer `Web.API` como proyecto de inicio y ejecutar (F5 o Ctrl+F5).
- Asegurarse de actualizar `appsettings.json` o las variables de entorno antes de arrancar.

Variables de entorno útiles

- `ASPNETCORE_ENVIRONMENT=Development`
- `ConnectionStrings__Default`
- `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience`, `Jwt__ExpiresMinutes`

Notas

- Cambiar cualquier secreto y claves antes de desplegar a producción.
- Revisar la carpeta `Infrastructure` para detalles de persistencia y migraciones.

Contribuir

- Abrir issues para reportar errores o proponer mejoras.
- Crear ramas con nombres descriptivos y enviar Pull Requests.

Licencia

- Revisar el repositorio para información sobre la licencia.
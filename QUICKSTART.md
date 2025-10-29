# Inicio Rápido - Dataverse Metadata MCP Server

## 🚀 Para Usuarios

### Instalación en 3 Pasos

1. **Asegúrate de tener .NET 8.0 SDK instalado**:
   ```bash
   dotnet --version
   # Debe mostrar 8.0.x o superior
   ```

2. **Crea o edita tu archivo de configuración MCP**:
   - Windows: `%APPDATA%\Code\User\globalStorage\github.copilot-chat\mcp.json`
   - macOS/Linux: `~/.config/Code/User/globalStorage/github.copilot-chat/mcp.json`

3. **Agrega esta configuración** (reemplaza los valores con tus datos):
   ```json
   {
     "mcpServers": {
       "dataverse-metadata": {
         "type": "stdio",
         "command": "dnx",
         "args": ["DataverseMetadataMcp@1.0.0", "--yes"],
         "env": {
           "DATAVERSE_MCP_Dataverse__CurrentEnvironment": "production",
           "DATAVERSE_MCP_Dataverse__Environments__production__DisplayName": "Mi Entorno",
           "DATAVERSE_MCP_Dataverse__Environments__production__OrganizationUrl": "https://tuorg.crm.dynamics.com",
           "DATAVERSE_MCP_Dataverse__Environments__production__Authentication__AuthType": "Interactive",
           "DATAVERSE_MCP_Dataverse__Environments__production__Authentication__TenantId": "tu-tenant-id",
           "DATAVERSE_MCP_Dataverse__Environments__production__Authentication__ClientId": "tu-client-id"
         }
       }
     }
   }
   ```

4. **Reinicia VS Code** y ¡listo! El servidor estará disponible en GitHub Copilot.

### Primeros Pasos con Copilot

Prueba estos comandos en GitHub Copilot:

```
@workspace ¿Qué entidades tengo en Dataverse?
@workspace Muéstrame los campos de la tabla account
@workspace Dame detalles de la entidad contact
```

---

## 👨‍💻 Para Desarrolladores

### Desarrollo Local

1. **Clonar el repositorio**:
   ```bash
   git clone https://github.com/sergiocabellopacha/Davaverse_Metadata_MCP.git
   cd Davaverse_Metadata_MCP
   ```

2. **Copiar configuración de ejemplo**:
   ```bash
   cp .vscode/mcp.json.example .vscode/mcp.json
   ```

3. **Editar `.vscode/mcp.json`** con tus credenciales de Dataverse

4. **Compilar y ejecutar**:
   ```bash
   dotnet build src/DataverseMetadataMcp/DataverseMetadataMcp.csproj
   dotnet run --project src/DataverseMetadataMcp/DataverseMetadataMcp.csproj
   ```

### Probar con Copilot en Desarrollo

Tu configuración `.vscode/mcp.json` local debe usar:
```json
{
  "mcpServers": {
    "dataverse-metadata": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "src/DataverseMetadataMcp/DataverseMetadataMcp.csproj"
      ],
      "env": { ... }
    }
  }
}
```

---

## 📦 Para Publicar en NuGet

```bash
# 1. Actualizar versión en .csproj y .mcp/server.json

# 2. Compilar
dotnet build -c Release src/DataverseMetadataMcp/DataverseMetadataMcp.csproj

# 3. Empaquetar
dotnet pack -c Release src/DataverseMetadataMcp/DataverseMetadataMcp.csproj

# 4. Publicar
dotnet nuget push src/DataverseMetadataMcp/bin/Release/*.nupkg \
  --api-key TU_API_KEY \
  --source https://api.nuget.org/v3/index.json
```

Ver `docs/PUBLISHING.md` para más detalles.

---

## 🔑 Configurar Azure AD App

1. Ve a [Azure Portal](https://portal.azure.com) → Azure Active Directory → App registrations
2. Crear nueva aplicación:
   - Name: `Dataverse MCP Server`
   - Redirect URI: `http://localhost`
3. Copiar **Application (client) ID** y **Directory (tenant) ID**
4. Para Service Principal: Crear Client Secret en "Certificates & secrets"
5. Configurar permisos: API permissions → Dynamics CRM → user_impersonation

---

## 📚 Documentación Completa

- **README.md**: Documentación completa del proyecto
- **CONTRIBUTING.md**: Guía para contribuir
- **docs/PUBLISHING.md**: Guía de publicación en NuGet
- **CHANGELOG.md**: Resumen de todos los cambios

---

## 🆘 Problemas Comunes

### "No se encontró el comando dnx"
➡️ Instala .NET 8.0 SDK: https://dotnet.microsoft.com/download/dotnet/8.0

### El servidor no aparece en Copilot
➡️ Verifica el archivo `mcp.json` y reinicia VS Code

### Error de autenticación
➡️ Verifica TenantId y ClientId en la configuración

---

## 🎯 Herramientas Disponibles

| Herramienta | Descripción |
|------------|-------------|
| `GetEnvironmentInfo` | Info del entorno actual |
| `ListEnvironments` | Listar entornos configurados |
| `SetEnvironment` | Cambiar de entorno |
| `ListEntities` | Listar todas las entidades |
| `GetEntityDetails` | Detalles de una entidad |
| `ListEntityAttributes` | Listar campos de una entidad |

---

¿Necesitas ayuda? Abre un issue en GitHub o consulta la documentación completa en el README.md

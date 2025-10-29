# Dataverse Metadata MCP Server

Un servidor MCP (Model Context Protocol) para consultar metadatos de Microsoft Dataverse, incluyendo entidades, atributos, relaciones y conjuntos de opciones a través de múltiples entornos.

## 🚀 Características

- **Consulta de Metadatos**: Acceso completo a metadatos de entidades, atributos y relaciones
- **Múltiples Entornos**: Soporte para múltiples entornos de Dataverse (desarrollo, producción, sandbox, etc.)
- **Autenticación Flexible**: Soporta autenticación interactiva y service principal
- **Integración con GitHub Copilot**: Funciona perfectamente con GitHub Copilot en VS Code
- **Fácil Instalación**: Disponible como paquete NuGet, no requiere clonar el repositorio

## 📋 Requisitos Previos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) o superior
- [Visual Studio Code](https://code.visualstudio.com/)
- [Extensión de GitHub Copilot](https://marketplace.visualstudio.com/items?itemName=GitHub.copilot) para VS Code
- Acceso a un entorno de Microsoft Dataverse
- Azure AD App Registration para autenticación

## 🔧 Instalación

### Opción 1: Instalación vía NuGet (Próximamente)

> ⚠️ **Nota**: El paquete aún no está publicado en NuGet. Por ahora, usa la **Opción 2: Desarrollo Local**.

Una vez publicado, podrás instalarlo así:

1. **Instalar .NET SDK** (si aún no lo tienes):
   ```bash
   # Verificar instalación
   dotnet --version
   ```

2. **Instalar la herramienta globalmente**:
   ```bash
   dotnet tool install -g DataverseMetadataMcp
   ```
   
   **Nota**: Si ya la tienes instalada y quieres actualizar a la última versión:
   ```bash
   dotnet tool update -g DataverseMetadataMcp
   ```

3. **Configurar en VS Code**:
   
   Crea o edita el archivo de configuración MCP en VS Code:
   - **Windows**: `%APPDATA%\Roaming\Code\User\mcp.json`

   O crea un archivo `.vscode/mcp.json` en la raíz de tu workspace.

4. **Agregar configuración del servidor**:

   ```json
   {
     "mcpServers": {
       "dataverse-metadata": {
         "type": "stdio",
         "command": "dataverse-mcp-server",
         "args": [],
         "env": {
           "DATAVERSE_MCP_Dataverse__CurrentEnvironment": "production",
           "DATAVERSE_MCP_Dataverse__Environments__production__DisplayName": "Mi Entorno de Producción",
           "DATAVERSE_MCP_Dataverse__Environments__production__OrganizationUrl": "https://tuorg.crm.dynamics.com",
           "DATAVERSE_MCP_Dataverse__Environments__production__Authentication__AuthType": "Interactive"
         }
       }
     }
   }
   ```

   **Nota**: Para autenticación interactiva, solo necesitas el `AuthType`. El `ClientId` y `TenantId` son opcionales (se usarán valores por defecto si no se especifican).

5. **Reinicia VS Code** y ¡listo! El servidor estará disponible en GitHub Copilot.

### Opción 2: Desarrollo Local (Recomendado actualmente)

1. **Clonar el repositorio**:
   ```bash
   git clone https://github.com/sergiocabellopacha/Davaverse_Metadata_MCP.git
   cd Davaverse_Metadata_MCP
   ```

2. **Restaurar dependencias**:
   ```bash
   dotnet restore src/DataverseMetadataMcp/DataverseMetadataMcp.csproj
   ```

3. **Configurar entorno local**:
   
   Crea un archivo `.vscode/mcp.json` en la raíz del proyecto con esta configuración:
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
         "env": {
           "DATAVERSE_MCP_Dataverse__CurrentEnvironment": "development",
           "DATAVERSE_MCP_Dataverse__Environments__development__DisplayName": "Desarrollo",
           "DATAVERSE_MCP_Dataverse__Environments__development__OrganizationUrl": "https://tuorg-dev.crm.dynamics.com",
           "DATAVERSE_MCP_Dataverse__Environments__development__Authentication__AuthType": "Interactive"
         }
       }
     }
   }
   ```
    **Nota**: es posible que ese path al proyecto no sirva y tengas que ponerlo completo a donde lo hayas clonado.
   **Nota**: Reemplaza `https://tuorg-dev.crm.dynamics.com` con la URL de tu organización de Dataverse.

4. **Reinicia VS Code** y el servidor MCP estará disponible en GitHub Copilot.

## ⚙️ Configuración

### Variables de Entorno

El servidor se configura mediante variables de entorno con el prefijo `DATAVERSE_MCP_`. Puedes configurar múltiples entornos:

#### Configuración General
- `DATAVERSE_MCP_Dataverse__CurrentEnvironment`: Nombre del entorno actual a utilizar

#### Configuración por Entorno
Para cada entorno (reemplaza `{env}` con el nombre del entorno, ej: `development`, `production`, `sandbox`):

- `DATAVERSE_MCP_Dataverse__Environments__{env}__DisplayName`: Nombre descriptivo del entorno
- `DATAVERSE_MCP_Dataverse__Environments__{env}__OrganizationUrl`: URL de la organización Dataverse
- `DATAVERSE_MCP_Dataverse__Environments__{env}__Authentication__AuthType`: Tipo de autenticación (`Interactive` o `ServicePrincipal`)
- `DATAVERSE_MCP_Dataverse__Environments__{env}__Authentication__TenantId`: ID del tenant de Azure AD (opcional para `Interactive`)
- `DATAVERSE_MCP_Dataverse__Environments__{env}__Authentication__ClientId`: ID de la aplicación Azure AD (opcional para `Interactive`)
- `DATAVERSE_MCP_Dataverse__Environments__{env}__Authentication__ClientSecret`: Secreto del cliente (requerido solo para `ServicePrincipal`)

### Ejemplo con Múltiples Entornos

```json
{
  "mcpServers": {
    "dataverse-metadata": {
      "type": "stdio",
      "command": "dataverse-mcp-server",
      "args": [],
      "env": {
        "DATAVERSE_MCP_Dataverse__CurrentEnvironment": "development",
        
        "DATAVERSE_MCP_Dataverse__Environments__development__DisplayName": "Desarrollo",
        "DATAVERSE_MCP_Dataverse__Environments__development__OrganizationUrl": "https://dev.crm.dynamics.com",
        "DATAVERSE_MCP_Dataverse__Environments__development__Authentication__AuthType": "Interactive",
        
        "DATAVERSE_MCP_Dataverse__Environments__production__DisplayName": "Producción",
        "DATAVERSE_MCP_Dataverse__Environments__production__OrganizationUrl": "https://prod.crm.dynamics.com",
        "DATAVERSE_MCP_Dataverse__Environments__production__Authentication__AuthType": "ServicePrincipal",
        "DATAVERSE_MCP_Dataverse__Environments__production__Authentication__ClientId": "client-id",
        "DATAVERSE_MCP_Dataverse__Environments__production__Authentication__ClientSecret": "secret"
      }
    }
  }
}
```

**Notas importantes**:
- Para **autenticación Interactive**: Solo necesitas `AuthType`. `ClientId` y `TenantId` son opcionales.
- Para **autenticación ServicePrincipal**: Debes proporcionar `ClientId` y `ClientSecret` (ambos requeridos).

## 🔑 Configuración de Azure AD

**Para autenticación interactiva**, no necesitas configurar una Azure AD App Registration. El servidor utilizará la aplicación por defecto de Microsoft.

**Para autenticación Service Principal**, sigue estos pasos:

1. Ve a [Azure Portal](https://portal.azure.com)
2. Navega a **Azure Active Directory** > **App registrations** > **New registration**
3. Configura la aplicación:
   - **Name**: Dataverse MCP Server
   - **Supported account types**: Single tenant
4. Copia el **Application (client) ID** y **Directory (tenant) ID**
5. Ve a **Certificates & secrets** > **New client secret**
   - Copia el secreto generado
6. Configura permisos API:
   - **API permissions** > **Add a permission** > **Dynamics CRM**
   - Agrega **user_impersonation** permission
7. Asegúrate de que el Service Principal tenga acceso al entorno de Dataverse

## 🛠️ Herramientas Disponibles

El servidor expone las siguientes herramientas para GitHub Copilot:

### `GetEnvironmentInfo`
Obtiene información sobre el entorno Dataverse actual, incluyendo estado de conexión y versión.

**Ejemplo de uso en Copilot**:
```
@workspace ¿Cuál es el estado de mi entorno de Dataverse?
```

### `ListEnvironments`
Lista todos los entornos de Dataverse configurados en el servidor.

**Ejemplo de uso**:
```
@workspace Muéstrame los entornos disponibles
```

### `SetEnvironment`
Cambia a un entorno de Dataverse diferente.

**Parámetros**:
- `environmentName`: Nombre del entorno (ej: development, production, sandbox)

**Ejemplo de uso**:
```
@workspace Cambia al entorno de producción
```

### `ListEntities`
Lista todas las entidades (tablas) en el entorno actual.

**Parámetros**:
- `customOnly` (opcional): Si es true, solo devuelve entidades personalizadas

**Ejemplo de uso**:
```
@workspace Lista todas las entidades personalizadas
@workspace Muéstrame todas las tablas en Dataverse
```

### `GetEntityDetails`
Obtiene metadatos detallados de una entidad específica.

**Parámetros**:
- `entityLogicalName`: Nombre lógico de la entidad (ej: account, contact)

**Ejemplo de uso**:
```
@workspace Dame los detalles de la entidad account
@workspace ¿Qué metadatos tiene la tabla contact?
```

### `ListEntityAttributes`
Lista todos los atributos (columnas/campos) de una entidad específica.

**Parámetros**:
- `entityLogicalName`: Nombre lógico de la entidad
- `customOnly` (opcional): Si es true, solo devuelve atributos personalizados

**Ejemplo de uso**:
```
@workspace Lista los campos de la entidad account
@workspace Muéstrame los atributos personalizados de contact
```

## 📚 Ejemplos de Uso

### Explorar Metadatos

```
Usuario: @workspace ¿Qué entidades tengo disponibles?
Copilot: [Usa ListEntities para mostrar todas las entidades]

Usuario: @workspace Dame detalles sobre la entidad account
Copilot: [Usa GetEntityDetails con entityLogicalName="account"]

Usuario: @workspace ¿Qué campos tiene la tabla de contactos?
Copilot: [Usa ListEntityAttributes con entityLogicalName="contact"]
```

### Cambiar entre Entornos

```
Usuario: @workspace Cambia a mi entorno de producción
Copilot: [Usa SetEnvironment con environmentName="production"]

Usuario: @workspace ¿En qué entorno estoy?
Copilot: [Usa GetEnvironmentInfo]
```

## 🐛 Solución de Problemas

### Error: "No se encontró el comando dataverse-mcp-server"
Este error significa que la herramienta no está instalada globalmente. Soluciones:

1. **Instala la herramienta**:
   ```bash
   dotnet tool install -g DataverseMetadataMcp
   ```

2. **Verifica que esté instalada**:
   ```bash
   dotnet tool list -g
   # Debe aparecer "dataversemetadatamcp" en la lista
   ```

3. **Si ya está instalada, actualízala**:
   ```bash
   dotnet tool update -g DataverseMetadataMcp
   ```

4. **Verifica la ruta de las herramientas globales**:
   - Las herramientas se instalan en `%USERPROFILE%\.dotnet\tools` (Windows)
   - Asegúrate de que esta ruta esté en tu variable de entorno PATH

### Error de autenticación Interactive
- Si no especificas `ClientId`, se usa la aplicación por defecto de Microsoft
- Asegúrate de tener permisos para acceder al entorno de Dataverse
- Si aparece un error de autenticación, intenta especificar tu propio `ClientId` y `TenantId`

### Error de autenticación ServicePrincipal
- Verifica que el `ClientId` y `ClientSecret` sean correctos
- Asegúrate de que el Service Principal tenga acceso al entorno de Dataverse
- Verifica que la aplicación tenga permisos para Dynamics CRM en Azure AD

### El servidor no aparece en Copilot
1. Verifica que el archivo `mcp.json` esté en la ubicación correcta
2. Reinicia VS Code
3. Comprueba los logs en la consola de desarrollador de VS Code (Help > Toggle Developer Tools)

### Error de conexión a Dataverse
- Verifica que la OrganizationUrl sea correcta
- Asegúrate de tener acceso al entorno
- Prueba la conexión manualmente primero

## 📦 Publicación (Para Mantenedores)

Para publicar una nueva versión en NuGet:

```bash
# 1. Actualizar versión en .csproj y .mcp/server.json

# 2. Compilar en modo Release
dotnet build -c Release src/DataverseMetadataMcp/DataverseMetadataMcp.csproj

# 3. Empaquetar
dotnet pack -c Release src/DataverseMetadataMcp/DataverseMetadataMcp.csproj

# 4. Publicar a NuGet
dotnet nuget push src/DataverseMetadataMcp/bin/Release/*.nupkg --api-key <your-api-key> --source https://api.nuget.org/v3/index.json
```

## 🤝 Contribuir

Las contribuciones son bienvenidas! Por favor:

1. Haz fork del repositorio
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto está licenciado bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para más detalles.

## 👤 Autor

**Sergio Cabello Pacha**

- GitHub: [@sergiocabellopacha](https://github.com/sergiocabellopacha)

## 🙏 Agradecimientos

- [Model Context Protocol](https://modelcontextprotocol.io/) por el protocolo
- [Microsoft Dataverse](https://docs.microsoft.com/en-us/power-apps/developer/data-platform/) por la plataforma
- Comunidad de .NET y Power Platform

---

**Nota**: Este es un proyecto independiente y no está afiliado oficialmente con Microsoft.

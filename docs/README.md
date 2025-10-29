# Dataverse Metadata MCP Server

Un servidor MCP (Model Context Protocol) especializado para consultar metadatos de Microsoft Dataverse. Este servidor permite a los LLMs obtener información detallada sobre entidades, campos, relaciones y configuraciones de Dataverse sin necesidad de conocer los nombres internos o estructuras específicas.

## 🚀 Características

### 🔧 **Herramientas MCP Disponibles**

| Herramienta | Descripción |
|-------------|-------------|
| `get-environment-info` | Obtiene información del entorno actual y estado de conexión |
| `list-environments` | Lista todos los entornos configurados disponibles |
| `set-environment` | Cambia al entorno especificado |
| `list-entities` | Lista todas las entidades del entorno actual |
| `get-entity-details` | Obtiene detalles completos de una entidad específica |
| `list-entity-attributes` | Lista todos los campos de una entidad |
| `get-attribute-details` | Obtiene detalles completos de un campo específico |
| `list-entity-relationships` | Lista todas las relaciones de una entidad |
| `search-entities` | Busca entidades por nombre o descripción |
| `get-global-optionset-values` | Obtiene valores de conjuntos de opciones globales |
| `get-entity-summary` | Obtiene un resumen completo de una entidad |

### 🔐 **Autenticación Soportada**

- **Service Principal**: Para entornos automatizados (CI/CD, producción)
- **Autenticación Interactiva**: Para desarrollo local

### 🌍 **Gestión Multi-Entorno**

- Configuración sencilla de múltiples entornos
- Cambio dinámico entre entornos sin reiniciar
- Soporte para desarrollo, testing, y producción

## 📋 Requisitos Previos

- .NET 8.0 o superior
- Acceso a un entorno de Microsoft Dataverse
- Azure AD App Registration con permisos apropiados

## ⚙️ Configuración

### 1. **Configurar Azure AD App Registration**

Para autenticación interactiva:
```json
{
  "AuthType": "Interactive",
  "TenantId": "tu-tenant-id",
  "ClientId": "tu-client-id",
  "RedirectUri": "http://localhost"
}
```

Para Service Principal:
```json
{
  "AuthType": "ServicePrincipal",
  "TenantId": "tu-tenant-id", 
  "ClientId": "tu-client-id",
  "ClientSecret": "tu-client-secret"
}
```

### 2. **Configurar Entornos**

Edita `config/server-config.json` o `appsettings.json`:

```json
{
  "Dataverse": {
    "CurrentEnvironment": "development",
    "Environments": {
      "development": {
        "DisplayName": "Entorno de Desarrollo",
        "OrganizationUrl": "https://tuorg-dev.crm.dynamics.com",
        "Authentication": {
          "AuthType": "Interactive",
          "TenantId": "tu-tenant-id",
          "ClientId": "tu-client-id"
        },
        "TimeoutSeconds": 30,
        "MaxRetryAttempts": 3
      },
      "production": {
        "DisplayName": "Entorno de Producción",
        "OrganizationUrl": "https://tuorg.crm.dynamics.com",
        "Authentication": {
          "AuthType": "ServicePrincipal",
          "TenantId": "tu-tenant-id",
          "ClientId": "tu-client-id",
          "ClientSecret": "tu-client-secret"
        },
        "TimeoutSeconds": 60,
        "MaxRetryAttempts": 5
      }
    }
  }
}
```

## 🚀 Instalación y Ejecución

### 1. **Clonar y Construir**

```bash
git clone <repository-url>
cd Davaverse_Metadata_MCP
cd src/DataverseMetadataMcp
dotnet restore
dotnet build
```

### 2. **Configurar el Proyecto**

1. Copia `config/server-config.json` y actualiza con tus datos
2. Configura tus Azure AD App Registrations
3. Actualiza las URLs de organización

### 3. **Ejecutar el Servidor**

```bash
dotnet run
```

### 4. **Configurar en tu Cliente MCP**

Añade a tu configuración de cliente MCP:

```json
{
  "mcpServers": {
    "dataverse-metadata": {
      "command": "dotnet",
      "args": ["run", "--project", "ruta/al/DataverseMetadataMcp.csproj"],
      "env": {
        "DATAVERSE_MCP_CurrentEnvironment": "development"
      }
    }
  }
}
```

## 📖 Ejemplos de Uso

### **Obtener información del entorno actual**
```json
{
  "tool": "get-environment-info"
}
```

### **Listar todas las entidades**
```json
{
  "tool": "list-entities",
  "parameters": {
    "customOnly": false
  }
}
```

### **Obtener detalles de una entidad**
```json
{
  "tool": "get-entity-details", 
  "parameters": {
    "entityLogicalName": "account"
  }
}
```

### **Listar campos de una entidad**
```json
{
  "tool": "list-entity-attributes",
  "parameters": {
    "entityLogicalName": "contact",
    "customOnly": true
  }
}
```

### **Obtener resumen completo de una entidad**
```json
{
  "tool": "get-entity-summary",
  "parameters": {
    "entityLogicalName": "opportunity",
    "includeCustomOnly": false
  }
}
```

### **Buscar entidades**
```json
{
  "tool": "search-entities",
  "parameters": {
    "searchTerm": "custom"
  }
}
```

### **Cambiar de entorno**
```json
{
  "tool": "set-environment",
  "parameters": {
    "environmentName": "production"  
  }
}
```

## 🔧 Configuración Avanzada

### **Variables de Entorno**

Puedes usar variables de entorno con el prefijo `DATAVERSE_MCP_`:

```bash
export DATAVERSE_MCP_CurrentEnvironment=production
export DATAVERSE_MCP_Environments__production__Authentication__ClientSecret=tu-secret
```

### **Logging**

Configura el nivel de logging en `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "DataverseMetadataMcp": "Debug"
    }
  }
}
```

### **Timeouts y Reintentos**

```json
{
  "TimeoutSeconds": 60,
  "MaxRetryAttempts": 5
}
```

## 🔍 Casos de Uso

### **Para Desarrolladores**
- Conocer nombres internos de campos
- Entender relaciones entre entidades
- Validar tipos de datos antes de programar
- Explorar estructura de entidades customizadas

### **Para Consultores/Analistas**
- Documentar configuraciones
- Analizar modelos de datos
- Comparar configuraciones entre entornos
- Exportar metadatos para documentación

### **Para LLMs/AI**
- Generar código Dataverse más preciso
- Proporcionar ayuda contextual sobre esquemas
- Automatizar documentación de proyectos
- Facilitar desarrollo asistido por IA

## 🧪 Testing

```bash
cd tests/DataverseMetadataMcp.Tests
dotnet test
```

## 🤝 Contribución

1. Fork el repositorio
2. Crea una rama para tu feature (`git checkout -b feature/nueva-funcionalidad`)
3. Commit tus cambios (`git commit -am 'Añadir nueva funcionalidad'`)
4. Push a la rama (`git push origin feature/nueva-funcionalidad`)
5. Crea un Pull Request

## 📝 Licencia

[Añadir información de licencia]

## 🐛 Troubleshooting

### **Error de Autenticación**
- Verifica que el Client ID y Tenant ID sean correctos
- Asegúrate de que la App Registration tenga permisos de Dataverse
- Para Service Principal, verifica que el Client Secret sea válido

### **Error de Conexión**
- Verifica que la URL de organización sea correcta
- Comprueba conectividad de red
- Revisa la configuración de firewall/proxy

### **Entidad no Encontrada**
- Verifica que el nombre lógico de la entidad sea correcto
- Asegúrate de tener permisos de lectura sobre la entidad
- Comprueba que la entidad exista en el entorno actual

## 📧 Soporte

Para reportar bugs o solicitar features, crea un issue en GitHub.

---

*Desarrollado con ❤️ para la comunidad de Dataverse*
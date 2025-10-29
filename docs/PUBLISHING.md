# Guía de Publicación en NuGet

Esta guía explica cómo publicar el servidor MCP de Dataverse Metadata en NuGet.

## Prerrequisitos

1. Cuenta en [NuGet.org](https://www.nuget.org/)
2. API Key de NuGet
3. .NET 8.0 SDK instalado

## Pasos para Publicar

### 1. Actualizar la Versión

Edita `src/DataverseMetadataMcp/DataverseMetadataMcp.csproj` y actualiza la versión:

```xml
<Version>1.0.1</Version>
```

También actualiza la versión en `src/DataverseMetadataMcp/.mcp/server.json`:

```json
{
  "version": "1.0.1",
  "packages": [
    {
      "version": "1.0.1",
      ...
    }
  ]
}
```

### 2. Compilar en Modo Release

```bash
cd src/DataverseMetadataMcp
dotnet build -c Release
```

### 3. Empaquetar

```bash
dotnet pack -c Release
```

Esto generará varios archivos `.nupkg` en `src/DataverseMetadataMcp/bin/Release/`:
- `DataverseMetadataMcp.1.0.0.nupkg` (paquete de herramienta)
- Paquetes específicos de plataforma para cada RuntimeIdentifier

### 4. Probar el Paquete Localmente (Opcional)

Antes de publicar, puedes probar el paquete localmente:

```bash
# Instalar globalmente desde el archivo local
dotnet tool install --global --add-source ./bin/Release DataverseMetadataMcp

# Probar
dataverse-mcp-server

# Desinstalar después de probar
dotnet tool uninstall --global DataverseMetadataMcp
```

### 5. Publicar en NuGet.org

```bash
# Publicar todos los paquetes
dotnet nuget push bin/Release/*.nupkg --api-key <tu-api-key> --source https://api.nuget.org/v3/index.json
```

**Nota**: Asegúrate de publicar TODOS los archivos `.nupkg` para que el paquete esté disponible en todas las plataformas compatibles.

### 6. Probar con NuGet.org (Entorno de Integración)

Si quieres probar antes de publicar en producción, puedes usar el entorno de integración:

```bash
# Registrarse en https://int.nugettest.org
dotnet nuget push bin/Release/*.nupkg --api-key <tu-api-key> --source https://apiint.nugettest.org/v3/index.json
```

### 7. Verificar la Publicación

1. Ve a [NuGet.org](https://www.nuget.org/)
2. Busca tu paquete: `DataverseMetadataMcp`
3. Verifica que la versión correcta esté disponible
4. Comprueba la pestaña "MCP Server" para ver la configuración generada

### 8. Actualizar el README

Actualiza el README con instrucciones para la nueva versión:

```json
{
  "mcpServers": {
    "dataverse-metadata": {
      "type": "stdio",
      "command": "dataverse-mcp-server",
      "args": [],
      ...
    }
  }
}
```

## Obtener una API Key de NuGet

1. Inicia sesión en [NuGet.org](https://www.nuget.org/)
2. Ve a tu perfil > API Keys
3. Clic en "Create"
4. Configura:
   - **Key Name**: Nombre descriptivo (ej: "DataverseMCP Deployment")
   - **Package Owner**: Tu usuario
   - **Select Scopes**: Push new packages and package versions
   - **Select Packages**: All packages o selecciona DataverseMetadataMcp
   - **Expiration**: Recomiendado 365 días
5. Copia la API Key (¡solo se muestra una vez!)

## Versionado Semántico

Seguimos el versionado semántico (SemVer): `MAJOR.MINOR.PATCH`

- **MAJOR**: Cambios incompatibles en la API
- **MINOR**: Nueva funcionalidad compatible con versiones anteriores
- **PATCH**: Correcciones de bugs compatibles con versiones anteriores

Ejemplos:
- `1.0.0` → Primera versión estable
- `1.0.1` → Corrección de bugs
- `1.1.0` → Nueva herramienta agregada
- `2.0.0` → Cambio en la estructura de configuración

## Notas de Versión

Al publicar una nueva versión, actualiza las Release Notes en GitHub:

1. Ve a GitHub > Releases
2. Clic en "Draft a new release"
3. Tag version: `v1.0.1`
4. Release title: `Version 1.0.1`
5. Describe los cambios:

```markdown
## ¿Qué hay de nuevo?
- Agregada herramienta para consultar relaciones
- Corrección de bug en autenticación con Service Principal

## Cambios
- Mejoras de rendimiento en consultas de metadatos
- Actualización de dependencias

## Correcciones
- Solucionado error al listar entidades en entornos grandes
```

## Lista de Verificación Pre-Publicación

- [ ] Todas las pruebas pasan
- [ ] Versión actualizada en .csproj
- [ ] Versión actualizada en .mcp/server.json
- [ ] README.md actualizado
- [ ] CHANGELOG.md actualizado (si existe)
- [ ] Cambios commiteados y pusheados a GitHub
- [ ] Tag de versión creado en Git
- [ ] Paquete compilado en Release
- [ ] Paquete probado localmente
- [ ] API Key de NuGet válida

## Automatización (GitHub Actions)

Para automatizar el proceso, puedes crear un workflow de GitHub Actions:

```yaml
name: Publish to NuGet

on:
  release:
    types: [published]

jobs:
  publish:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: 8.0.x
    
    - name: Restore dependencies
      run: dotnet restore src/DataverseMetadataMcp/DataverseMetadataMcp.csproj
    
    - name: Build
      run: dotnet build -c Release src/DataverseMetadataMcp/DataverseMetadataMcp.csproj
    
    - name: Pack
      run: dotnet pack -c Release src/DataverseMetadataMcp/DataverseMetadataMcp.csproj
    
    - name: Publish to NuGet
      run: dotnet nuget push src/DataverseMetadataMcp/bin/Release/*.nupkg --api-key ${{ secrets.NUGET_API_KEY }} --source https://api.nuget.org/v3/index.json
```

Agrega tu API Key de NuGet como secret en GitHub:
1. Settings > Secrets and variables > Actions
2. New repository secret
3. Name: `NUGET_API_KEY`
4. Value: Tu API Key

## Soporte

Si tienes problemas publicando:
- Verifica que el PackageId sea único en NuGet.org
- Asegúrate de que no estés publicando una versión que ya existe
- Verifica que tu API Key tenga los permisos correctos
- Contacta con el mantenedor del proyecto

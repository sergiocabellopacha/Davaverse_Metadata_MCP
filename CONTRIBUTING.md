# Guía de Contribución

¡Gracias por tu interés en contribuir al Dataverse Metadata MCP Server!

## Cómo Contribuir

1. **Fork el repositorio**
2. **Crea una rama** para tu feature (`git checkout -b feature/AmazingFeature`)
3. **Haz commit** de tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. **Push** a la rama (`git push origin feature/AmazingFeature`)
5. **Abre un Pull Request**

## Desarrollo Local

### Prerrequisitos

- .NET 8.0 SDK o superior
- Visual Studio Code (recomendado)
- Acceso a un entorno Dataverse para pruebas

### Configuración

1. Clona el repositorio:
```bash
git clone https://github.com/sergiocabellopacha/Davaverse_Metadata_MCP.git
cd Davaverse_Metadata_MCP
```

2. Restaura las dependencias:
```bash
dotnet restore src/DataverseMetadataMcp/DataverseMetadataMcp.csproj
```

3. Copia el archivo de ejemplo de configuración:
```bash
cp .vscode/mcp.json.example .vscode/mcp.json
```

4. Edita `.vscode/mcp.json` con tus configuraciones de Dataverse

5. Compila el proyecto:
```bash
dotnet build src/DataverseMetadataMcp/DataverseMetadataMcp.csproj
```

### Ejecutar Pruebas

```bash
dotnet test tests/DataverseMetadataMcp.Tests/DataverseMetadataMcp.Tests.csproj
```

### Estructura del Proyecto

```
src/DataverseMetadataMcp/
├── Configuration/      # Clases de configuración
├── Models/            # Modelos de datos
├── Services/          # Servicios de negocio
├── Tools/             # Herramientas MCP expuestas
├── McpServer.cs       # Implementación del servidor MCP
└── Program.cs         # Punto de entrada

tests/
└── DataverseMetadataMcp.Tests/  # Pruebas unitarias
```

## Estándares de Código

- Usa nombres descriptivos para variables y métodos
- Agrega comentarios XML para métodos públicos
- Sigue las convenciones de C# estándar
- Asegúrate de que el código compile sin errores
- Las advertencias de nullable deben ser atendidas si es posible

## Agregar Nueva Funcionalidad

### Agregar una Nueva Herramienta MCP

1. Agrega el método en `Tools/DataverseMetadataToolsSimple.cs`:
```csharp
[Description("Descripción de la herramienta")]
public async Task<object> NombreHerramienta(
    [Description("Descripción del parámetro")]
    string parametro,
    CancellationToken cancellationToken = default)
{
    // Implementación
}
```

2. Registra la herramienta en `McpServer.cs` en el método `HandleToolsListAsync()`:
```csharp
new
{
    name = "nombre_herramienta",
    description = "Descripción de la herramienta",
    inputSchema = new
    {
        type = "object",
        properties = new
        {
            parametro = new
            {
                type = "string",
                description = "Descripción del parámetro"
            }
        },
        required = new[] { "parametro" }
    }
}
```

3. Agrega el manejo en `HandleToolCallAsync()`:
```csharp
"nombre_herramienta" => await _tools.NombreHerramienta(
    arguments.GetProperty("parametro").GetString()!),
```

## Reportar Bugs

Si encuentras un bug, por favor:

1. Verifica que no esté ya reportado en los Issues
2. Crea un nuevo Issue con:
   - Descripción clara del problema
   - Pasos para reproducir
   - Comportamiento esperado vs actual
   - Versión de .NET y sistema operativo
   - Logs relevantes (si aplica)

## Sugerir Mejoras

Las sugerencias de mejora son bienvenidas! Por favor:

1. Abre un Issue describiendo la mejora
2. Explica por qué sería útil
3. Proporciona ejemplos de uso si es posible

## Proceso de Pull Request

1. Asegúrate de que tu código compile sin errores
2. Actualiza la documentación si es necesario
3. Agrega pruebas para nueva funcionalidad
4. Actualiza el README.md si agregaste herramientas nuevas
5. El PR será revisado y se proporcionará feedback

## Licencia

Al contribuir, aceptas que tus contribuciones estarán bajo la misma licencia MIT del proyecto.

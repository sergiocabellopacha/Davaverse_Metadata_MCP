using DataverseMetadataMcp.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataverseMetadataMcp;

/// <summary>
/// Simple MCP Server implementation using stdio transport
/// </summary>
public class McpServer
{
    private readonly IServiceProvider _services;
    private readonly ILogger<McpServer> _logger;
    private readonly DataverseTools _tools;
    private readonly JsonSerializerOptions _jsonOptions;

    public McpServer(IServiceProvider services)
    {
        _services = services;
        _logger = services.GetRequiredService<ILogger<McpServer>>();
        _tools = services.GetRequiredService<DataverseTools>();
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public async Task RunAsync()
    {
        _logger.LogInformation("MCP Server ready, listening on stdio...");

        // Read from stdin and write to stdout
        using var reader = new StreamReader(Console.OpenStandardInput());
        using var writer = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };

        while (!reader.EndOfStream)
        {
            try
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                _logger.LogDebug("Received request: {Request}", line);

                var response = await ProcessRequestAsync(line);
                
                var responseJson = JsonSerializer.Serialize(response, _jsonOptions);
                await writer.WriteLineAsync(responseJson);
                
                _logger.LogDebug("Sent response: {Response}", responseJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing request");
                
                var errorResponse = new
                {
                    jsonrpc = "2.0",
                    error = new
                    {
                        code = -32603,
                        message = "Internal error",
                        data = ex.Message
                    },
                    id = (object?)null
                };
                
                await writer.WriteLineAsync(JsonSerializer.Serialize(errorResponse, _jsonOptions));
            }
        }
    }

    private async Task<object> ProcessRequestAsync(string requestJson)
    {
        var request = JsonSerializer.Deserialize<JsonElement>(requestJson);
        
        var method = request.GetProperty("method").GetString();
        var id = request.TryGetProperty("id", out var idProp) ? idProp : default;
        
        try
        {
            var result = method switch
            {
                "initialize" => await HandleInitializeAsync(request),
                "tools/list" => await HandleToolsListAsync(),
                "tools/call" => await HandleToolCallAsync(request),
                "ping" => new { success = true },
                _ => throw new Exception($"Unknown method: {method}")
            };

            return new
            {
                jsonrpc = "2.0",
                result,
                id = id.ValueKind != JsonValueKind.Undefined ? (object?)id : null
            };
        }
        catch (Exception ex)
        {
            return new
            {
                jsonrpc = "2.0",
                error = new
                {
                    code = -32603,
                    message = ex.Message
                },
                id = id.ValueKind != JsonValueKind.Undefined ? (object?)id : null
            };
        }
    }

    private Task<object> HandleInitializeAsync(JsonElement request)
    {
        return Task.FromResult<object>(new
        {
            protocolVersion = "2024-11-05",
            capabilities = new
            {
                tools = new { }
            },
            serverInfo = new
            {
                name = "dataverse-metadata-mcp",
                version = "1.0.0"
            }
        });
    }

    private Task<object> HandleToolsListAsync()
    {
        var tools = new List<object>
        {
            new
            {
                name = "get_environment_info",
                description = "Get information about the current Dataverse environment including connection status and version",
                inputSchema = new
                {
                    type = "object",
                    properties = new { },
                    required = Array.Empty<string>()
                }
            },
            new
            {
                name = "list_environments",
                description = "List all available Dataverse environments configured in the server",
                inputSchema = new
                {
                    type = "object",
                    properties = new { },
                    required = Array.Empty<string>()
                }
            },
            new
            {
                name = "set_environment",
                description = "Switch to a different configured Dataverse environment",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        environmentName = new
                        {
                            type = "string",
                            description = "Name of the environment to switch to (e.g., development, production, sandbox)"
                        }
                    },
                    required = new[] { "environmentName" }
                }
            },
            new
            {
                name = "list_entities",
                description = "List all entities (tables) in the current Dataverse environment with optional filtering",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        customOnly = new
                        {
                            type = "boolean",
                            description = "If true, only return custom entities; if false, return all entities",
                            @default = false
                        }
                    },
                    required = Array.Empty<string>()
                }
            },
            new
            {
                name = "get_entity_details",
                description = "Get detailed metadata for a specific entity (table) including all its properties and settings",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        entityLogicalName = new
                        {
                            type = "string",
                            description = "Logical name of the entity (e.g., account, contact, custom_entity)"
                        }
                    },
                    required = new[] { "entityLogicalName" }
                }
            },
            new
            {
                name = "list_entity_attributes",
                description = "List all attributes (columns/fields) for a specific entity with their metadata",
                inputSchema = new
                {
                    type = "object",
                    properties = new
                    {
                        entityLogicalName = new
                        {
                            type = "string",
                            description = "Logical name of the entity (e.g., account, contact)"
                        },
                        customOnly = new
                        {
                            type = "boolean",
                            description = "If true, only return custom attributes; if false, return all attributes",
                            @default = false
                        }
                    },
                    required = new[] { "entityLogicalName" }
                }
            }
        };

        return Task.FromResult<object>(new { tools });
    }

    private async Task<object> HandleToolCallAsync(JsonElement request)
    {
        var paramsElement = request.GetProperty("params");
        var toolName = paramsElement.GetProperty("name").GetString();
        var arguments = paramsElement.TryGetProperty("arguments", out var argsElement) 
            ? argsElement 
            : default;

        object result = toolName switch
        {
            "get_environment_info" => await _tools.GetEnvironmentInfo(),
            "list_environments" => await _tools.ListEnvironments(),
            "set_environment" => await _tools.SetEnvironment(
                arguments.GetProperty("environmentName").GetString()!),
            "list_entities" => await _tools.ListEntities(
                arguments.TryGetProperty("customOnly", out var customOnlyProp) && customOnlyProp.GetBoolean()),
            "get_entity_details" => await _tools.GetEntityDetails(
                arguments.GetProperty("entityLogicalName").GetString()!),
            "list_entity_attributes" => await _tools.ListEntityAttributes(
                arguments.GetProperty("entityLogicalName").GetString()!,
                arguments.TryGetProperty("customOnly", out var customOnlyProp2) && customOnlyProp2.GetBoolean()),
            _ => throw new Exception($"Unknown tool: {toolName}")
        };

        return new
        {
            content = new[]
            {
                new
                {
                    type = "text",
                    text = JsonSerializer.Serialize(result, _jsonOptions)
                }
            }
        };
    }
}

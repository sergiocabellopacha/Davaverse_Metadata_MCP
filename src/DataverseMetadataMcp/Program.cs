using DataverseMetadataMcp.Configuration;
using DataverseMetadataMcp.Services;
using DataverseMetadataMcp.Tools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DataverseMetadataMcp;

/// <summary>
/// Entry point for the Dataverse Metadata MCP Server
/// </summary>
internal class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            // Create host builder with MCP server configuration
            var builder = Host.CreateApplicationBuilder(args);
            
            // Configure logging to stderr for MCP compatibility
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole(options =>
            {
                options.LogToStandardErrorThreshold = LogLevel.Trace;
            });
            builder.Logging.SetMinimumLevel(LogLevel.Information);

            // Add configuration from multiple sources
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddJsonFile("config/server-config.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables("DATAVERSE_MCP_")
                .AddCommandLine(args);

            // Configure Dataverse settings
            builder.Services.Configure<DataverseConfig>(
                builder.Configuration.GetSection(DataverseConfig.SectionName));

            // Register services
            builder.Services.AddSingleton<IDataverseService, DataverseService>();
            builder.Services.AddSingleton<DataverseTools>();

            // Build and run the host
            var host = builder.Build();
            
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Starting Dataverse Metadata MCP Server...");

            // Validate configuration on startup
            await ValidateConfigurationAsync(host.Services, logger);
            
            logger.LogInformation("Dataverse Metadata MCP Server started successfully");
            
            // Start MCP server with stdio transport
            var mcpServer = new McpServer(host.Services);
            await mcpServer.RunAsync();
        }
        catch (Exception ex)
        {
            // Use Console.Error for critical startup errors since logging might not be configured
            await Console.Error.WriteLineAsync($"Critical error starting Dataverse Metadata MCP Server: {ex}");
            Environment.Exit(1);
        }
    }

    /// <summary>
    /// Validate configuration on startup
    /// </summary>
    private static async Task ValidateConfigurationAsync(IServiceProvider services, ILogger logger)
    {
        try
        {
            var dataverseService = services.GetRequiredService<IDataverseService>();
            
            // Check if we have any environments configured
            var environments = await dataverseService.GetAvailableEnvironmentsAsync();
            
            if (!environments.Any())
            {
                logger.LogWarning("No Dataverse environments configured. Please configure at least one environment in the configuration file.");
                return;
            }

            logger.LogInformation("Found {Count} configured environments: {Environments}", 
                environments.Count, 
                string.Join(", ", environments.Select(kvp => $"{kvp.Key} ({kvp.Value})")));

            // Test connection to current environment
            var (currentEnv, displayName) = await dataverseService.GetCurrentEnvironmentAsync();
            logger.LogInformation("Current environment: {Environment} ({DisplayName})", currentEnv, displayName);

            // Optionally test the connection (comment out if you don't want to test on startup)
            logger.LogInformation("Testing connection to current environment...");
            var (success, message, orgName, version) = await dataverseService.TestConnectionAsync();
            
            if (success)
            {
                logger.LogInformation("Successfully connected to {Organization} (v{Version})", orgName, version);
            }
            else
            {
                logger.LogWarning("Failed to connect to current environment: {Message}", message);
                logger.LogWarning("The server will start, but you may need to check your configuration or authentication settings.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating configuration. The server will continue to start, but there may be configuration issues.");
        }
    }
}
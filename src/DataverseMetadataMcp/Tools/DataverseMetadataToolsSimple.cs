using DataverseMetadataMcp.Models;
using DataverseMetadataMcp.Services;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace DataverseMetadataMcp.Tools;

/// <summary>
/// Tools for querying Dataverse metadata
/// </summary>
public class DataverseTools
{
    private readonly IDataverseService _dataverseService;
    private readonly ILogger<DataverseTools> _logger;

    public DataverseTools(IDataverseService dataverseService, ILogger<DataverseTools> logger)
    {
        _dataverseService = dataverseService;
        _logger = logger;
    }

    /// <summary>
    /// Get information about the current Dataverse environment
    /// </summary>
    [Description("Get information about the current Dataverse environment including connection status and version")]
    public async Task<object> GetEnvironmentInfo(CancellationToken cancellationToken = default)
    {
        try
        {
            var (envName, displayName) = await _dataverseService.GetCurrentEnvironmentAsync();
            var (success, message, orgName, version) = await _dataverseService.TestConnectionAsync(cancellationToken);
            
            return new
            {
                currentEnvironment = envName,
                displayName,
                connectionStatus = success ? "Connected" : "Failed",
                connectionMessage = message,
                organizationName = orgName,
                dataverseVersion = version,
                timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting environment info");
            return new { error = ex.Message };
        }
    }

    /// <summary>
    /// List all available Dataverse environments configured in the server
    /// </summary>
    [Description("List all available Dataverse environments configured in the server")]
    public async Task<object> ListEnvironments()
    {
        try
        {
            var environments = await _dataverseService.GetAvailableEnvironmentsAsync();
            var (currentEnv, _) = await _dataverseService.GetCurrentEnvironmentAsync();
            
            return new
            {
                currentEnvironment = currentEnv,
                availableEnvironments = environments.Select(kvp => new
                {
                    name = kvp.Key,
                    displayName = kvp.Value,
                    isCurrent = kvp.Key == currentEnv
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing environments");
            return new { error = ex.Message };
        }
    }

    /// <summary>
    /// Switch to a different Dataverse environment
    /// </summary>
    [Description("Switch to a different configured Dataverse environment")]
    public async Task<object> SetEnvironment(
        [Description("Name of the environment to switch to (e.g., development, production, sandbox)")]
        string environmentName, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (success, message) = await _dataverseService.SetEnvironmentAsync(environmentName, cancellationToken);
            
            return new
            {
                success,
                message,
                newEnvironment = success ? environmentName : null,
                timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting environment to: {Environment}", environmentName);
            return new { error = ex.Message };
        }
    }

    /// <summary>
    /// List all entities in the current Dataverse environment
    /// </summary>
    [Description("List all entities (tables) in the current Dataverse environment with optional filtering")]
    public async Task<object> ListEntities(
        [Description("If true, only return custom entities; if false, return all entities")]
        bool customOnly = false, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entities = await _dataverseService.GetEntitiesAsync(customOnly, cancellationToken);
            
            return new
            {
                totalCount = entities.Count,
                customOnly,
                entities = entities.Select(e => new
                {
                    logicalName = e.LogicalName,
                    displayName = e.DisplayName,
                    displayCollectionName = e.DisplayCollectionName,
                    description = e.Description,
                    entitySetName = e.EntitySetName,
                    primaryIdAttribute = e.PrimaryIdAttribute,
                    primaryNameAttribute = e.PrimaryNameAttribute,
                    isCustomEntity = e.IsCustomEntity,
                    isActivity = e.IsActivity,
                    objectTypeCode = e.ObjectTypeCode
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing entities");
            return new { error = ex.Message };
        }
    }

    /// <summary>
    /// Get detailed metadata for a specific entity
    /// </summary>
    [Description("Get detailed metadata for a specific entity (table) including all its properties and settings")]
    public async Task<object> GetEntityDetails(
        [Description("Logical name of the entity (e.g., account, contact, custom_entity)")]
        string entityLogicalName, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _dataverseService.GetEntityDetailsAsync(entityLogicalName, cancellationToken);
            
            if (entity == null)
            {
                return new { error = $"Entity '{entityLogicalName}' not found" };
            }

            return new
            {
                entityMetadata = entity,
                retrievedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity details for: {EntityName}", entityLogicalName);
            return new { error = ex.Message };
        }
    }

    /// <summary>
    /// List all attributes (fields) for a specific entity
    /// </summary>
    [Description("List all attributes (columns/fields) for a specific entity with their metadata")]
    public async Task<object> ListEntityAttributes(
        [Description("Logical name of the entity (e.g., account, contact)")]
        string entityLogicalName, 
        [Description("If true, only return custom attributes; if false, return all attributes")]
        bool customOnly = false, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var attributes = await _dataverseService.GetEntityAttributesAsync(entityLogicalName, customOnly, cancellationToken);
            
            return new
            {
                entityLogicalName,
                totalCount = attributes.Count,
                customOnly,
                attributes = attributes.Select(a => new
                {
                    logicalName = a.LogicalName,
                    displayName = a.DisplayName,
                    description = a.Description,
                    attributeType = a.AttributeType,
                    isRequired = a.IsRequired,
                    isPrimaryId = a.IsPrimaryId,
                    isPrimaryName = a.IsPrimaryName,
                    isCustomAttribute = a.IsCustomAttribute,
                    canRead = a.CanRead,
                    canUpdate = a.CanUpdate,
                    maxLength = a.MaxLength,
                    lookupTargetEntity = a.LookupTargetEntity,
                    optionSetValuesCount = a.OptionSetValues?.Count
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing attributes for entity: {EntityName}", entityLogicalName);
            return new { error = ex.Message };
        }
    }
}
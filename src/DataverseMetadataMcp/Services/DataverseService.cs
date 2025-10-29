using DataverseMetadataMcp.Configuration;
using DataverseMetadataMcp.Models;
using DvEntityMetadata = DataverseMetadataMcp.Models.EntityMetadata;
using DvAttributeMetadata = DataverseMetadataMcp.Models.AttributeMetadata;
using DvRelationshipMetadata = DataverseMetadataMcp.Models.RelationshipMetadata;
using DvOptionMetadata = DataverseMetadataMcp.Models.OptionMetadata;
using XrmCascadeConfiguration = Microsoft.Xrm.Sdk.Metadata.CascadeConfiguration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System.ComponentModel;

namespace DataverseMetadataMcp.Services;

/// <summary>
/// Service for interacting with Dataverse metadata
/// </summary>
public class DataverseService : IDataverseService, IDisposable
{
    private readonly ILogger<DataverseService> _logger;
    private readonly IOptionsMonitor<DataverseConfig> _config;
    private ServiceClient? _serviceClient;
    private string _currentEnvironment;
    private readonly object _lock = new();

    public DataverseService(ILogger<DataverseService> logger, IOptionsMonitor<DataverseConfig> config)
    {
        _logger = logger;
        _config = config;
        _currentEnvironment = _config.CurrentValue.CurrentEnvironment;
    }

    public Task<(string EnvironmentName, string DisplayName)> GetCurrentEnvironmentAsync()
    {
        var config = _config.CurrentValue;
        var environment = config.Environments.GetValueOrDefault(_currentEnvironment);
        var displayName = environment?.DisplayName ?? "Unknown";
        
        return Task.FromResult((_currentEnvironment, displayName));
    }

    public async Task<(bool Success, string Message)> SetEnvironmentAsync(string environmentName, CancellationToken cancellationToken = default)
    {
        try
        {
            var config = _config.CurrentValue;
            
            if (!config.Environments.ContainsKey(environmentName))
            {
                return (false, $"Environment '{environmentName}' not found in configuration");
            }

            lock (_lock)
            {
                // Dispose current connection
                _serviceClient?.Dispose();
                _serviceClient = null;
                _currentEnvironment = environmentName;
            }

            // Test the new connection
            var (success, message, _, _) = await TestConnectionAsync(cancellationToken);
            
            if (success)
            {
                _logger.LogInformation("Successfully switched to environment: {Environment}", environmentName);
                return (true, $"Successfully switched to environment '{environmentName}'");
            }
            else
            {
                return (false, $"Failed to connect to environment '{environmentName}': {message}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error switching to environment: {Environment}", environmentName);
            return (false, $"Error switching environment: {ex.Message}");
        }
    }

    public Task<Dictionary<string, string>> GetAvailableEnvironmentsAsync()
    {
        var config = _config.CurrentValue;
        var environments = config.Environments.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.DisplayName
        );
        
        return Task.FromResult(environments);
    }

    public async Task<(bool Success, string Message, string? OrganizationName, string? Version)> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var client = await GetServiceClientAsync(cancellationToken);
            
            if (client?.IsReady == true)
            {
                var orgName = client.ConnectedOrgFriendlyName;
                var version = client.ConnectedOrgVersion?.ToString();
                
                _logger.LogInformation("Successfully connected to {Organization} (v{Version})", orgName, version);
                return (true, "Connection successful", orgName, version);
            }
            else
            {
                var error = client?.LastError ?? "Unknown connection error";
                _logger.LogError("Failed to connect to Dataverse: {Error}", error);
                return (false, error, null, null);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing Dataverse connection");
            return (false, ex.Message, null, null);
        }
    }

    public async Task<List<DvEntityMetadata>> GetEntitiesAsync(bool includeCustomOnly = false, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = await GetServiceClientAsync(cancellationToken);
            
            var request = new RetrieveAllEntitiesRequest
            {
                EntityFilters = EntityFilters.Entity,
                RetrieveAsIfPublished = true
            };

            var response = (RetrieveAllEntitiesResponse)await client.ExecuteAsync(request);
            var entities = new List<DvEntityMetadata>();

            foreach (var entityMetadata in response.EntityMetadata)
            {
                if (includeCustomOnly && !entityMetadata.IsCustomEntity.GetValueOrDefault())
                    continue;

                entities.Add(MapToEntityMetadata(entityMetadata));
            }

            _logger.LogInformation("Retrieved {Count} entities (CustomOnly: {CustomOnly})", entities.Count, includeCustomOnly);
            return entities.OrderBy(e => e.DisplayName).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving entities");
            throw;
        }
    }

    public async Task<DvEntityMetadata?> GetEntityDetailsAsync(string entityLogicalName, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = await GetServiceClientAsync(cancellationToken);
            
            var request = new RetrieveEntityRequest
            {
                LogicalName = entityLogicalName,
                EntityFilters = EntityFilters.All,
                RetrieveAsIfPublished = true
            };

            var response = (RetrieveEntityResponse)await client.ExecuteAsync(request);
            var result = MapToEntityMetadata(response.EntityMetadata);
            
            _logger.LogInformation("Retrieved details for entity: {EntityName}", entityLogicalName);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving entity details for: {EntityName}", entityLogicalName);
            return null;
        }
    }

    public async Task<List<DvAttributeMetadata>> GetEntityAttributesAsync(string entityLogicalName, bool includeCustomOnly = false, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = await GetServiceClientAsync(cancellationToken);
            
            var request = new RetrieveEntityRequest
            {
                LogicalName = entityLogicalName,
                EntityFilters = EntityFilters.Attributes,
                RetrieveAsIfPublished = true
            };

            var response = (RetrieveEntityResponse)await client.ExecuteAsync(request);
            var attributes = new List<DvAttributeMetadata>();

            foreach (var attributeMetadata in response.EntityMetadata.Attributes)
            {
                if (includeCustomOnly && !attributeMetadata.IsCustomAttribute.GetValueOrDefault())
                    continue;

                attributes.Add(MapToAttributeMetadata(attributeMetadata));
            }

            _logger.LogInformation("Retrieved {Count} attributes for entity {EntityName} (CustomOnly: {CustomOnly})", 
                attributes.Count, entityLogicalName, includeCustomOnly);
            return attributes.OrderBy(a => a.DisplayName).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving attributes for entity: {EntityName}", entityLogicalName);
            throw;
        }
    }

    public async Task<DvAttributeMetadata?> GetAttributeDetailsAsync(string entityLogicalName, string attributeLogicalName, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = await GetServiceClientAsync(cancellationToken);
            
            var request = new RetrieveAttributeRequest
            {
                EntityLogicalName = entityLogicalName,
                LogicalName = attributeLogicalName,
                RetrieveAsIfPublished = true
            };

            var response = (RetrieveAttributeResponse)await client.ExecuteAsync(request);
            var result = MapToAttributeMetadata(response.AttributeMetadata);
            
            _logger.LogInformation("Retrieved details for attribute: {EntityName}.{AttributeName}", entityLogicalName, attributeLogicalName);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving attribute details for: {EntityName}.{AttributeName}", entityLogicalName, attributeLogicalName);
            return null;
        }
    }

    public async Task<List<DvRelationshipMetadata>> GetEntityRelationshipsAsync(string entityLogicalName, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = await GetServiceClientAsync(cancellationToken);
            
            var request = new RetrieveEntityRequest
            {
                LogicalName = entityLogicalName,
                EntityFilters = EntityFilters.Relationships,
                RetrieveAsIfPublished = true
            };

            var response = (RetrieveEntityResponse)await client.ExecuteAsync(request);
            var relationships = new List<DvRelationshipMetadata>();

            // One-to-Many relationships where this entity is the primary
            foreach (var rel in response.EntityMetadata.OneToManyRelationships ?? Array.Empty<OneToManyRelationshipMetadata>())
            {
                relationships.Add(MapToRelationshipMetadata(rel, RelationshipTypes.OneToMany));
            }

            // Many-to-One relationships where this entity is the related
            foreach (var rel in response.EntityMetadata.ManyToOneRelationships ?? Array.Empty<OneToManyRelationshipMetadata>())
            {
                relationships.Add(MapToRelationshipMetadata(rel, RelationshipTypes.ManyToOne));
            }

            // Many-to-Many relationships
            foreach (var rel in response.EntityMetadata.ManyToManyRelationships ?? Array.Empty<ManyToManyRelationshipMetadata>())
            {
                relationships.Add(MapToRelationshipMetadata(rel));
            }

            _logger.LogInformation("Retrieved {Count} relationships for entity: {EntityName}", relationships.Count, entityLogicalName);
            return relationships.OrderBy(r => r.SchemaName).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving relationships for entity: {EntityName}", entityLogicalName);
            throw;
        }
    }

    public async Task<List<DvEntityMetadata>> SearchEntitiesAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            var allEntities = await GetEntitiesAsync(false, cancellationToken);
            
            var searchTermLower = searchTerm.ToLowerInvariant();
            var results = allEntities.Where(e =>
                e.LogicalName.Contains(searchTermLower, StringComparison.OrdinalIgnoreCase) ||
                e.DisplayName.Contains(searchTermLower, StringComparison.OrdinalIgnoreCase) ||
                e.DisplayCollectionName.Contains(searchTermLower, StringComparison.OrdinalIgnoreCase) ||
                (e.Description?.Contains(searchTermLower, StringComparison.OrdinalIgnoreCase) == true)
            ).ToList();

            _logger.LogInformation("Found {Count} entities matching search term: {SearchTerm}", results.Count, searchTerm);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching entities with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<List<DvOptionMetadata>> GetGlobalOptionSetValuesAsync(string optionSetName, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = await GetServiceClientAsync(cancellationToken);
            
            var request = new RetrieveOptionSetRequest
            {
                Name = optionSetName,
                RetrieveAsIfPublished = true
            };

            var response = (RetrieveOptionSetResponse)await client.ExecuteAsync(request);
            
            if (response.OptionSetMetadata is OptionSetMetadata optionSetMetadata)
            {
                var options = new List<DvOptionMetadata>();
                
                foreach (var option in optionSetMetadata.Options)
                {
                    options.Add(new DvOptionMetadata
                    {
                        Value = option.Value.GetValueOrDefault(),
                        Label = option.Label?.UserLocalizedLabel?.Label ?? string.Empty,
                        Description = option.Description?.UserLocalizedLabel?.Label,
                        Color = option.Color,
                        IsDefault = option.IsManaged.GetValueOrDefault()
                    });
                }

                _logger.LogInformation("Retrieved {Count} options for global option set: {OptionSetName}", options.Count, optionSetName);
                return options.OrderBy(o => o.Value).ToList();
            }

            return new List<DvOptionMetadata>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving global option set values for: {OptionSetName}", optionSetName);
            throw;
        }
    }

    private async Task<ServiceClient> GetServiceClientAsync(CancellationToken cancellationToken = default)
    {
        if (_serviceClient?.IsReady == true)
            return _serviceClient;

        lock (_lock)
        {
            if (_serviceClient?.IsReady == true)
                return _serviceClient;

            _serviceClient?.Dispose();
            _serviceClient = CreateServiceClient();
        }

        // Wait for connection to be ready
        var timeout = TimeSpan.FromSeconds(30);
        var startTime = DateTime.UtcNow;
        
        while (!_serviceClient.IsReady && DateTime.UtcNow - startTime < timeout)
        {
            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException();
                
            await Task.Delay(100, cancellationToken);
        }

        if (!_serviceClient.IsReady)
        {
            throw new InvalidOperationException($"Failed to connect to Dataverse within timeout period. Error: {_serviceClient.LastError}");
        }

        return _serviceClient;
    }

    private ServiceClient CreateServiceClient()
    {
        var config = _config.CurrentValue;
        
        if (!config.Environments.TryGetValue(_currentEnvironment, out var environment))
        {
            throw new InvalidOperationException($"Environment '{_currentEnvironment}' not found in configuration");
        }

        var connectionString = BuildConnectionString(environment);
        
        _logger.LogInformation("Creating Dataverse connection to: {OrganizationUrl}", environment.OrganizationUrl);
        
        return new ServiceClient(connectionString);
    }

    private static string BuildConnectionString(DataverseEnvironment environment)
    {
        var connectionStringBuilder = new System.Text.StringBuilder();
        
        connectionStringBuilder.Append($"ServiceUri={environment.OrganizationUrl};");
        
        if (environment.Authentication.AuthType == AuthTypes.ServicePrincipal)
        {
            if (string.IsNullOrEmpty(environment.Authentication.ClientSecret))
            {
                throw new InvalidOperationException("ClientSecret is required for ServicePrincipal authentication");
            }
            
            if (string.IsNullOrEmpty(environment.Authentication.ClientId))
            {
                throw new InvalidOperationException("ClientId is required for ServicePrincipal authentication");
            }
            
            connectionStringBuilder.Append("AuthType=ClientSecret;");
            connectionStringBuilder.Append($"ClientId={environment.Authentication.ClientId};");
            connectionStringBuilder.Append($"ClientSecret={environment.Authentication.ClientSecret};");
        }
        else if (environment.Authentication.AuthType == AuthTypes.Interactive)
        {
            connectionStringBuilder.Append("AuthType=OAuth;");
            
            // ClientId is optional - if not provided, uses default app ID: 51f81489-12ee-4a9e-aaae-a2591f45987d
            if (!string.IsNullOrEmpty(environment.Authentication.ClientId))
            {
                connectionStringBuilder.Append($"ClientId={environment.Authentication.ClientId};");
            }
            
            if (!string.IsNullOrEmpty(environment.Authentication.RedirectUri))
            {
                connectionStringBuilder.Append($"RedirectUri={environment.Authentication.RedirectUri};");
            }
            
            connectionStringBuilder.Append("LoginPrompt=Auto;");
        }
        else
        {
            throw new InvalidOperationException($"Unsupported authentication type: {environment.Authentication.AuthType}");
        }
        
        connectionStringBuilder.Append($"Timeout={TimeSpan.FromSeconds(environment.TimeoutSeconds)};");
        connectionStringBuilder.Append($"MaxRetries={environment.MaxRetryAttempts};");
        
        return connectionStringBuilder.ToString();
    }

    private static DvEntityMetadata MapToEntityMetadata(Microsoft.Xrm.Sdk.Metadata.EntityMetadata metadata)
    {
        return new DvEntityMetadata
        {
            LogicalName = metadata.LogicalName ?? string.Empty,
            DisplayName = metadata.DisplayName?.UserLocalizedLabel?.Label ?? string.Empty,
            DisplayCollectionName = metadata.DisplayCollectionName?.UserLocalizedLabel?.Label ?? string.Empty,
            Description = metadata.Description?.UserLocalizedLabel?.Label,
            EntitySetName = metadata.EntitySetName ?? string.Empty,
            PrimaryIdAttribute = metadata.PrimaryIdAttribute ?? string.Empty,
            PrimaryNameAttribute = metadata.PrimaryNameAttribute,
            IsCustomEntity = metadata.IsCustomEntity.GetValueOrDefault(),
            IsManaged = metadata.IsManaged.GetValueOrDefault(),
            ObjectTypeCode = metadata.ObjectTypeCode,
            SchemaName = metadata.SchemaName ?? string.Empty,
            IsActivity = metadata.IsActivity.GetValueOrDefault(),
            IsIntersect = metadata.IsIntersect.GetValueOrDefault(),
            Capabilities = new EntityCapabilities
            {
                CanCreate = true, // Default to true, can be refined later
                CanRead = true,
                CanUpdate = true,
                CanDelete = true,
                CanBeInBusinessProcess = metadata.CanBeInManyToMany?.Value == true,
                CanBeRelatedEntityInRelationship = metadata.CanBeRelatedEntityInRelationship?.Value == true,
                CanBePrimaryEntityInRelationship = metadata.CanBePrimaryEntityInRelationship?.Value == true
            }
        };
    }

    private static DvAttributeMetadata MapToAttributeMetadata(Microsoft.Xrm.Sdk.Metadata.AttributeMetadata metadata)
    {
        var result = new DvAttributeMetadata
        {
            LogicalName = metadata.LogicalName ?? string.Empty,
            DisplayName = metadata.DisplayName?.UserLocalizedLabel?.Label ?? string.Empty,
            Description = metadata.Description?.UserLocalizedLabel?.Label,
            SchemaName = metadata.SchemaName ?? string.Empty,
            AttributeType = metadata.AttributeType?.ToString() ?? string.Empty,
            IsPrimaryId = metadata.IsPrimaryId.GetValueOrDefault(),
            IsPrimaryName = metadata.IsPrimaryName.GetValueOrDefault(),
            IsCustomAttribute = metadata.IsCustomAttribute.GetValueOrDefault(),
            IsManaged = metadata.IsManaged.GetValueOrDefault(),
            IsRequired = metadata.RequiredLevel?.Value == AttributeRequiredLevel.ApplicationRequired,
            CanRead = metadata.IsValidForRead == true,
            CanUpdate = metadata.IsValidForUpdate == true
        };

        // Handle specific attribute types
        switch (metadata)
        {
            case StringAttributeMetadata stringAttr:
                result.MaxLength = stringAttr.MaxLength;
                result.Format = stringAttr.Format?.ToString();
                break;
                
            case IntegerAttributeMetadata intAttr:
                result.MinValue = intAttr.MinValue;
                result.MaxValue = intAttr.MaxValue;
                result.Format = intAttr.Format?.ToString();
                break;
                
            case DecimalAttributeMetadata decimalAttr:
                result.MinValue = decimalAttr.MinValue;
                result.MaxValue = decimalAttr.MaxValue;
                result.Precision = decimalAttr.Precision;
                break;
                
            case MoneyAttributeMetadata moneyAttr:
                result.MinValue = moneyAttr.MinValue;
                result.MaxValue = moneyAttr.MaxValue;
                result.Precision = moneyAttr.Precision;
                result.PrecisionSource = moneyAttr.PrecisionSource;
                break;
                
            case DateTimeAttributeMetadata dateTimeAttr:
                result.Format = dateTimeAttr.Format?.ToString();
                break;
                
            case LookupAttributeMetadata lookupAttr:
                result.LookupTargetEntity = lookupAttr.Targets?.FirstOrDefault();
                break;
                
            case PicklistAttributeMetadata picklistAttr:
                result.IsGlobalOptionSet = picklistAttr.OptionSet?.IsGlobal == true;
                result.GlobalOptionSetName = picklistAttr.OptionSet?.Name;
                result.OptionSetValues = picklistAttr.OptionSet?.Options?.Select(o => new DvOptionMetadata
                {
                    Value = o.Value.GetValueOrDefault(),
                    Label = o.Label?.UserLocalizedLabel?.Label ?? string.Empty,
                    Description = o.Description?.UserLocalizedLabel?.Label,
                    Color = o.Color
                }).ToList();
                break;
        }

        return result;
    }

    private static DvRelationshipMetadata MapToRelationshipMetadata(OneToManyRelationshipMetadata metadata, string relationshipType)
    {
        return new DvRelationshipMetadata
        {
            SchemaName = metadata.SchemaName ?? string.Empty,
            RelationshipType = relationshipType,
            PrimaryEntity = metadata.ReferencedEntity ?? string.Empty,
            RelatedEntity = metadata.ReferencingEntity ?? string.Empty,
            IsCustomRelationship = metadata.IsCustomRelationship.GetValueOrDefault(),
            IsManaged = metadata.IsManaged.GetValueOrDefault(),
            LookupAttributeName = metadata.ReferencingAttribute,
            ReferencedEntityNavigationPropertyName = metadata.ReferencedEntityNavigationPropertyName,
            ReferencingEntityNavigationPropertyName = metadata.ReferencingEntityNavigationPropertyName,
            CascadeConfiguration = metadata.CascadeConfiguration != null ? new Models.CascadeConfiguration
            {
                Delete = metadata.CascadeConfiguration.Delete.ToString(),
                Assign = metadata.CascadeConfiguration.Assign.ToString(),
                Share = metadata.CascadeConfiguration.Share.ToString(),
                Unshare = metadata.CascadeConfiguration.Unshare.ToString(),
                Reparent = metadata.CascadeConfiguration.Reparent.ToString(),
                Rollup = metadata.CascadeConfiguration.RollupView.ToString()
            } : null
        };
    }

    private static DvRelationshipMetadata MapToRelationshipMetadata(ManyToManyRelationshipMetadata metadata)
    {
        return new DvRelationshipMetadata
        {
            SchemaName = metadata.SchemaName ?? string.Empty,
            RelationshipType = RelationshipTypes.ManyToMany,
            PrimaryEntity = metadata.Entity1LogicalName ?? string.Empty,
            RelatedEntity = metadata.Entity2LogicalName ?? string.Empty,
            IsCustomRelationship = metadata.IsCustomRelationship.GetValueOrDefault(),
            IsManaged = metadata.IsManaged.GetValueOrDefault(),
            IntersectEntityName = metadata.IntersectEntityName,
            ReferencedEntityNavigationPropertyName = metadata.Entity1NavigationPropertyName,
            ReferencingEntityNavigationPropertyName = metadata.Entity2NavigationPropertyName
        };
    }

    public void Dispose()
    {
        _serviceClient?.Dispose();
        GC.SuppressFinalize(this);
    }
}
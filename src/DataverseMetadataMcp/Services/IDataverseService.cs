using DataverseMetadataMcp.Models;
using DvEntityMetadata = DataverseMetadataMcp.Models.EntityMetadata;
using DvAttributeMetadata = DataverseMetadataMcp.Models.AttributeMetadata;
using DvRelationshipMetadata = DataverseMetadataMcp.Models.RelationshipMetadata;
using DvOptionMetadata = DataverseMetadataMcp.Models.OptionMetadata;

namespace DataverseMetadataMcp.Services;

/// <summary>
/// Service interface for interacting with Dataverse metadata
/// </summary>
public interface IDataverseService
{
    /// <summary>
    /// Get the current environment configuration being used
    /// </summary>
    /// <returns>Current environment name and display name</returns>
    Task<(string EnvironmentName, string DisplayName)> GetCurrentEnvironmentAsync();

    /// <summary>
    /// Switch to a different configured environment
    /// </summary>
    /// <param name="environmentName">Name of the environment to switch to</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status and message</returns>
    Task<(bool Success, string Message)> SetEnvironmentAsync(string environmentName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a list of all available environments
    /// </summary>
    /// <returns>Dictionary of environment names and their display names</returns>
    Task<Dictionary<string, string>> GetAvailableEnvironmentsAsync();

    /// <summary>
    /// Test the connection to the current Dataverse environment
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success status and connection details</returns>
    Task<(bool Success, string Message, string? OrganizationName, string? Version)> TestConnectionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a list of all entities in the current environment
    /// </summary>
    /// <param name="includeCustomOnly">If true, only return custom entities</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of entity metadata</returns>
    Task<List<DvEntityMetadata>> GetEntitiesAsync(bool includeCustomOnly = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get detailed metadata for a specific entity
    /// </summary>
    /// <param name="entityLogicalName">Logical name of the entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed entity metadata or null if not found</returns>
    Task<DvEntityMetadata?> GetEntityDetailsAsync(string entityLogicalName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all attributes for a specific entity
    /// </summary>
    /// <param name="entityLogicalName">Logical name of the entity</param>
    /// <param name="includeCustomOnly">If true, only return custom attributes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of attribute metadata</returns>
    Task<List<DvAttributeMetadata>> GetEntityAttributesAsync(string entityLogicalName, bool includeCustomOnly = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get detailed metadata for a specific attribute
    /// </summary>
    /// <param name="entityLogicalName">Logical name of the entity</param>
    /// <param name="attributeLogicalName">Logical name of the attribute</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Detailed attribute metadata or null if not found</returns>
    Task<DvAttributeMetadata?> GetAttributeDetailsAsync(string entityLogicalName, string attributeLogicalName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all relationships for a specific entity
    /// </summary>
    /// <param name="entityLogicalName">Logical name of the entity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of relationship metadata</returns>
    Task<List<DvRelationshipMetadata>> GetEntityRelationshipsAsync(string entityLogicalName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Search for entities by name or description
    /// </summary>
    /// <param name="searchTerm">Term to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching entity metadata</returns>
    Task<List<DvEntityMetadata>> SearchEntitiesAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get values for a global option set
    /// </summary>
    /// <param name="optionSetName">Name of the global option set</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of option metadata</returns>
    Task<List<DvOptionMetadata>> GetGlobalOptionSetValuesAsync(string optionSetName, CancellationToken cancellationToken = default);
}
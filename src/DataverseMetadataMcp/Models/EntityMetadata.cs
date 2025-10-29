using System.Text.Json.Serialization;

namespace DataverseMetadataMcp.Models;

/// <summary>
/// Represents metadata information for a Dataverse entity
/// </summary>
public class EntityMetadata
{
    /// <summary>
    /// Internal logical name of the entity (e.g., "account", "contact")
    /// </summary>
    [JsonPropertyName("logicalName")]
    public string LogicalName { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the entity (e.g., "Account", "Contact")
    /// </summary>
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Plural display name of the entity (e.g., "Accounts", "Contacts")
    /// </summary>
    [JsonPropertyName("displayCollectionName")]
    public string DisplayCollectionName { get; set; } = string.Empty;

    /// <summary>
    /// Description of the entity
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Entity set name used in OData queries
    /// </summary>
    [JsonPropertyName("entitySetName")]
    public string EntitySetName { get; set; } = string.Empty;

    /// <summary>
    /// Primary key attribute name
    /// </summary>
    [JsonPropertyName("primaryIdAttribute")]
    public string PrimaryIdAttribute { get; set; } = string.Empty;

    /// <summary>
    /// Primary name attribute (the main display field)
    /// </summary>
    [JsonPropertyName("primaryNameAttribute")]
    public string? PrimaryNameAttribute { get; set; }

    /// <summary>
    /// Whether this is a custom entity (created by users)
    /// </summary>
    [JsonPropertyName("isCustomEntity")]
    public bool IsCustomEntity { get; set; }

    /// <summary>
    /// Whether this entity is managed (part of a solution)
    /// </summary>
    [JsonPropertyName("isManaged")]
    public bool IsManaged { get; set; }

    /// <summary>
    /// Entity type code
    /// </summary>
    [JsonPropertyName("objectTypeCode")]
    public int? ObjectTypeCode { get; set; }

    /// <summary>
    /// Schema name of the entity
    /// </summary>
    [JsonPropertyName("schemaName")]
    public string SchemaName { get; set; } = string.Empty;

    /// <summary>
    /// Whether the entity has activities associated with it
    /// </summary>
    [JsonPropertyName("isActivity")]
    public bool IsActivity { get; set; }

    /// <summary>
    /// Whether the entity is intersect (many-to-many relationship table)
    /// </summary>
    [JsonPropertyName("isIntersect")]
    public bool IsIntersect { get; set; }

    /// <summary>
    /// Brief summary of entity capabilities
    /// </summary>
    [JsonPropertyName("capabilities")]
    public EntityCapabilities Capabilities { get; set; } = new();
}

/// <summary>
/// Represents the capabilities of a Dataverse entity
/// </summary>
public class EntityCapabilities
{
    /// <summary>
    /// Whether records can be created
    /// </summary>
    [JsonPropertyName("canCreate")]
    public bool CanCreate { get; set; }

    /// <summary>
    /// Whether records can be read
    /// </summary>
    [JsonPropertyName("canRead")]
    public bool CanRead { get; set; }

    /// <summary>
    /// Whether records can be updated
    /// </summary>
    [JsonPropertyName("canUpdate")]
    public bool CanUpdate { get; set; }

    /// <summary>
    /// Whether records can be deleted
    /// </summary>
    [JsonPropertyName("canDelete")]
    public bool CanDelete { get; set; }

    /// <summary>
    /// Whether the entity supports business process flows
    /// </summary>
    [JsonPropertyName("canBeInBusinessProcess")]
    public bool CanBeInBusinessProcess { get; set; }

    /// <summary>
    /// Whether the entity can be related to other entities
    /// </summary>
    [JsonPropertyName("canBeRelatedEntityInRelationship")]
    public bool CanBeRelatedEntityInRelationship { get; set; }

    /// <summary>
    /// Whether the entity can be the primary entity in a relationship
    /// </summary>
    [JsonPropertyName("canBePrimaryEntityInRelationship")]
    public bool CanBePrimaryEntityInRelationship { get; set; }
}
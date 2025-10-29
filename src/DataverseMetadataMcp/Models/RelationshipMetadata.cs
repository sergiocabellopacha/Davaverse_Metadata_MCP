using System.Text.Json.Serialization;

namespace DataverseMetadataMcp.Models;

/// <summary>
/// Represents metadata information for a Dataverse entity relationship
/// </summary>
public class RelationshipMetadata
{
    /// <summary>
    /// Schema name of the relationship
    /// </summary>
    [JsonPropertyName("schemaName")]
    public string SchemaName { get; set; } = string.Empty;

    /// <summary>
    /// Type of relationship: "OneToMany", "ManyToOne", or "ManyToMany"
    /// </summary>
    [JsonPropertyName("relationshipType")]
    public string RelationshipType { get; set; } = string.Empty;

    /// <summary>
    /// Logical name of the primary entity (the "one" side in 1:N or the first entity in N:N)
    /// </summary>
    [JsonPropertyName("primaryEntity")]
    public string PrimaryEntity { get; set; } = string.Empty;

    /// <summary>
    /// Logical name of the related entity (the "many" side in 1:N or the second entity in N:N)
    /// </summary>
    [JsonPropertyName("relatedEntity")]
    public string RelatedEntity { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the relationship
    /// </summary>
    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    /// <summary>
    /// Whether this is a custom relationship (created by users)
    /// </summary>
    [JsonPropertyName("isCustomRelationship")]
    public bool IsCustomRelationship { get; set; }

    /// <summary>
    /// Whether this relationship is managed (part of a solution)
    /// </summary>
    [JsonPropertyName("isManaged")]
    public bool IsManaged { get; set; }

    /// <summary>
    /// For OneToMany relationships, the lookup attribute name in the related entity
    /// </summary>
    [JsonPropertyName("lookupAttributeName")]
    public string? LookupAttributeName { get; set; }

    /// <summary>
    /// For ManyToMany relationships, the intersect entity name
    /// </summary>
    [JsonPropertyName("intersectEntityName")]
    public string? IntersectEntityName { get; set; }

    /// <summary>
    /// Cascade configuration for various operations
    /// </summary>
    [JsonPropertyName("cascadeConfiguration")]
    public CascadeConfiguration? CascadeConfiguration { get; set; }

    /// <summary>
    /// Navigation property name for accessing related records from primary entity
    /// </summary>
    [JsonPropertyName("referencedEntityNavigationPropertyName")]
    public string? ReferencedEntityNavigationPropertyName { get; set; }

    /// <summary>
    /// Navigation property name for accessing the primary record from related entity
    /// </summary>
    [JsonPropertyName("referencingEntityNavigationPropertyName")]
    public string? ReferencingEntityNavigationPropertyName { get; set; }

    /// <summary>
    /// Role name for this relationship (used in self-referencing relationships)
    /// </summary>
    [JsonPropertyName("roleName")]
    public string? RoleName { get; set; }
}

/// <summary>
/// Represents cascade behavior configuration for relationship operations
/// </summary>
public class CascadeConfiguration
{
    /// <summary>
    /// Behavior when the primary record is deleted
    /// </summary>
    [JsonPropertyName("delete")]
    public string Delete { get; set; } = string.Empty;

    /// <summary>
    /// Behavior when the primary record is assigned to another user
    /// </summary>
    [JsonPropertyName("assign")]
    public string Assign { get; set; } = string.Empty;

    /// <summary>
    /// Behavior when the primary record is shared
    /// </summary>
    [JsonPropertyName("share")]
    public string Share { get; set; } = string.Empty;

    /// <summary>
    /// Behavior when sharing of the primary record is removed
    /// </summary>
    [JsonPropertyName("unshare")]
    public string Unshare { get; set; } = string.Empty;

    /// <summary>
    /// Behavior when the primary record is reparented
    /// </summary>
    [JsonPropertyName("reparent")]
    public string Reparent { get; set; } = string.Empty;

    /// <summary>
    /// Behavior when the primary record's role is changed
    /// </summary>
    [JsonPropertyName("rollup")]
    public string Rollup { get; set; } = string.Empty;
}

/// <summary>
/// Common cascade behavior types
/// </summary>
public static class CascadeBehaviors
{
    public const string None = "None";
    public const string Cascade = "Cascade";
    public const string Active = "Active";
    public const string UserOwned = "UserOwned";
    public const string RemoveLink = "RemoveLink";
    public const string Restrict = "Restrict";
}

/// <summary>
/// Relationship types
/// </summary>
public static class RelationshipTypes
{
    public const string OneToMany = "OneToMany";
    public const string ManyToOne = "ManyToOne";
    public const string ManyToMany = "ManyToMany";
}
using System.Text.Json.Serialization;

namespace DataverseMetadataMcp.Models;

/// <summary>
/// Represents metadata information for a Dataverse entity attribute (field)
/// </summary>
public class AttributeMetadata
{
    /// <summary>
    /// Internal logical name of the attribute (e.g., "firstname", "accountid")
    /// </summary>
    [JsonPropertyName("logicalName")]
    public string LogicalName { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the attribute (e.g., "First Name", "Account")
    /// </summary>
    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Description of the attribute
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Schema name of the attribute
    /// </summary>
    [JsonPropertyName("schemaName")]
    public string SchemaName { get; set; } = string.Empty;

    /// <summary>
    /// Type of the attribute (e.g., "String", "Integer", "Lookup", "OptionSet")
    /// </summary>
    [JsonPropertyName("attributeType")]
    public string AttributeType { get; set; } = string.Empty;

    /// <summary>
    /// Whether this attribute is the primary key
    /// </summary>
    [JsonPropertyName("isPrimaryId")]
    public bool IsPrimaryId { get; set; }

    /// <summary>
    /// Whether this attribute is the primary name field
    /// </summary>
    [JsonPropertyName("isPrimaryName")]
    public bool IsPrimaryName { get; set; }

    /// <summary>
    /// Whether this is a custom attribute (created by users)
    /// </summary>
    [JsonPropertyName("isCustomAttribute")]
    public bool IsCustomAttribute { get; set; }

    /// <summary>
    /// Whether this attribute is managed (part of a solution)
    /// </summary>
    [JsonPropertyName("isManaged")]
    public bool IsManaged { get; set; }

    /// <summary>
    /// Whether this attribute is required
    /// </summary>
    [JsonPropertyName("isRequired")]
    public bool IsRequired { get; set; }

    /// <summary>
    /// Whether this attribute can be read
    /// </summary>
    [JsonPropertyName("canRead")]
    public bool CanRead { get; set; }

    /// <summary>
    /// Whether this attribute can be updated
    /// </summary>
    [JsonPropertyName("canUpdate")]
    public bool CanUpdate { get; set; }

    /// <summary>
    /// Maximum length for string attributes
    /// </summary>
    [JsonPropertyName("maxLength")]
    public int? MaxLength { get; set; }

    /// <summary>
    /// Minimum value for numeric attributes
    /// </summary>
    [JsonPropertyName("minValue")]
    public object? MinValue { get; set; }

    /// <summary>
    /// Maximum value for numeric attributes
    /// </summary>
    [JsonPropertyName("maxValue")]
    public object? MaxValue { get; set; }

    /// <summary>
    /// Precision for decimal attributes
    /// </summary>
    [JsonPropertyName("precision")]
    public int? Precision { get; set; }

    /// <summary>
    /// Number of decimal places for money/decimal attributes
    /// </summary>
    [JsonPropertyName("precisionSource")]
    public int? PrecisionSource { get; set; }

    /// <summary>
    /// Format for date/time attributes
    /// </summary>
    [JsonPropertyName("format")]
    public string? Format { get; set; }

    /// <summary>
    /// Default value for the attribute
    /// </summary>
    [JsonPropertyName("defaultValue")]
    public object? DefaultValue { get; set; }

    /// <summary>
    /// For lookup attributes, the target entity logical name
    /// </summary>
    [JsonPropertyName("lookupTargetEntity")]
    public string? LookupTargetEntity { get; set; }

    /// <summary>
    /// For lookup attributes, the relationship name
    /// </summary>
    [JsonPropertyName("lookupRelationshipName")]
    public string? LookupRelationshipName { get; set; }

    /// <summary>
    /// For OptionSet attributes, the available options
    /// </summary>
    [JsonPropertyName("optionSetValues")]
    public List<OptionMetadata>? OptionSetValues { get; set; }

    /// <summary>
    /// For OptionSet attributes, whether it's a global option set
    /// </summary>
    [JsonPropertyName("isGlobalOptionSet")]
    public bool? IsGlobalOptionSet { get; set; }

    /// <summary>
    /// For global option sets, the name of the option set
    /// </summary>
    [JsonPropertyName("globalOptionSetName")]
    public string? GlobalOptionSetName { get; set; }
}

/// <summary>
/// Represents an option in an OptionSet attribute
/// </summary>
public class OptionMetadata
{
    /// <summary>
    /// Numeric value of the option
    /// </summary>
    [JsonPropertyName("value")]
    public int Value { get; set; }

    /// <summary>
    /// Display label of the option
    /// </summary>
    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Description of the option
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Color associated with the option (if any)
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }

    /// <summary>
    /// Whether this is the default option
    /// </summary>
    [JsonPropertyName("isDefault")]
    public bool IsDefault { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace DataverseMetadataMcp.Configuration;

/// <summary>
/// Configuration settings for Dataverse connection
/// </summary>
public class DataverseConfig
{
    public const string SectionName = "Dataverse";

    /// <summary>
    /// Name of the current environment configuration to use
    /// </summary>
    [Required]
    public string CurrentEnvironment { get; set; } = string.Empty;

    /// <summary>
    /// Available environment configurations
    /// </summary>
    [Required]
    public Dictionary<string, DataverseEnvironment> Environments { get; set; } = new();
}

/// <summary>
/// Configuration for a specific Dataverse environment
/// </summary>
public class DataverseEnvironment
{
    /// <summary>
    /// Display name for this environment
    /// </summary>
    [Required]
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Dataverse organization URL (e.g., https://myorg.crm.dynamics.com)
    /// </summary>
    [Required]
    public string OrganizationUrl { get; set; } = string.Empty;

    /// <summary>
    /// Authentication configuration
    /// </summary>
    [Required]
    public DataverseAuth Authentication { get; set; } = new();

    /// <summary>
    /// Connection timeout in seconds (default: 30)
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Maximum retry attempts for failed requests (default: 3)
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;
}

/// <summary>
/// Authentication configuration for Dataverse
/// </summary>
public class DataverseAuth
{
    /// <summary>
    /// Type of authentication: "ServicePrincipal" or "Interactive"
    /// </summary>
    [Required]
    public string AuthType { get; set; } = string.Empty;

    /// <summary>
    /// Azure AD Tenant ID
    /// </summary>
    [Required]
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// Client ID (Application ID) for authentication
    /// </summary>
    [Required]
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Client Secret (only required for ServicePrincipal auth)
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Redirect URI for interactive authentication (optional)
    /// </summary>
    public string? RedirectUri { get; set; } = "http://localhost";
}

/// <summary>
/// Authentication types supported
/// </summary>
public static class AuthTypes
{
    public const string ServicePrincipal = "ServicePrincipal";
    public const string Interactive = "Interactive";
}
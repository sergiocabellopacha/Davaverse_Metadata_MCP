using DataverseMetadataMcp.Configuration;
using Xunit;

namespace DataverseMetadataMcp.Tests;

/// <summary>
/// Basic tests for configuration models
/// </summary>
public class ConfigurationTests
{
    [Fact]
    public void DataverseConfig_ShouldHaveCorrectSectionName()
    {
        // Arrange & Act
        var sectionName = DataverseConfig.SectionName;

        // Assert
        Assert.Equal("Dataverse", sectionName);
    }

    [Fact]
    public void AuthTypes_ShouldHaveCorrectConstants()
    {
        // Assert
        Assert.Equal("ServicePrincipal", AuthTypes.ServicePrincipal);
        Assert.Equal("Interactive", AuthTypes.Interactive);
    }

    [Fact]
    public void DataverseEnvironment_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var environment = new DataverseEnvironment();

        // Assert
        Assert.Equal(30, environment.TimeoutSeconds);
        Assert.Equal(3, environment.MaxRetryAttempts);
        Assert.NotNull(environment.Authentication);
        Assert.Equal(string.Empty, environment.DisplayName);
        Assert.Equal(string.Empty, environment.OrganizationUrl);
    }

    [Theory]
    [InlineData("ServicePrincipal")]
    [InlineData("Interactive")]
    public void DataverseAuth_ShouldAcceptValidAuthTypes(string authType)
    {
        // Arrange & Act
        var auth = new DataverseAuth { AuthType = authType };

        // Assert
        Assert.Equal(authType, auth.AuthType);
    }
}
using Microsoft.Extensions.Primitives;
using Task1.Domain;
using Task2.Provider;
using Xunit;

namespace Lab2.Tests;

public class ExternalConfigurationProviderTests
{
    [Fact]
    public void Scenario1_AddsSinglePair_TriggersReload()
    {
        // Arrange
        ExternalConfigurationProvider provider = new();
        IChangeToken t1 = provider.GetReloadToken();

        // Act
        bool reloaded = provider.TryApplyItems(new[] { new ConfigurationItem("k", "v") });
        IChangeToken t2 = provider.GetReloadToken();

        // Assert
        Assert.True(reloaded);
        Assert.NotSame(t1, t2);
        Assert.True(provider.TryGet("k", out string? val));
        Assert.Equal("v", val);
    }

    [Fact]
    public void Scenario2_SameData_NoReload()
    {
        // Arrange
        ExternalConfigurationProvider provider = new();
        provider.TryApplyItems(new[] { new ConfigurationItem("k", "v") });
        IChangeToken t1 = provider.GetReloadToken();

        // Act
        bool reloaded = provider.TryApplyItems(new[] { new ConfigurationItem("k", "v") });
        IChangeToken t2 = provider.GetReloadToken();

        // Assert
        Assert.False(reloaded);
        Assert.Same(t1, t2);
        Assert.True(provider.TryGet("k", out string? val));
        Assert.Equal("v", val);
    }

    [Fact]
    public void Scenario3_SameKey_NewValue_UpdatesAndReloads()
    {
        // Arrange
        ExternalConfigurationProvider provider = new();
        provider.TryApplyItems(new[] { new ConfigurationItem("k", "v1") });
        IChangeToken t1 = provider.GetReloadToken();

        // Act
        bool reloaded = provider.TryApplyItems(new[] { new ConfigurationItem("k", "v2") });
        IChangeToken t2 = provider.GetReloadToken();

        // Assert
        Assert.True(reloaded);
        Assert.NotSame(t1, t2);
        Assert.True(provider.TryGet("k", out string? val));
        Assert.Equal("v2", val);
    }

    [Fact]
    public void Scenario4_EmptyCollection_ClearsAndReloads()
    {
        // Arrange
        ExternalConfigurationProvider provider = new();
        provider.TryApplyItems(new[] { new ConfigurationItem("k", "v") });
        IChangeToken t1 = provider.GetReloadToken();

        // Act
        bool reloaded = provider.TryApplyItems(Array.Empty<ConfigurationItem>());
        IChangeToken t2 = provider.GetReloadToken();

        // Assert
        Assert.True(reloaded);
        Assert.NotSame(t1, t2);
        Assert.False(provider.TryGet("k", out _));
    }
}
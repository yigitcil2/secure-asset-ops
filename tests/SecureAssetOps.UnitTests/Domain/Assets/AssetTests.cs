using SecureAssetOps.Domain.Assets;
using SecureAssetOps.Domain.Assets.Events;
using Xunit;

namespace SecureAssetOps.UnitTests.Domain.Assets;

public sealed class AssetTests
{
    [Fact]
    public void Register_WithValidData_ShouldCreateAvailableAsset()
    {
        Guid id = Guid.NewGuid();

        var asset = Asset.Register(
            id,
            "AST-0001",
            "Dell Latitude 7440",
            "SN-123456");

        Assert.Equal(id, asset.Id);
        Assert.Equal("AST-0001", asset.AssetTag);
        Assert.Equal("Dell Latitude 7440", asset.Name);
        Assert.Equal("SN-123456", asset.SerialNumber);
        Assert.Equal(AssetStatus.Available, asset.Status);
    }

    [Fact]
    public void Register_WithEmptyAssetTag_ShouldThrowArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => Asset.Register(
                Guid.NewGuid(),
                "   ",
                "Dell Latitude 7440",
                "SN-123456"));

        Assert.Equal("assetTag", exception.ParamName);
    }

    [Fact]
    public void Register_WithEmptyName_ShouldThrowArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => Asset.Register(
                Guid.NewGuid(),
                "AST-0001",
                "   ",
                "SN-123456"));

        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Register_WithWhitespaceInValues_ShouldTrimValues()
    {
        var asset = Asset.Register(
            Guid.NewGuid(),
            "  AST-0001  ",
            "  Dell Latitude 7440  ",
            "  SN-123456  ");

        Assert.Equal("AST-0001", asset.AssetTag);
        Assert.Equal("Dell Latitude 7440", asset.Name);
        Assert.Equal("SN-123456", asset.SerialNumber);
    }

    [Fact]
    public void Register_WithEmptySerialNumber_ShouldSetSerialNumberToNull()
    {
        var asset = Asset.Register(
            Guid.NewGuid(),
            "AST-0001",
            "Dell Latitude 7440",
            "   ");

        Assert.Null(asset.SerialNumber);
    }

    [Fact]
    public void Register_WithValidData_ShouldRaiseAssetRegisteredDomainEvent()
    {
        Guid id = Guid.NewGuid();

        var asset = Asset.Register(
            id,
            "AST-0001",
            "Dell Latitude 7440",
            "SN-123456");

        AssetRegisteredDomainEvent domainEvent =
            Assert.IsType<AssetRegisteredDomainEvent>(
                Assert.Single(asset.DomainEvents));

        Assert.Equal(id, domainEvent.AssetId);
    }
}

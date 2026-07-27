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
    
    private static Asset CreateAvailableAsset()
    {
        return Asset.Register(
            Guid.NewGuid(),
            "AST-0001",
            "Dell Latitude Laptop",
            "SN-123456"
            );
    }
    [Fact]
    public void SendToMaintenance_WhenAssetIsAvailable_ShouldSetStatusToMaintenance()
    {
        var asset = CreateAvailableAsset();
        asset.SendToMaintenance();
        asset.ClearDomainEvents();

        Assert.Throws<InvalidOperationException>(() => asset.SendToMaintenance());

        Assert.Equal(AssetStatus.Maintenance, asset.Status);
        Assert.Empty(asset.DomainEvents);
    }
    [Fact]
    public void CompleteMaintenance_WhenAssetIsInMaintenance_ShouldSetStatusToAvailable()
    {
        var asset = CreateAvailableAsset();
        asset.SendToMaintenance();
        asset.ClearDomainEvents();
        asset.CompleteMaintenance();

        Assert.Equal(AssetStatus.Available, asset.Status);

        AssetMaintenanceCompletedDomainEvent domainEvent =
        Assert.IsType<AssetMaintenanceCompletedDomainEvent>(
            Assert.Single(asset.DomainEvents));
        Assert.Equal(asset.Id, domainEvent.AssetId);
    }
    [Fact]
    public void CompleteMaintenance_WhenAssetIsAvailable_ShouldThrowInvalidOperationException()
    {
        var asset = CreateAvailableAsset();

        asset.ClearDomainEvents();

        Assert.Throws<InvalidOperationException>(
            () => asset.CompleteMaintenance());

        Assert.Equal(AssetStatus.Available, asset.Status);
        Assert.Empty(asset.DomainEvents);
    }
    [Fact]
    public void AssignToPersonnel_WhenAssetIsAvailable_ShouldAssignAsset()
    {
        var asset = CreateAvailableAsset();
        Guid personnelId = Guid.NewGuid();
        asset.ClearDomainEvents();

        asset.AssignToPersonnel(personnelId);

        Assert.Equal(AssetStatus.Assigned, asset.Status);
        Assert.Equal(personnelId, asset.AssignedPersonnelId);
        AssetAssignedToPersonnelDomainEvent domainEvent = Assert.IsType<AssetAssignedToPersonnelDomainEvent>
            (Assert.Single(asset.DomainEvents));
        Assert.Equal(asset.Id, domainEvent.AssetId);
        Assert.Equal(personalId, domainEvent.PersonnelId);
    }
    [Fact]
    public void AssignToPersonnel_WithEmptyPersonnelId_ShouldThrowArgumentException()
    {
        var asset = CreateAvailableAsset();
        
        asset.ClearDomainEvents();

        asset.AssignToPersonnel(personnelId);
        ArgumentException exception = Assert.Throws<ArgumentException>(
        () => asset.AssignToPersonnel(Guid.Empty));

        Assert.Equal("personnelId", exception.ParamName);
        Assert.Equal(AssetStatus.Available, asset.Status);
        Assert.Null(asset.AssignedPersonnelId);
        Assert.Empty(asset.DomainEvents);
    }
    [Fact]
    public void AssignToPersonnel_WhenAssetIsAlreadyAssigned_ShouldThrowInvalidOperationException()
    {
        var asset = CreateAvailableAsset();
        Guid firstPersonnelId = Guid.NewGuid();
        Guid secondPersonnelId = Guid.NewGuid();
        asset.AssignToPersonnel(firstPersonnelId);
        asset.ClearDomainEvents();

        Assert.Throws<InvalidOperationException>(
        () => asset.AssignToPersonnel(secondPersonnelId));
        Assert.Equal(AssetStatus.Assigned, asset.Status);
        Assert.Equal(firstPersonnelId, asset.AssignedPersonnelId);
        Assert.Empty(asset.DomainEvents);

    }
    [Fact]
    public void ReturnFromPersonnel_WhenAssetIsAssigned_ShouldMakeAssetAvailable()
    {
        var asset = CreateAvailableAsset();
        Guid personnelId = Guid.NewGuid();

        asset.AssignToPersonnel(personnelId);
        asset.ClearDomainEvents();

        asset.ReturnFromPersonnel();

        Assert.Equal(AssetStatus.Available, asset.Status);
        Assert.Null(asset.AssignedPersonnelId);

        AssetReturnedFromPersonnelDomainEvent domainEvent =
            Assert.IsType<AssetReturnedFromPersonnelDomainEvent>(
                Assert.Single(asset.DomainEvents));

        Assert.Equal(asset.Id, domainEvent.AssetId);
        Assert.Equal(personnelId, domainEvent.PersonnelId);
    }
    [Fact]
    public void ReturnFromPersonnel_WhenAssetIsAvailable_ShouldThrowInvalidOperationException()
    {
        var asset = CreateAvailableAsset();

        asset.ClearDomainEvents();

        Assert.Throws<InvalidOperationException>(
            () => asset.ReturnFromPersonnel());

        Assert.Equal(AssetStatus.Available, asset.Status);
        Assert.Null(asset.AssignedPersonnelId);
        Assert.Empty(asset.DomainEvents);
    }

}

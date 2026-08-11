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
        Assert.Null(asset.AssignedPersonnelId);
        Assert.Null(asset.DisposalReason);
    }

    [Fact]
    public void Register_WithEmptyAssetTag_ShouldThrowArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => Asset.Register(
                    Guid.NewGuid(),
                    "   ",
                    "Dell Latitude 7440",
                    "SN-123456"));

        Assert.Equal(
            "assetTag",
            exception.ParamName);
    }

    [Fact]
    public void Register_WithEmptyName_ShouldThrowArgumentException()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => Asset.Register(
                    Guid.NewGuid(),
                    "AST-0001",
                    "   ",
                    "SN-123456"));

        Assert.Equal(
            "name",
            exception.ParamName);
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
        Assert.Equal(
            "Dell Latitude 7440",
            asset.Name);

        Assert.Equal(
            "SN-123456",
            asset.SerialNumber);
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

        Assert.Equal(
            id,
            domainEvent.AssetId);
    }

    [Fact]
    public void SendToMaintenance_WhenAssetIsAvailable_ShouldSetStatusToMaintenance()
    {
        var asset = CreateAvailableAsset();

        asset.ClearDomainEvents();

        asset.SendToMaintenance();

        Assert.Equal(
            AssetStatus.Maintenance,
            asset.Status);

        AssetSentToMaintenanceDomainEvent domainEvent =
            Assert.IsType<AssetSentToMaintenanceDomainEvent>(
                Assert.Single(asset.DomainEvents));

        Assert.Equal(
            asset.Id,
            domainEvent.AssetId);
    }

    [Fact]
    public void SendToMaintenance_WhenAssetIsAlreadyInMaintenance_ShouldThrowInvalidOperationException()
    {
        var asset = CreateAvailableAsset();

        asset.SendToMaintenance();
        asset.ClearDomainEvents();

        Assert.Throws<InvalidOperationException>(
            () => asset.SendToMaintenance());

        Assert.Equal(
            AssetStatus.Maintenance,
            asset.Status);

        Assert.Empty(asset.DomainEvents);
    }

    [Fact]
    public void CompleteMaintenance_WhenAssetIsInMaintenance_ShouldSetStatusToAvailable()
    {
        var asset = CreateAvailableAsset();

        asset.SendToMaintenance();
        asset.ClearDomainEvents();

        asset.CompleteMaintenance();

        Assert.Equal(
            AssetStatus.Available,
            asset.Status);

        AssetMaintenanceCompletedDomainEvent domainEvent =
            Assert.IsType<AssetMaintenanceCompletedDomainEvent>(
                Assert.Single(asset.DomainEvents));

        Assert.Equal(
            asset.Id,
            domainEvent.AssetId);
    }

    [Fact]
    public void CompleteMaintenance_WhenAssetIsAvailable_ShouldThrowInvalidOperationException()
    {
        var asset = CreateAvailableAsset();

        asset.ClearDomainEvents();

        Assert.Throws<InvalidOperationException>(
            () => asset.CompleteMaintenance());

        Assert.Equal(
            AssetStatus.Available,
            asset.Status);

        Assert.Empty(asset.DomainEvents);
    }

    [Fact]
    public void AssignToPersonnel_WhenAssetIsAvailable_ShouldAssignAsset()
    {
        var asset = CreateAvailableAsset();

        Guid personnelId = Guid.NewGuid();

        asset.ClearDomainEvents();

        asset.AssignToPersonnel(personnelId);

        Assert.Equal(
            AssetStatus.Assigned,
            asset.Status);

        Assert.Equal(
            personnelId,
            asset.AssignedPersonnelId);

        AssetAssignedToPersonnelDomainEvent domainEvent =
            Assert.IsType<AssetAssignedToPersonnelDomainEvent>(
                Assert.Single(asset.DomainEvents));

        Assert.Equal(
            asset.Id,
            domainEvent.AssetId);

        Assert.Equal(
            personnelId,
            domainEvent.PersonnelId);
    }

    [Fact]
    public void AssignToPersonnel_WithEmptyPersonnelId_ShouldThrowArgumentException()
    {
        var asset = CreateAvailableAsset();

        asset.ClearDomainEvents();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => asset.AssignToPersonnel(Guid.Empty));

        Assert.Equal(
            "personnelId",
            exception.ParamName);

        Assert.Equal(
            AssetStatus.Available,
            asset.Status);

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
            () => asset.AssignToPersonnel(
                secondPersonnelId));

        Assert.Equal(
            AssetStatus.Assigned,
            asset.Status);

        Assert.Equal(
            firstPersonnelId,
            asset.AssignedPersonnelId);

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

        Assert.Equal(
            AssetStatus.Available,
            asset.Status);

        Assert.Null(asset.AssignedPersonnelId);

        AssetReturnedFromPersonnelDomainEvent domainEvent =
            Assert.IsType<AssetReturnedFromPersonnelDomainEvent>(
                Assert.Single(asset.DomainEvents));

        Assert.Equal(
            asset.Id,
            domainEvent.AssetId);

        Assert.Equal(
            personnelId,
            domainEvent.PersonnelId);
    }

    [Fact]
    public void ReturnFromPersonnel_WhenAssetIsAvailable_ShouldThrowInvalidOperationException()
    {
        var asset = CreateAvailableAsset();

        asset.ClearDomainEvents();

        Assert.Throws<InvalidOperationException>(
            () => asset.ReturnFromPersonnel());

        Assert.Equal(
            AssetStatus.Available,
            asset.Status);

        Assert.Null(asset.AssignedPersonnelId);
        Assert.Empty(asset.DomainEvents);
    }

    [Fact]
    public void MarkAsLost_WhenAssetIsAvailable_ShouldSetStatusToLost()
    {
        var asset = CreateAvailableAsset();

        asset.ClearDomainEvents();

        asset.MarkAsLost();

        Assert.Equal(
            AssetStatus.Lost,
            asset.Status);

        Assert.Null(asset.AssignedPersonnelId);

        AssetMarkedAsLostDomainEvent domainEvent =
            Assert.IsType<AssetMarkedAsLostDomainEvent>(
                Assert.Single(asset.DomainEvents));

        Assert.Equal(
            asset.Id,
            domainEvent.AssetId);

        Assert.Null(
            domainEvent.PreviousPersonnelId);
    }

    [Fact]
    public void MarkAsLost_WhenAssetIsAssigned_ShouldClearPersonnelAssignment()
    {
        var asset = CreateAvailableAsset();

        Guid personnelId = Guid.NewGuid();

        asset.AssignToPersonnel(personnelId);
        asset.ClearDomainEvents();

        asset.MarkAsLost();

        Assert.Equal(
            AssetStatus.Lost,
            asset.Status);

        Assert.Null(asset.AssignedPersonnelId);

        AssetMarkedAsLostDomainEvent domainEvent =
            Assert.IsType<AssetMarkedAsLostDomainEvent>(
                Assert.Single(asset.DomainEvents));

        Assert.Equal(
            asset.Id,
            domainEvent.AssetId);

        Assert.Equal(
            personnelId,
            domainEvent.PreviousPersonnelId);
    }

    [Fact]
    public void MarkAsLost_WhenAssetIsInMaintenance_ShouldThrowInvalidOperationException()
    {
        var asset = CreateAvailableAsset();

        asset.SendToMaintenance();
        asset.ClearDomainEvents();

        Assert.Throws<InvalidOperationException>(
            () => asset.MarkAsLost());

        Assert.Equal(
            AssetStatus.Maintenance,
            asset.Status);

        Assert.Empty(asset.DomainEvents);
    }

    [Fact]
    public void MarkAsLost_WhenAssetIsAlreadyLost_ShouldThrowInvalidOperationException()
    {
        var asset = CreateAvailableAsset();

        asset.MarkAsLost();
        asset.ClearDomainEvents();

        Assert.Throws<InvalidOperationException>(
            () => asset.MarkAsLost());

        Assert.Equal(
            AssetStatus.Lost,
            asset.Status);

        Assert.Empty(asset.DomainEvents);
    }

    [Fact]
    public void RecoverFromLost_WhenAssetIsLost_ShouldSetStatusToAvailable()
    {
        var asset = CreateAvailableAsset();

        asset.MarkAsLost();
        asset.ClearDomainEvents();

        asset.RecoverFromLost();

        Assert.Equal(
            AssetStatus.Available,
            asset.Status);

        Assert.Null(asset.AssignedPersonnelId);

        AssetRecoveredFromLostDomainEvent domainEvent =
            Assert.IsType<AssetRecoveredFromLostDomainEvent>(
                Assert.Single(asset.DomainEvents));

        Assert.Equal(
            asset.Id,
            domainEvent.AssetId);
    }

    [Fact]
    public void RecoverFromLost_WhenAssetIsAvailable_ShouldThrowInvalidOperationException()
    {
        var asset = CreateAvailableAsset();

        asset.ClearDomainEvents();

        Assert.Throws<InvalidOperationException>(
            () => asset.RecoverFromLost());

        Assert.Equal(
            AssetStatus.Available,
            asset.Status);

        Assert.Empty(asset.DomainEvents);
    }

    [Fact]
    public void Dispose_WhenAssetIsAvailable_ShouldSetStatusToDisposed()
    {
        var asset = CreateAvailableAsset();

        asset.ClearDomainEvents();

        asset.Dispose(
            "Device reached the end of its useful life.");

        Assert.Equal(
            AssetStatus.Disposed,
            asset.Status);

        Assert.Equal(
            "Device reached the end of its useful life.",
            asset.DisposalReason);

        Assert.Null(asset.AssignedPersonnelId);

        AssetDisposedDomainEvent domainEvent =
            Assert.IsType<AssetDisposedDomainEvent>(
                Assert.Single(asset.DomainEvents));

        Assert.Equal(
            asset.Id,
            domainEvent.AssetId);

        Assert.Equal(
            AssetStatus.Available,
            domainEvent.PreviousStatus);

        Assert.Equal(
            "Device reached the end of its useful life.",
            domainEvent.Reason);
    }

    [Fact]
    public void Dispose_WhenAssetIsInMaintenance_ShouldSetStatusToDisposed()
    {
        var asset = CreateAvailableAsset();

        asset.SendToMaintenance();
        asset.ClearDomainEvents();

        asset.Dispose(
            "Device cannot be repaired.");

        Assert.Equal(
            AssetStatus.Disposed,
            asset.Status);

        Assert.Equal(
            "Device cannot be repaired.",
            asset.DisposalReason);

        AssetDisposedDomainEvent domainEvent =
            Assert.IsType<AssetDisposedDomainEvent>(
                Assert.Single(asset.DomainEvents));

        Assert.Equal(
            AssetStatus.Maintenance,
            domainEvent.PreviousStatus);
    }

    [Fact]
    public void Dispose_WithEmptyReason_ShouldThrowArgumentException()
    {
        var asset = CreateAvailableAsset();

        asset.ClearDomainEvents();

        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => asset.Dispose("   "));

        Assert.Equal(
            "reason",
            exception.ParamName);

        Assert.Equal(
            AssetStatus.Available,
            asset.Status);

        Assert.Null(asset.DisposalReason);
        Assert.Empty(asset.DomainEvents);
    }

    [Fact]
    public void Dispose_WhenAssetIsAssigned_ShouldThrowInvalidOperationException()
    {
        var asset = CreateAvailableAsset();

        Guid personnelId = Guid.NewGuid();

        asset.AssignToPersonnel(personnelId);
        asset.ClearDomainEvents();

        Assert.Throws<InvalidOperationException>(
            () => asset.Dispose(
                "Device reached the end of its useful life."));

        Assert.Equal(
            AssetStatus.Assigned,
            asset.Status);

        Assert.Equal(
            personnelId,
            asset.AssignedPersonnelId);

        Assert.Null(asset.DisposalReason);
        Assert.Empty(asset.DomainEvents);
    }

    [Fact]
    public void Dispose_WhenAssetIsLost_ShouldThrowInvalidOperationException()
    {
        var asset = CreateAvailableAsset();

        asset.MarkAsLost();
        asset.ClearDomainEvents();

        Assert.Throws<InvalidOperationException>(
            () => asset.Dispose(
                "Device is no longer needed."));

        Assert.Equal(
            AssetStatus.Lost,
            asset.Status);

        Assert.Null(asset.DisposalReason);
        Assert.Empty(asset.DomainEvents);
    }

    [Fact]
    public void Dispose_WhenAssetIsAlreadyDisposed_ShouldThrowInvalidOperationException()
    {
        var asset = CreateAvailableAsset();

        asset.Dispose(
            "Device reached the end of its useful life.");

        asset.ClearDomainEvents();

        Assert.Throws<InvalidOperationException>(
            () => asset.Dispose(
                "Another reason."));

        Assert.Equal(
            AssetStatus.Disposed,
            asset.Status);

        Assert.Equal(
            "Device reached the end of its useful life.",
            asset.DisposalReason);

        Assert.Empty(asset.DomainEvents);
    }

    private static Asset CreateAvailableAsset()
    {
        return Asset.Register(
            Guid.NewGuid(),
            "AST-0001",
            "Dell Latitude 7440",
            "SN-123456");
    }
}

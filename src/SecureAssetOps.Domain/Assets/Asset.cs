using SecureAssetOps.Domain.Assets.Events;
using SecureAssetOps.Domain.Common;

namespace SecureAssetOps.Domain.Assets;

public sealed class Asset : AggregateRoot
{
    public string AssetTag { get; private set; }

    public string Name { get; private set; }

    public string? SerialNumber { get; private set; }

    public AssetStatus Status { get; private set; }
    public Guid? AssignedPersonnelId { get; private set; } 

    private Asset(
        Guid id,
        string assetTag,
        string name,
        string? serialNumber)
        : base(id)
    {
        AssetTag = assetTag;
        Name = name;
        SerialNumber = serialNumber;
        Status = AssetStatus.Available;
        AssignedPersonnelId = null;
    }

    public static Asset Register(
        Guid id,
        string assetTag,
        string name,
        string? serialNumber)
    {
        if (string.IsNullOrWhiteSpace(assetTag))
        {
            throw new ArgumentException(
                "Asset tag cannot be empty.",
                nameof(assetTag));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Asset name cannot be empty.",
                nameof(name));
        }

        string normalizedAssetTag = assetTag.Trim();
        string normalizedName = name.Trim();

        string? normalizedSerialNumber =
            string.IsNullOrWhiteSpace(serialNumber)
                ? null
                : serialNumber.Trim();

        var asset = new Asset(
            id,
            normalizedAssetTag,
            normalizedName,
            normalizedSerialNumber);

        asset.RaiseDomainEvent(
            new AssetRegisteredDomainEvent(asset.Id));

        return asset;
    }
    public void SendToMaintenance()
    {
        if (Status != AssetStatus.Available)
        {
            throw new InvalidOperationException(
                $"Asset in '{Status}' status cannot be sent to maintenance.");
        }

        Status = AssetStatus.Maintenance;

        RaiseDomainEvent(
            new AssetSentToMaintenanceDomainEvent(Id));
    }

    public void CompleteMaintenance()
    {
        if (Status != AssetStatus.Maintenance)
        {
            throw new InvalidOperationException(
                $"Maintenance cannot be completed for an asset in '{Status}' status.");
        }

        Status = AssetStatus.Available;

        RaiseDomainEvent(
            new AssetMaintenanceCompletedDomainEvent(Id));
    }
    public void AssignToPersonnel(Guid personnelId)
    {
        if (personnelId == Guid.Empty)
        {
            throw new ArgumentException(
                "Personnel ID cannot be empty.",
                nameof(personnelId));
        }

        if (Status != AssetStatus.Available)
        {
            throw new InvalidOperationException(
                $"Asset in '{Status}' status cannot be assigned.");
        }

        AssignedPersonnelId = personnelId;
        Status = AssetStatus.Assigned;

        RaiseDomainEvent(
            new AssetAssignedToPersonnelDomainEvent(
                Id,
                personnelId));
    }
    public void ReturnFromPersonnel()
    {
        if (Status != AssetStatus.Assigned)
        {
            throw new InvalidOperationException(
                $"Asset in '{Status}' status cannot be returned from personnel.");
        }

        Guid personnelId = AssignedPersonnelId
            ?? throw new InvalidOperationException(
                "Assigned asset does not have a personnel ID.");

        AssignedPersonnelId = null;
        Status = AssetStatus.Available;

        RaiseDomainEvent(
            new AssetReturnedFromPersonnelDomainEvent(
                Id,
                personnelId));
    }
}

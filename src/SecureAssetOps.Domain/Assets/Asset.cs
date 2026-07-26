using SecureAssetOps.Domain.Assets.Events;
using SecureAssetOps.Domain.Common;

namespace SecureAssetOps.Domain.Assets;

public sealed class Asset : AggregateRoot
{
    public string AssetTag { get; private set; }

    public string Name { get; private set; }

    public string? SerialNumber { get; private set; }

    public AssetStatus Status { get; private set; }

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
}

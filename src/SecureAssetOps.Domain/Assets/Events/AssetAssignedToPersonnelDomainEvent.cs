using SecureAssetOps.Domain.Common;

namespace SecureAssetOps.Domain.Assets.Events;

public sealed record AssetAssignedToPersonnelDomainEvent(
    Guid AssetId,
    Guid PersonnelId)
    : IDomainEvent;

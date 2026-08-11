using SecureAssetOps.Domain.Common;

namespace SecureAssetOps.Domain.Assets.Events;

public sealed record AssetMarkedAsLostDomainEvent(
    Guid AssetId,
    Guid? PreviousPersonnelId)
    : IDomainEvent;

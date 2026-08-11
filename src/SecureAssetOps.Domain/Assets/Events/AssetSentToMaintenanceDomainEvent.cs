using SecureAssetOps.Domain.Common;

namespace SecureAssetOps.Domain.Assets.Events;

public sealed record AssetSentToMaintenanceDomainEvent(Guid AssetId)
    : IDomainEvent;

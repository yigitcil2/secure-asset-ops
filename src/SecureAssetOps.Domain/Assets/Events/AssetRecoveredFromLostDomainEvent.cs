using SecureAssetOps.Domain.Common;

namespace SecureAssetOps.Domain.Assets.Events;

public sealed record AssetRecoveredFromLostDomainEvent(Guid AssetId)
    : IDomainEvent;

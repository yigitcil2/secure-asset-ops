using SecureAssetOps.Domain.Common;

namespace SecureAssetOps.Domain.Assets.Events;

public sealed record AssetRegisteredDomainEvent(Guid AssetId)
    : IDomainEvent;

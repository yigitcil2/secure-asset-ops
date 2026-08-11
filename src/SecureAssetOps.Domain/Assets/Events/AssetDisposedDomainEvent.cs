using SecureAssetOps.Domain.Assets;
using SecureAssetOps.Domain.Common;

namespace SecureAssetOps.Domain.Assets.Events;

public sealed record AssetDisposedDomainEvent(
    Guid AssetId,
    AssetStatus PreviousStatus,
    string Reason)
    : IDomainEvent;

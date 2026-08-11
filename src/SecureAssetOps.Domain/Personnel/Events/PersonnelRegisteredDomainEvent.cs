using SecureAssetOps.Domain.Common;

namespace SecureAssetOps.Domain.Personnel.Events;

public sealed record PersonnelRegisteredDomainEvent(Guid PersonnelId)
    : IDomainEvent;

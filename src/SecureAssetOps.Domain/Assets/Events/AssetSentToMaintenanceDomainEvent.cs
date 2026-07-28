using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecureAssetOps.Domain.Common;

namespace SecureAssetOps.Domain.Assets.Events
{
    public sealed record AssetSentToMaintenanceDomainEvent(Guid AssetId) : IDomainEvent;
    
}

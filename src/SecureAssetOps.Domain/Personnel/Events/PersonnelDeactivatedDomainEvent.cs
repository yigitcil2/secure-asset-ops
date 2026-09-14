using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecureAssetOps.Domain.Common;

namespace SecureAssetOps.Domain.Personnel.Events
{
    public sealed record PersonnelDeactivatedDomainEvent(Guid PersonnelId) : IDomainEvent;
    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureAssetOps.Domain.Assets
{
    public enum AssetStatus
    {
        Available = 1,
        Assigned = 2,
        Maintenance = 3,
        Lost = 4,
        Disposed = 5
    }
}

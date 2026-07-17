using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureAssetOps.Domain.Common
{
    public abstract class Entity
    {
        public Guid Id { get; private set; }
        protected Entity(Guid id)
        {
            if(id == Guid.Empty)
            {
                throw new ArgumentException(
                    "Entity ID cannot be empty.",
                    nameof(id)
                    );
            }
            Id = id;
        }
        
    }
}

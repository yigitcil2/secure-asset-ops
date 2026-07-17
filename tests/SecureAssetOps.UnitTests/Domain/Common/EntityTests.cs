using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecureAssetOps.Domain.Common;
using Xunit;

namespace SecureAssetOps.UnitTests.Domain.Common
{
    public sealed class EntityTests
    {
        [Fact]
        public void Constructor_WithValidId_ShouldSetId()
        {
            Guid id =  Guid.NewGuid();
            var entity = new TestEntity(id);
            Assert.Equal(id, entity.Id);
        }
        [Fact]
        public void Constructor_WithEmptyId_ShouldThrowArgumentException()
        {
            ArgumentException exception = Assert.Throws<ArgumentException>(
            () => new TestEntity(Guid.Empty));

            Assert.Equal("id", exception.ParamName);
        }
        private sealed class TestEntity : Entity
        {
            public TestEntity(Guid id) : base(id)
            {

            }
        }
    }
}

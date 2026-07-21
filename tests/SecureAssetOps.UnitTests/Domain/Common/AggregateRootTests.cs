using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecureAssetOps.Domain.Common;
using Xunit;

namespace SecureAssetOps.UnitTests.Domain.Common
{
    public sealed class AggregateRootTests
    {
        [Fact]
        public void RaiseDomainEvent_WithValidEvent_ShouldAddEvent()
        {
            var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());
            var domainEvent = new TestDomainEvent();

            aggregateRoot.Raise(domainEvent);

            Assert.Single(aggregateRoot.DomainEvents);
            Assert.Contains(domainEvent, aggregateRoot.DomainEvents);
        }
        [Fact]
        public void RaiseDomainEvent_WithNullEvent_ShouldThrowArgumentNullException()
        {
            var aggregateRoot = new TestAggregateRoot(Guid.NewGuid());

            Assert.Throws<ArgumentNullException>(
            () => aggregateRoot.Raise(null!));
        }
        private sealed class TestAggregateRoot : AggregateRoot
        {
            public TestAggregateRoot(Guid id)
                : base(id)
            {
            }

            public void Raise(IDomainEvent domainEvent)
            {
                RaiseDomainEvent(domainEvent);
            }
        }

        private sealed class TestDomainEvent : IDomainEvent
        {
        }

    }
}

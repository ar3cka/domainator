using Domainator.Entities;
using Xunit;

namespace Domainator.UnitTests.Entities
{
    public class AbstractEntityIdentityTests
    {
        public class Int32TestableEntityIdentity : AbstractEntityIdentity<int>
        {
            public Int32TestableEntityIdentity(int id)
            {
                Id = id;
                Tag = "Int32TestableEntityIdentity";
            }
            
            public override int Id { get; protected set; }
            public override string Tag { get; }
        }
        
        [Fact]
        public void EqualityTests()
        {
            var id1 = new Int32TestableEntityIdentity(2);
            var id2 = new Int32TestableEntityIdentity(2);

            Assert.Equal(id1, id1);
            Assert.True(id1 == id2);
        }
    }
}
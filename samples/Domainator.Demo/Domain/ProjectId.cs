using Domainator.Entities;

namespace Domainator.Demo.Domain.Domain
{
    public sealed class ProjectId : Int32EntityIdentity
    {
        public override string Tag => "project";

        public ProjectId(IEntityIdentity identity) : base(identity)
        {
        }

        public ProjectId(int id) : base(id)
        {
        }
    }
}

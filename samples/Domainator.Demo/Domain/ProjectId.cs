using Domainator.Entities;

namespace Domainator.Demo.Domain.Domain
{
    public sealed class ProjectId : AbstractEntityIdentity<int>
    {
        public override string Tag => "project";

        public ProjectId(int id) : base(id)
        {
        }
    }
}

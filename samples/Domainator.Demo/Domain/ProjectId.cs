using System;
using Domainator.Entities;
using Domainator.Utilities;

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

    public sealed class UserId : GuidEntityIdentity
    {
        public override string Tag => "user";

        public UserId(IEntityIdentity identity) : base(identity)
        {
        }

        public UserId(Guid id) : base(id)
        {
            Require.NotEmpty(id, nameof(id));
        }
    }
}

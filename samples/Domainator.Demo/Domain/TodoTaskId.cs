using Domainator.Entities;

namespace Domainator.Demo.Domain.Domain
{
    public sealed class TodoTaskId : Int32EntityIdentity
    {
        public override string Tag => "todo_task";

        public TodoTaskId(IEntityIdentity identity) : base(identity)
        {
        }

        public TodoTaskId(int id) : base(id)
        {
        }
    }
}

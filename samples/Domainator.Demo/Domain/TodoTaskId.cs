using Domainator.Entities;

namespace Domainator.Demo.Domain.Domain
{
    public sealed class TodoTaskId : AbstractEntityIdentity<int>
    {
        public override string Tag => "todo_task";

        public TodoTaskId(int id) : base(id)
        {
        }
    }
}

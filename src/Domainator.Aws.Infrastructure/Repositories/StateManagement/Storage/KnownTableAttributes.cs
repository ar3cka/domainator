namespace Domainator.Infrastructure.Repositories.StateManagement.Storage
{
    public static class KnownTableAttributes
    {
        public static readonly string PartitionKey = "PK";
        public static readonly string SortKey = "SK";
        public static readonly string Data = "Data";
        public static readonly string Version = "Ver";
        public static readonly string AggregateId = "Id";
        public static readonly string AggregateType = "Tag";
        public static readonly string CreatedAt = "CreatedAt";
        public static readonly string UpdatedAt = "UpdatedAt";
    }
}

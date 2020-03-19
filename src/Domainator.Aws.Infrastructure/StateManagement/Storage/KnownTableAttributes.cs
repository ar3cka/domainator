namespace Domainator.Infrastructure.StateManagement.Storage
{
    public static class KnownTableAttributes
    {
        public static readonly string PartitionKey = "PK";
        public static readonly string SortKey = "SK";
        public static readonly string Data = "Data";
        public static readonly string Version = "Version";
        public static readonly string AggregateType = "AggregateType";
        public static readonly string CreatedAt = "CreatedAt";
        public static readonly string UpdatedAt = "UpdatedAt";
    }
}

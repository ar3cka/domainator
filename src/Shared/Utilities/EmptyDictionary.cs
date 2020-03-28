using System.Collections.Generic;

namespace Domainator.Utilities
{
    internal static class EmptyDictionary<TKey, TValue>
    {
        public static readonly IReadOnlyDictionary<TKey, TValue> Instance = new Dictionary<TKey, TValue>(0);
    }
}

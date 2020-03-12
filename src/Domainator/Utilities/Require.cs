using System;

namespace Domainator.Utilities
{
    [AttributeUsage(AttributeTargets.Parameter)]
    internal sealed class ValidatedNotNullAttribute : Attribute
    {
    }

    public static class Require
    {
        public static void NotNull<T>([ValidatedNotNullAttribute] T paramValue, string paramName) where T : class
        {
            if (paramValue == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void ZeroOrGreater(int value, string param)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(param, value, "Value must be zero or greater.");
            }
        }

        public static void NotEmpty(string paramValue, string paramName)
        {
            False(string.IsNullOrEmpty(paramValue), paramName, "Value must be not empty.");
        }

        public static void False(bool condition, string paramName, string message)
        {
            if (condition)
            {
                throw new ArgumentException(message, paramName);
            }
        }
    }
}

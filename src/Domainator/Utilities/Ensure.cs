using System;

namespace Domainator.Utilities
{
    public static class Ensure
    {
        public static void True(bool condition, string error)
        {
            True<InvalidOperationException>(condition, error);
        }

        public static void True<TException>(bool condition, string error)
            where TException : Exception
        {
            Require.NotEmpty(error, "error");

            if (condition)
            {
                return;
            }

            throw (TException)Activator.CreateInstance(typeof(TException), error);
        }
    }
}

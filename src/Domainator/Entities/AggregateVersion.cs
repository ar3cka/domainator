using System;
using System.Globalization;
using Domainator.Utilities;

namespace Domainator.Entities
{
    /// <summary>
    /// The version of the aggregate.
    /// </summary>
    public struct AggregateVersion : IEquatable<AggregateVersion>, IComparable<AggregateVersion>
    {
        private readonly int _value;

        public static readonly AggregateVersion Emtpy = new AggregateVersion(0);

        private AggregateVersion(int value)
        {
            _value = value;
        }

        public static bool IsEmpty(AggregateVersion version)
        {
            return version._value == 0;
        }

        public static AggregateVersion Create(int version)
        {
            Require.ZeroOrGreater(version, "value");

            return version == 0 ? Emtpy : new AggregateVersion(version);
        }

        public static AggregateVersion Parse(string version)
        {
            Require.NotEmpty(version, "version");

            return new AggregateVersion(int.Parse(version, CultureInfo.InvariantCulture));
        }

        public AggregateVersion Increment(int incrementValue)
        {
            Require.ZeroOrGreater(incrementValue, "incrementValue");

            var incrementedValue = _value + incrementValue;

            return incrementedValue == 0 ? Emtpy : Create(incrementedValue);
        }

        public AggregateVersion Increment()
        {
            return Increment(1);
        }

        public int CompareTo(AggregateVersion other)
        {
            return _value.CompareTo(other._value);
        }

        public bool Equals(AggregateVersion other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is AggregateVersion && Equals((AggregateVersion) obj);
        }

        public override int GetHashCode()
        {
            return _value;
        }

        public override string ToString()
        {
            return _value.ToString("0000000000", CultureInfo.InvariantCulture);
        }

        public static bool operator ==(AggregateVersion left, AggregateVersion right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AggregateVersion left, AggregateVersion right)
        {
            return !left.Equals(right);
        }

        public static bool operator >=(AggregateVersion left, AggregateVersion right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator <=(AggregateVersion left, AggregateVersion right)
        {
            return left.CompareTo(right) <= 0;
        }

        public static bool operator >(AggregateVersion left, AggregateVersion right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <(AggregateVersion left, AggregateVersion right)
        {
            return left.CompareTo(right) < 0;
        }

        public static explicit operator int(AggregateVersion version)
        {
            return version._value;
        }
    }
}

using System;

namespace AbilitySystem.Core.Abilities
{
    /// <summary>
    /// Value Object for ability identification
    /// </summary>
    public readonly struct AbilityId : IEquatable<AbilityId>
    {
        public string Value { get; }

        public AbilityId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Ability ID cannot be null or empty", nameof(value));
            }
            Value = value;
        }

        public bool Equals(AbilityId other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is AbilityId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return Value;
        }

        public static bool operator ==(AbilityId left, AbilityId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AbilityId left, AbilityId right)
        {
            return !left.Equals(right);
        }

        public static implicit operator string(AbilityId id)
        {
            return id.Value;
        }

        public static explicit operator AbilityId(string value)
        {
            return new AbilityId(value);
        }
    }
}



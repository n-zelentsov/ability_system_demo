using AbilitySystem.Core.Modifiers;

namespace AbilitySystem.Gameplay.Services.Effects
{
    /// <summary>
    /// Concrete stat modifier implementation
    /// </summary>
    public sealed class StatModifierAbility : IModifier
    {
        public string Id { get; }
        public string SourceId { get; }
        public string TargetStatId { get; }
        public DamageModifierType Type { get; }
        public float Value { get; }
        public int Priority { get; }
        public bool IsExpired => false; // Managed by effect

        public StatModifierAbility(
            string id,
            string sourceId,
            string targetStatId,
            DamageModifierType type,
            float value,
            int priority = 0)
        {
            Id = id;
            SourceId = sourceId;
            TargetStatId = targetStatId;
            Type = type;
            Value = value;
            Priority = priority;
        }

        public float Apply(float baseValue, float currentValue)
        {
            return Type switch
            {
                DamageModifierType.Flat => currentValue + Value,
                DamageModifierType.PercentAdd => currentValue + (baseValue * Value),
                DamageModifierType.PercentMultiply => currentValue * (1 + Value),
                DamageModifierType.Override => Value,
                _ => currentValue
            };
        }
    }
}
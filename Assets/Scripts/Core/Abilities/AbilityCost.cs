using AbilitySystem.Core.Events;

namespace AbilitySystem.Core.Abilities
{
    public readonly struct AbilityCost
    {
        public AbilityCostType Type { get; }
        public float Amount { get; }

        public AbilityCost(AbilityCostType type, float amount)
        {
            Type = type;
            Amount = amount;
        }
    }
}
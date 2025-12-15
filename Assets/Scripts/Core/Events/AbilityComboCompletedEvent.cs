using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Combos;

namespace AbilitySystem.Core.Events
{
    public readonly struct AbilityComboCompletedEvent : IEvent
    {
        public IAbilityOwner Caster { get; }
        public ComboDefinition Combo { get; }
        public float TotalDamageMultiplier { get; }

        public AbilityComboCompletedEvent(IAbilityOwner caster, ComboDefinition combo, float totalMultiplier)
        {
            Caster = caster;
            Combo = combo;
            TotalDamageMultiplier = totalMultiplier;
        }
    }
}
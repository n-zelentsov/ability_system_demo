using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Combos;

namespace AbilitySystem.Core.Events
{
    public readonly struct AbilityComboDroppedEvent : IEvent
    {
        public IAbilityOwner Caster { get; }
        public ComboDefinition Combo { get; }
        public string Reason { get; }

        public AbilityComboDroppedEvent(IAbilityOwner caster, ComboDefinition combo, string reason)
        {
            Caster = caster;
            Combo = combo;
            Reason = reason;
        }
    }
}
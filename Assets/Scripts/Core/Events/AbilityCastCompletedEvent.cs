using AbilitySystem.Core.Abilities;

namespace AbilitySystem.Core.Events
{
    public readonly struct AbilityCastCompletedEvent : IEvent
    {
        public IAbilityOwner Caster { get; }
        public AbilityId AbilityId { get; }

        public AbilityCastCompletedEvent(IAbilityOwner caster, AbilityId abilityId)
        {
            Caster = caster;
            AbilityId = abilityId;
        }
    }
}
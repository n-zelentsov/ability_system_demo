using AbilitySystem.Core.Abilities;

namespace AbilitySystem.Core.Events
{
    public readonly struct AbilityCastStartedEvent : IEvent
    {
        public IAbilityOwner Caster { get; }
        public AbilityId AbilityId { get; }

        public AbilityCastStartedEvent(IAbilityOwner caster, AbilityId abilityId)
        {
            Caster = caster;
            AbilityId = abilityId;
        }
    }
}
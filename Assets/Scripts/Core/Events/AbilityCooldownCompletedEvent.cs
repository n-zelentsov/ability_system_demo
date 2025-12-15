using AbilitySystem.Core.Abilities;

namespace AbilitySystem.Core.Events
{
    public readonly struct AbilityCooldownCompletedEvent : IEvent
    {
        public IAbilityOwner Owner { get; }
        public AbilityId AbilityId { get; }

        public AbilityCooldownCompletedEvent(IAbilityOwner owner, AbilityId abilityId)
        {
            Owner = owner;
            AbilityId = abilityId;
        }
    }
}


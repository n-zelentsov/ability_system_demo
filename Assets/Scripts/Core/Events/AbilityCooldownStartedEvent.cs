using AbilitySystem.Core.Abilities;

namespace AbilitySystem.Core.Events
{
    public readonly struct AbilityCooldownStartedEvent : IEvent
    {
        public IAbilityOwner Owner { get; }
        public AbilityId AbilityId { get; }
        public float Duration { get; }

        public AbilityCooldownStartedEvent(IAbilityOwner owner, AbilityId abilityId, float duration)
        {
            Owner = owner;
            AbilityId = abilityId;
            Duration = duration;
        }
    }
}
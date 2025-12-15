using AbilitySystem.Core.Abilities;

namespace AbilitySystem.Core.Events
{
    public readonly struct AbilityCastCancelledEvent : IEvent
    {
        public IAbilityOwner Caster { get; }
        public AbilityId AbilityId { get; }
        public string Reason { get; }

        public AbilityCastCancelledEvent(IAbilityOwner caster, AbilityId abilityId, string reason)
        {
            Caster = caster;
            AbilityId = abilityId;
            Reason = reason;
        }
    }
}
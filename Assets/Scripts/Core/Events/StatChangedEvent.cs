using AbilitySystem.Core.Abilities;

namespace AbilitySystem.Core.Events
{
    public readonly struct StatChangedEvent : IEvent
    {
        public IAbilityOwner Owner { get; }
        public string StatId { get; }
        public float OldValue { get; }
        public float NewValue { get; }

        public StatChangedEvent(IAbilityOwner owner, string statId, float oldValue, float newValue)
        {
            Owner = owner;
            StatId = statId;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
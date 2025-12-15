using AbilitySystem.Core.Abilities;

namespace AbilitySystem.Core.Events
{
    public readonly struct HealingDealtEvent : IEvent
    {
        public IAbilityOwner Source { get; }
        public IEffectTarget Target { get; }
        public float Amount { get; }
        public bool IsCritical { get; }

        public HealingDealtEvent(IAbilityOwner source, IEffectTarget target, float amount, bool isCritical)
        {
            Source = source;
            Target = target;
            Amount = amount;
            IsCritical = isCritical;
        }
    }
}
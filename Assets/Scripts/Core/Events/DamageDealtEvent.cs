using AbilitySystem.Core.Abilities;

namespace AbilitySystem.Core.Events
{
    public readonly struct DamageDealtEvent : IEvent
    {
        public IAbilityOwner Source { get; }
        public IEffectTarget Target { get; }
        public float Amount { get; }
        public DamageType DamageType { get; }
        public bool IsCritical { get; }

        public DamageDealtEvent(IAbilityOwner source, IEffectTarget target, float amount, 
            DamageType damageType, bool isCritical)
        {
            Source = source;
            Target = target;
            Amount = amount;
            DamageType = damageType;
            IsCritical = isCritical;
        }
    }
}
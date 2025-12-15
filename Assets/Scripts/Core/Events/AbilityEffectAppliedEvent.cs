using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Effects;

namespace AbilitySystem.Core.Events
{
    public readonly struct AbilityEffectAppliedEvent : IEvent
    {
        public IEffectTarget Target { get; }
        public IAbilityEffect AbilityEffect { get; }
        public AbilityEffectResult Result { get; }

        public AbilityEffectAppliedEvent(IEffectTarget target, IAbilityEffect abilityEffect, AbilityEffectResult result)
        {
            Target = target;
            AbilityEffect = abilityEffect;
            Result = result;
        }
    }
}
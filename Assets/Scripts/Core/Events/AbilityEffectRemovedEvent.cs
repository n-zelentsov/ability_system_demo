using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Effects;

namespace AbilitySystem.Core.Events
{
    public readonly struct AbilityEffectRemovedEvent : IEvent
    {
        public IEffectTarget Target { get; }
        public IAbilityEffect AbilityEffect { get; }

        public AbilityEffectRemovedEvent(IEffectTarget target, IAbilityEffect abilityEffect)
        {
            Target = target;
            AbilityEffect = abilityEffect;
        }
    }
}
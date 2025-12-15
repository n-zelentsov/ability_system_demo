using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Combos;

namespace AbilitySystem.Core.Events
{
    public readonly struct AbilityElementalReactionEvent : IEvent
    {
        public IEffectTarget Target { get; }
        public IAbilityOwner Source { get; }
        public ElementalReactionType ReactionType { get; }
        public DamageType Element1 { get; }
        public DamageType Element2 { get; }
        public float BonusDamage { get; }

        public AbilityElementalReactionEvent(
            IEffectTarget target, 
            IAbilityOwner source,
            ElementalReactionType reactionType, 
            DamageType element1, 
            DamageType element2,
            float bonusDamage)
        {
            Target = target;
            Source = source;
            ReactionType = reactionType;
            Element1 = element1;
            Element2 = element2;
            BonusDamage = bonusDamage;
        }
    }
}


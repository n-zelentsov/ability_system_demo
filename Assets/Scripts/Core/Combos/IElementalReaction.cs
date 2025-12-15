using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Effects;
using AbilitySystem.Core.Events;

namespace AbilitySystem.Core.Combos
{
    /// <summary>
    /// Interface for elemental reaction system
    /// </summary>
    public interface IElementalReactionSystem
    {
        ElementalReaction CheckReaction(IEffectTarget target, DamageType incomingElement);
        void ApplyElement(IEffectTarget target, DamageType element, float duration);
        DamageType? GetActiveElement(IEffectTarget target);
        void Tick(float deltaTime);
    }
}
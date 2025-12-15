using System.Collections.Generic;
using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Effects;

namespace AbilitySystem.Core.Runtime
{
    /// <summary>
    /// Interface for processing and managing active effects
    /// </summary>
    public interface IEffectProcessor
    {
        AbilityEffectResult ApplyEffect(IAbilityEffect abilityEffect, AbilityEffectContext context);
        void ApplyDurationEffect(IAbilityDurationAbilityEffect abilityEffect, IEffectTarget target, AbilityEffectContext context);
        void RemoveEffect(IAbilityDurationAbilityEffect abilityEffect, IEffectTarget target);
        IReadOnlyList<IAbilityDurationAbilityEffect> GetActiveEffects(IEffectTarget target);
        void Tick(float deltaTime);
        void ClearEffects(IEffectTarget target);
    }
}


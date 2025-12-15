using System;
using MessagePipe;
using AbilitySystem.Core.Combos;
using AbilitySystem.Core.Effects;
using AbilitySystem.Core.Events;
using AbilitySystem.Gameplay.Services.Effects;
using AbilitySystem.Unity.Data;

namespace AbilitySystem.Unity.Factories
{
    /// <summary>
    /// Factory for creating effects using MessagePipe publishers
    /// </summary>
    public sealed class EffectFactory
    {
        private readonly IPublisher<DamageDealtEvent> _damagePublisher;
        private readonly IPublisher<HealingDealtEvent> _healPublisher;
        private readonly IPublisher<AbilityElementalReactionEvent> _reactionPublisher;
        private readonly IElementalReactionSystem _elementalSystem;

        public EffectFactory(
            IPublisher<DamageDealtEvent> damagePublisher,
            IPublisher<HealingDealtEvent> healPublisher,
            IPublisher<AbilityElementalReactionEvent> reactionPublisher,
            IElementalReactionSystem elementalSystem)
        {
            _damagePublisher = damagePublisher;
            _healPublisher = healPublisher;
            _reactionPublisher = reactionPublisher;
            _elementalSystem = elementalSystem;
        }

        public IAbilityEffect CreateEffect(EffectDefinition definition)
        {
            return definition.Category switch
            {
                EffectCategory.Damage => CreateDamageEffect(definition),
                EffectCategory.Heal => CreateHealEffect(definition),
                EffectCategory.DamageOverTime => CreateDotEffect(definition),
                EffectCategory.HealOverTime => CreateHotEffect(definition),
                EffectCategory.Buff or EffectCategory.Debuff => CreateStatModifierEffect(definition),
                _ => throw new ArgumentException($"Unknown effect category: {definition.Category}")
            };
        }

        private IAbilityEffect CreateDamageEffect(EffectDefinition def) =>
            new InstantDamageAbilityEffect(def.EffectId, def.DisplayName, def.BaseDamage, def.DamageType,
                _damagePublisher, _reactionPublisher, _elementalSystem, def.ScalingFactor, def.ScalingStat);

        private IAbilityEffect CreateHealEffect(EffectDefinition def) =>
            new InstantHealAbilityEffect(def.EffectId, def.DisplayName, def.BaseHeal,
                _healPublisher, def.ScalingFactor, def.ScalingStat);

        private IAbilityEffect CreateDotEffect(EffectDefinition def) =>
            new DamageOverTimeAbilityEffect(def.EffectId, def.DisplayName, def.Duration, def.TickInterval,
                def.BaseDamage, def.DamageType, _damagePublisher, def.IsStackable, def.MaxStacks);

        private IAbilityEffect CreateHotEffect(EffectDefinition def) =>
            new HealOverTimeAbilityEffect(def.EffectId, def.DisplayName, def.Duration, def.TickInterval,
                def.BaseHeal, _healPublisher, def.IsStackable, def.MaxStacks);

        private IAbilityEffect CreateStatModifierEffect(EffectDefinition def) =>
            new StatModifierAbilityEffect(def.EffectId, def.DisplayName, def.Duration, def.TargetStatId,
                def.DamageModifierType, def.ModifierValue, def.IsStackable, def.MaxStacks);
    }
}

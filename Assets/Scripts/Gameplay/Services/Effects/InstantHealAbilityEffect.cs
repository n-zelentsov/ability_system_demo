using System;
using MessagePipe;
using AbilitySystem.Core.Effects;
using AbilitySystem.Core.Events;
using AbilitySystem.Gameplay.Stats;

namespace AbilitySystem.Gameplay.Services.Effects
{
    /// <summary>
    /// Effect that instantly heals a target
    /// </summary>
    public sealed class InstantHealAbilityEffect : IAbilityEffect
    {
        private readonly IPublisher<HealingDealtEvent> _healPublisher;
        private readonly Random _random;

        public string Id { get; }
        public string Name { get; }
        public AbilityEffectType Type => AbilityEffectType.Instant;
        public float BaseHeal { get; }
        public float ScalingFactor { get; }
        public string ScalingStat { get; }

        public InstantHealAbilityEffect(
            string id, string name, float baseHeal,
            IPublisher<HealingDealtEvent> healPublisher,
            float scalingFactor = 0f, string scalingStat = null)
        {
            Id = id;
            Name = name;
            BaseHeal = baseHeal;
            _healPublisher = healPublisher;
            ScalingFactor = scalingFactor;
            ScalingStat = scalingStat ?? EntityStatType.SpellPower;
            _random = new Random();
        }

        public bool CanApply(AbilityEffectContext context) => context.Target != null && context.Target.IsAlive;

        public AbilityEffectResult Apply(AbilityEffectContext context)
        {
            if (!CanApply(context))
                return AbilityEffectResult.Failed("Invalid target");

            float healing = CalculateHealing(context);
            
            bool isCritical = CheckCritical(context);
            if (isCritical)
                healing *= 1f + context.Source.GetStat(EntityStatType.CriticalDamage);

            healing *= context.PowerMultiplier;
            context.Target.ModifyStat(EntityStatType.MaxHealth, healing);

            _healPublisher.Publish(new HealingDealtEvent(context.Source, context.Target, healing, isCritical));

            return isCritical ? AbilityEffectResult.Critical(healing) : AbilityEffectResult.Succeeded(healing);
        }

        private float CalculateHealing(AbilityEffectContext context)
        {
            float healing = BaseHeal;
            if (ScalingFactor > 0 && context.Source != null)
                healing += context.Source.GetStat(ScalingStat) * ScalingFactor;
            return healing;
        }

        private bool CheckCritical(AbilityEffectContext context)
        {
            if (context.Source == null) return false;
            return _random.NextDouble() < context.Source.GetStat(EntityStatType.CriticalChance);
        }
    }
}

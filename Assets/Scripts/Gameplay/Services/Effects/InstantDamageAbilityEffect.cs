using System;
using MessagePipe;
using AbilitySystem.Core.Combos;
using AbilitySystem.Core.Effects;
using AbilitySystem.Core.Events;
using AbilitySystem.Gameplay.Stats;

namespace AbilitySystem.Gameplay.Services.Effects
{
    /// <summary>
    /// Effect that deals instant damage with elemental reaction support
    /// </summary>
    public sealed class InstantDamageAbilityEffect : IAbilityEffect
    {
        private readonly IPublisher<DamageDealtEvent> _damagePublisher;
        private readonly IPublisher<AbilityElementalReactionEvent> _reactionPublisher;
        private readonly IElementalReactionSystem _elementalSystem;
        private readonly Random _random;

        public string Id { get; }
        public string Name { get; }
        public AbilityEffectType Type => AbilityEffectType.Instant;
        public float BaseDamage { get; }
        public DamageType DamageType { get; }
        public float ScalingFactor { get; }
        public string ScalingStat { get; }

        public InstantDamageAbilityEffect(
            string id, string name, float baseDamage, DamageType damageType,
            IPublisher<DamageDealtEvent> damagePublisher,
            IPublisher<AbilityElementalReactionEvent> reactionPublisher = null,
            IElementalReactionSystem elementalSystem = null,
            float scalingFactor = 0f, string scalingStat = null)
        {
            Id = id;
            Name = name;
            BaseDamage = baseDamage;
            DamageType = damageType;
            _damagePublisher = damagePublisher;
            _reactionPublisher = reactionPublisher;
            _elementalSystem = elementalSystem;
            ScalingFactor = scalingFactor;
            ScalingStat = scalingStat ?? EntityStatType.SpellPower;
            _random = new Random();
        }

        public bool CanApply(AbilityEffectContext context) => context.Target != null && context.Target.IsAlive;

        public AbilityEffectResult Apply(AbilityEffectContext context)
        {
            if (!CanApply(context))
                return AbilityEffectResult.Failed("Invalid target");

            float damage = CalculateDamage(context);
            
            bool isCritical = CheckCritical(context);
            if (isCritical)
                damage *= 1f + context.Source.GetStat(EntityStatType.CriticalDamage);

            ElementalReaction reaction = null;
            if (_elementalSystem != null)
            {
                reaction = _elementalSystem.CheckReaction(context.Target, DamageType);
                if (reaction.Type != ElementalReactionType.None)
                {
                    damage *= reaction.DamageMultiplier;
                    _reactionPublisher?.Publish(new AbilityElementalReactionEvent(
                        context.Target, context.Source, reaction.Type,
                        _elementalSystem.GetActiveElement(context.Target) ?? DamageType, DamageType,
                        damage * (reaction.DamageMultiplier - 1f)));
                }
                else
                {
                    _elementalSystem.ApplyElement(context.Target, DamageType, 10f);
                }
            }

            damage = ApplyResistance(damage, context);
            damage *= context.PowerMultiplier;

            context.Target.ModifyStat(EntityStatType.MaxHealth, -damage);

            _damagePublisher.Publish(new DamageDealtEvent(context.Source, context.Target, damage, DamageType, isCritical));

            return isCritical ? AbilityEffectResult.Critical(damage) : AbilityEffectResult.Succeeded(damage);
        }

        private float CalculateDamage(AbilityEffectContext context)
        {
            float damage = BaseDamage;
            if (ScalingFactor > 0 && context.Source != null)
                damage += context.Source.GetStat(ScalingStat) * ScalingFactor;
            return damage;
        }

        private bool CheckCritical(AbilityEffectContext context)
        {
            if (context.Source == null) return false;
            return _random.NextDouble() < context.Source.GetStat(EntityStatType.CriticalChance);
        }

        private float ApplyResistance(float damage, AbilityEffectContext context)
        {
            string resistStat = DamageType switch
            {
                DamageType.Physical => EntityStatType.Armor,
                DamageType.Fire => EntityStatType.FireResist,
                DamageType.Ice => EntityStatType.IceResist,
                DamageType.Lightning => EntityStatType.LightningResist,
                DamageType.Arcane or DamageType.Holy or DamageType.Shadow => EntityStatType.MagicResist,
                _ => null
            };
            if (resistStat == null) return damage;
            float resistance = context.Target.GetStat(resistStat);
            return damage * (100f / (100f + resistance));
        }
    }
}

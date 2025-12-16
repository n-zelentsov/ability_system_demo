using System.Collections.Generic;
using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Conditions;
using AbilitySystem.Core.Effects;
using AbilitySystem.Unity.Data;

namespace AbilitySystem.Unity.Factories
{
    /// <summary>
    /// Factory for creating abilities from definitions
    /// </summary>
    public sealed class AbilityFactory
    {
        private readonly EffectFactory _effectFactory;

        public AbilityFactory(EffectFactory effectFactory)
        {
            _effectFactory = effectFactory;
        }

        public IAbility CreateAbility(AbilityDefinition definition)
        {
            var effects = new List<IAbilityEffect>();
            
            if (definition.Effects != null)
            {
                foreach (var effectDef in definition.Effects)
                {
                    if (effectDef != null)
                    {
                        effects.Add(_effectFactory.CreateEffect(effectDef));
                    }
                }
            }

            return new RuntimeAbility(
                new AbilityId(definition.AbilityId),
                definition.DisplayName,
                definition.Description,
                definition.ToAbilityData(),
                definition.TargetingType,
                effects);
        }
    }

    /// <summary>
    /// Runtime ability implementation
    /// </summary>
    internal sealed class RuntimeAbility : IAbility
    {
        public AbilityId Id { get; }
        public string Name { get; }
        public string Description { get; }
        public AbilityData Data { get; }
        public TargetingType TargetingType { get; }
        public IReadOnlyList<IAbilityCondition> Conditions { get; }
        public IReadOnlyList<IAbilityEffect> Effects { get; }

        public RuntimeAbility(
            AbilityId id,
            string name,
            string description,
            AbilityData data,
            TargetingType targetingType,
            IReadOnlyList<IAbilityEffect> effects,
            IReadOnlyList<IAbilityCondition> conditions = null)
        {
            Id = id;
            Name = name;
            Description = description;
            Data = data;
            TargetingType = targetingType;
            Effects = effects;
            Conditions = conditions ?? System.Array.Empty<IAbilityCondition>();
        }
    }
}



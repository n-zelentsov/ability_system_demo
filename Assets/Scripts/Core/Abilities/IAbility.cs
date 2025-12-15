using System.Collections.Generic;
using AbilitySystem.Core.Conditions;
using AbilitySystem.Core.Effects;

namespace AbilitySystem.Core.Abilities
{
    /// <summary>
    /// Core ability interface defining the contract for all abilities
    /// </summary>
    public interface IAbility
    {
        AbilityId Id { get; }
        string Name { get; }
        string Description { get; }
        AbilityData Data { get; }
        
        IReadOnlyList<IAbilityCondition> Conditions { get; }
        IReadOnlyList<IAbilityEffect> Effects { get; }
        
        TargetingType TargetingType { get; }
    }

    public enum TargetingType
    {
        Self,
        SingleTarget,
        PointTarget,
        DirectionalTarget,
        AreaOfEffect,
        NoTarget
    }
}


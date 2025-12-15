using AbilitySystem.Core.Abilities;

namespace AbilitySystem.Core.Effects
{
    /// <summary>
    /// Core interface for all effects that can be applied to targets
    /// </summary>
    public interface IAbilityEffect
    {
        string Id { get; }
        string Name { get; }
        AbilityEffectType Type { get; }
        
        AbilityEffectResult Apply(AbilityEffectContext context);
        bool CanApply(AbilityEffectContext context);
    }
}


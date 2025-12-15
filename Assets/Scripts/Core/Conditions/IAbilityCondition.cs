using AbilitySystem.Core.Abilities;

namespace AbilitySystem.Core.Conditions
{
    /// <summary>
    /// Interface for ability cast conditions
    /// </summary>
    public interface IAbilityCondition
    {
        string Id { get; }
        string FailureMessage { get; }
        
        bool IsMet(IAbilityOwner caster, IAbility ability, AbilityCastContext context);
    }
}


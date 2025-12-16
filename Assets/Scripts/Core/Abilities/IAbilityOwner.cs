using System.Collections.Generic;
using AbilitySystem.Core.Events;

namespace AbilitySystem.Core.Abilities
{
    /// <summary>
    /// Interface for entities that can own and cast abilities
    /// </summary>
    public interface IAbilityOwner : IEffectTarget
    {
        IReadOnlyList<IAbility> Abilities { get; }
        
        bool HasResource(AbilityCostType type, float amount);
        void ConsumeResource(AbilityCostType type, float amount);
        float GetResource(AbilityCostType type);
        float GetMaxResource(AbilityCostType type);
        void ModifyResource(AbilityCostType type, float delta);
        
        void AddAbility(IAbility ability);
        void RemoveAbility(AbilityId abilityId);
        IAbility GetAbility(AbilityId abilityId);
        bool HasAbility(AbilityId abilityId);
    }
}



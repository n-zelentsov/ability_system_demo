using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Conditions;

namespace AbilitySystem.Gameplay.Casting.Conditions
{
    /// <summary>
    /// Condition that checks if caster has enough resources
    /// </summary>
    public sealed class ResourceAbilityCondition : IAbilityCondition
    {
        public string Id => "resource";
        public string FailureMessage { get; private set; } = "Not enough resources";

        public bool IsMet(IAbilityOwner caster, IAbility ability, AbilityCastContext context)
        {
            foreach (AbilityCost cost in ability.Data.ResourceCosts)
            {
                if (!caster.HasResource(cost.Type, cost.Amount))
                {
                    FailureMessage = $"Not enough {cost.Type}";
                    return false;
                }
            }
            return true;
        }
    }
}


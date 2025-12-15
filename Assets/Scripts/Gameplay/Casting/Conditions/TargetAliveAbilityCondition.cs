using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Conditions;

namespace AbilitySystem.Gameplay.Casting.Conditions
{
    /// <summary>
    /// Condition that checks if target is alive (for abilities that require living targets)
    /// </summary>
    public sealed class TargetAliveAbilityCondition : IAbilityCondition
    {
        public string Id => "target_alive";
        public string FailureMessage => "Target is dead";

        public bool IsMet(IAbilityOwner caster, IAbility ability, AbilityCastContext context)
        {
            if (ability.TargetingType != TargetingType.SingleTarget)
            {
                return true;
            }
            return context.Target is {IsAlive: true};
        }
    }
}


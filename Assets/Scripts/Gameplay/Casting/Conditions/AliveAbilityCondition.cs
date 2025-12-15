using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Conditions;

namespace AbilitySystem.Gameplay.Casting.Conditions
{
    /// <summary>
    /// Condition that checks if caster is alive
    /// </summary>
    public sealed class AliveAbilityCondition : IAbilityCondition
    {
        public string Id => "alive";
        public string FailureMessage => "Cannot cast while dead";

        public bool IsMet(IAbilityOwner caster, IAbility ability, AbilityCastContext context)
        {
            return caster.IsAlive;
        }
    }
}


using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Conditions;
using AbilitySystem.Core.Runtime;

namespace AbilitySystem.Gameplay.Casting.Conditions
{
    /// <summary>
    /// Condition that checks if ability is not on cooldown
    /// </summary>
    public sealed class CooldownAbilityCondition : IAbilityCondition
    {
        private readonly ICooldownManager _cooldownManager;

        public string Id => "cooldown";
        public string FailureMessage => "Ability is on cooldown";

        public CooldownAbilityCondition(ICooldownManager cooldownManager)
        {
            _cooldownManager = cooldownManager;
        }

        public bool IsMet(IAbilityOwner caster, IAbility ability, AbilityCastContext context)
        {
            return !_cooldownManager.IsOnCooldown(caster, ability.Id);
        }
    }
}


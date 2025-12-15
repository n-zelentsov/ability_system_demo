using System.Collections.Generic;
using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Conditions;

namespace AbilitySystem.Gameplay.Casting
{
    /// <summary>
    /// Pipeline for validating ability cast conditions
    /// </summary>
    public sealed class AbilityCastingConditionService
    {
        private readonly List<IAbilityCondition> _globalConditions = new();

        public void AddGlobalCondition(IAbilityCondition abilityCondition)
        {
            _globalConditions.Add(abilityCondition);
        }

        public void RemoveGlobalCondition(IAbilityCondition abilityCondition)
        {
            _globalConditions.Remove(abilityCondition);
        }

        public AbilityCastValidationResult Validate(IAbilityOwner caster, IAbility ability, AbilityCastContext context)
        {
            foreach (IAbilityCondition condition in _globalConditions)
            {
                if (!condition.IsMet(caster, ability, context))
                {
                    return AbilityCastValidationResult.Failed(condition.FailureMessage);
                }
            }

            foreach (IAbilityCondition condition in ability.Conditions)
            {
                if (!condition.IsMet(caster, ability, context))
                {
                    return AbilityCastValidationResult.Failed(condition.FailureMessage);
                }
            }

            return AbilityCastValidationResult.Success;
        }
    }
}


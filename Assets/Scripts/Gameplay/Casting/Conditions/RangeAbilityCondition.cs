using System;
using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Conditions;

namespace AbilitySystem.Gameplay.Casting.Conditions
{
    /// <summary>
    /// Condition that checks if target is within ability range
    /// </summary>
    public sealed class RangeAbilityCondition : IAbilityCondition
    {
        public string Id => "range";
        public string FailureMessage => "Target is out of range";

        public bool IsMet(IAbilityOwner caster, IAbility ability, AbilityCastContext context)
        {
            if (ability.TargetingType == TargetingType.Self)
                return true;

            if (ability.TargetingType == TargetingType.NoTarget)
                return true;

            if (context.Target != null)
            {
                float distance = CalculateDistance(caster.Position, context.Target.Position);
                return distance <= ability.Data.Range;
            }

            if (context.TargetPoint.HasValue)
            {
                float distance = CalculateDistance(caster.Position, context.TargetPoint.Value);
                return distance <= ability.Data.Range;
            }

            return true;
        }

        private static float CalculateDistance((float x, float y, float z) a, (float x, float y, float z) b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            float dz = a.z - b.z;
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
    }
}


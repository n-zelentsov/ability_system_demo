using AbilitySystem.Core.Abilities;

namespace AbilitySystem.Core.Conditions
{
    /// <summary>
    /// Context for checking cast conditions
    /// </summary>
    public sealed class AbilityCastContext
    {
        public IEffectTarget Target { get; }
        public (float x, float y, float z)? TargetPoint { get; }
        public (float x, float y, float z)? Direction { get; }

        public AbilityCastContext(
            IEffectTarget target = null,
            (float x, float y, float z)? targetPoint = null,
            (float x, float y, float z)? direction = null)
        {
            Target = target;
            TargetPoint = targetPoint;
            Direction = direction;
        }

        public static AbilityCastContext Empty => new AbilityCastContext();
        public static AbilityCastContext ForTarget(IEffectTarget target) => new AbilityCastContext(target);
        public static AbilityCastContext ForPoint((float x, float y, float z) point) => new AbilityCastContext(targetPoint: point);
    }
}
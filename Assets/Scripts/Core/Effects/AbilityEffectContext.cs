using AbilitySystem.Core.Abilities;

namespace AbilitySystem.Core.Effects
{
    /// <summary>
    /// Context containing all information needed to apply an effect
    /// </summary>
    public sealed class AbilityEffectContext
    {
        public IAbilityOwner Source { get; }
        public IEffectTarget Target { get; }
        public IAbility SourceAbility { get; }
        public (float x, float y, float z)? TargetPoint { get; }
        public (float x, float y, float z)? Direction { get; }
        public float PowerMultiplier { get; }

        public AbilityEffectContext(
            IAbilityOwner source,
            IEffectTarget target,
            IAbility sourceAbility = null,
            (float x, float y, float z)? targetPoint = null,
            (float x, float y, float z)? direction = null,
            float powerMultiplier = 1f)
        {
            Source = source;
            Target = target;
            SourceAbility = sourceAbility;
            TargetPoint = targetPoint;
            Direction = direction;
            PowerMultiplier = powerMultiplier;
        }

        public AbilityEffectContext WithTarget(IEffectTarget newTarget)
        {
            return new AbilityEffectContext(Source, newTarget, SourceAbility, TargetPoint, Direction, PowerMultiplier);
        }

        public AbilityEffectContext WithMultiplier(float multiplier)
        {
            return new AbilityEffectContext(Source, Target, SourceAbility, TargetPoint, Direction, multiplier);
        }
    }
}


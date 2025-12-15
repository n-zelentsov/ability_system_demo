using System.Collections.Generic;
using AbilitySystem.Core.Abilities;

namespace AbilitySystem.Gameplay.Targeting
{
    /// <summary>
    /// Strategy interface for different targeting methods
    /// </summary>
    public interface ITargetingStrategy
    {
        TargetingType SupportedType { get; }
        IReadOnlyList<IEffectTarget> FindTargets(TargetingRequest request);
        bool IsValidTarget(IEffectTarget target, TargetingRequest request);
    }

    public sealed class TargetingRequest
    {
        public IAbilityOwner Caster { get; }
        public IAbility Ability { get; }
        public IEffectTarget PrimaryTarget { get; }
        public (float x, float y, float z)? TargetPoint { get; }
        public (float x, float y, float z)? Direction { get; }
        public TargetFilter Filter { get; }

        public TargetingRequest(
            IAbilityOwner caster,
            IAbility ability,
            IEffectTarget primaryTarget = null,
            (float x, float y, float z)? targetPoint = null,
            (float x, float y, float z)? direction = null,
            TargetFilter filter = null)
        {
            Caster = caster;
            Ability = ability;
            PrimaryTarget = primaryTarget;
            TargetPoint = targetPoint;
            Direction = direction;
            Filter = filter ?? TargetFilter.Default;
        }
    }
}


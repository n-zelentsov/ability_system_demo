using System;
using System.Collections.Generic;
using System.Linq;
using AbilitySystem.Core.Abilities;

namespace AbilitySystem.Gameplay.Targeting
{
    /// <summary>
    /// Service for resolving targets based on ability targeting type
    /// </summary>
    public sealed class TargetingService
    {
        private readonly ITargetProvider _targetProvider;
        private readonly Dictionary<TargetingType, ITargetingStrategy> _strategies;

        public TargetingService(ITargetProvider targetProvider)
        {
            _targetProvider = targetProvider;
            _strategies = new Dictionary<TargetingType, ITargetingStrategy>();
            
            // Register default strategies
            RegisterStrategy(new SelfTargetingStrategy());
            RegisterStrategy(new SingleTargetStrategy());
            RegisterStrategy(new AreaTargetingStrategy(targetProvider));
            RegisterStrategy(new NoTargetStrategy());
        }

        public void RegisterStrategy(ITargetingStrategy strategy)
        {
            _strategies[strategy.SupportedType] = strategy;
        }

        public IReadOnlyList<IEffectTarget> ResolveTargets(TargetingRequest request)
        {
            if (_strategies.TryGetValue(request.Ability.TargetingType, out var strategy))
            {
                return strategy.FindTargets(request);
            }

            throw new InvalidOperationException($"No targeting strategy found for {request.Ability.TargetingType}");
        }

        public bool ValidateTarget(IEffectTarget target, TargetingRequest request)
        {
            if (_strategies.TryGetValue(request.Ability.TargetingType, out var strategy))
            {
                return strategy.IsValidTarget(target, request);
            }
            return false;
        }
    }

    internal sealed class SelfTargetingStrategy : ITargetingStrategy
    {
        public TargetingType SupportedType => TargetingType.Self;

        public IReadOnlyList<IEffectTarget> FindTargets(TargetingRequest request)
        {
            return new[] { request.Caster };
        }

        public bool IsValidTarget(IEffectTarget target, TargetingRequest request)
        {
            return target.Id == request.Caster.Id;
        }
    }

    internal sealed class SingleTargetStrategy : ITargetingStrategy
    {
        public TargetingType SupportedType => TargetingType.SingleTarget;

        public IReadOnlyList<IEffectTarget> FindTargets(TargetingRequest request)
        {
            if (request.PrimaryTarget == null)
                return Array.Empty<IEffectTarget>();

            if (!request.Filter.Passes(request.PrimaryTarget, request.Caster))
                return Array.Empty<IEffectTarget>();

            return new[] { request.PrimaryTarget };
        }

        public bool IsValidTarget(IEffectTarget target, TargetingRequest request)
        {
            if (!request.Filter.Passes(target, request.Caster))
                return false;

            // Check range
            var distance = CalculateDistance(request.Caster.Position, target.Position);
            return distance <= request.Ability.Data.Range;
        }

        private static float CalculateDistance((float x, float y, float z) a, (float x, float y, float z) b)
        {
            var dx = a.x - b.x;
            var dy = a.y - b.y;
            var dz = a.z - b.z;
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
    }

    internal sealed class AreaTargetingStrategy : ITargetingStrategy
    {
        private readonly ITargetProvider _targetProvider;

        public AreaTargetingStrategy(ITargetProvider targetProvider)
        {
            _targetProvider = targetProvider;
        }

        public TargetingType SupportedType => TargetingType.AreaOfEffect;

        public IReadOnlyList<IEffectTarget> FindTargets(TargetingRequest request)
        {
            var center = request.TargetPoint ?? request.Caster.Position;
            var radius = request.Ability.Data.AreaRadius;

            var targetsInRadius = _targetProvider.GetTargetsInRadius(center, radius);
            
            return targetsInRadius
                .Where(t => request.Filter.Passes(t, request.Caster))
                .Take(request.Filter.MaxTargets)
                .ToList();
        }

        public bool IsValidTarget(IEffectTarget target, TargetingRequest request)
        {
            return request.Filter.Passes(target, request.Caster);
        }
    }

    internal sealed class NoTargetStrategy : ITargetingStrategy
    {
        public TargetingType SupportedType => TargetingType.NoTarget;

        public IReadOnlyList<IEffectTarget> FindTargets(TargetingRequest request)
        {
            return Array.Empty<IEffectTarget>();
        }

        public bool IsValidTarget(IEffectTarget target, TargetingRequest request)
        {
            return false;
        }
    }
}


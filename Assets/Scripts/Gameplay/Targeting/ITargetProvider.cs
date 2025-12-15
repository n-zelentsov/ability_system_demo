using System.Collections.Generic;
using AbilitySystem.Core.Abilities;

namespace AbilitySystem.Gameplay.Targeting
{
    /// <summary>
    /// Interface for providing potential targets in the game world
    /// </summary>
    public interface ITargetProvider
    {
        IReadOnlyList<IEffectTarget> GetAllTargets();
        IEnumerable<IEffectTarget> GetTargetsInRadius((float x, float y, float z) center, float radius);
        IReadOnlyList<IEffectTarget> GetTargetsInCone((float x, float y, float z) origin, (float x, float y, float z) direction, float angle, float range);
        IEffectTarget GetClosestTarget((float x, float y, float z) position, TargetFilter filter, IAbilityOwner caster);
    }
}


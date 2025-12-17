using System;
using AbilitySystem.Core.Abilities;

namespace AbilitySystem.Gameplay.Targeting
{
    /// <summary>
    /// Filter configuration for target selection
    /// </summary>
    public sealed class TargetFilter
    {
        public TargetTeamFilter TeamFilter { get; }
        public bool IncludeDead { get; }
        public bool IncludeSelf { get; }
        public int MaxTargets { get; }
        public Func<IEffectTarget, bool> CustomFilter { get; }

        public TargetFilter(
            TargetTeamFilter teamFilter = TargetTeamFilter.Enemies,
            bool includeDead = false,
            bool includeSelf = false,
            int maxTargets = int.MaxValue,
            Func<IEffectTarget, bool> customFilter = null)
        {
            TeamFilter = teamFilter;
            IncludeDead = includeDead;
            IncludeSelf = includeSelf;
            MaxTargets = maxTargets;
            CustomFilter = customFilter;
        }

        public bool Passes(IEffectTarget target, IAbilityOwner caster)
        {
            if (target == null) return false;
            
            // Check if alive
            if (!IncludeDead && !target.IsAlive) return false;
            
            // Check self targeting
            if (!IncludeSelf && target.Id == caster.Id) return false;
            
            // Check team filter
            if (!PassesTeamFilter(target, caster)) return false;
            
            // Check custom filter
            if (CustomFilter != null && !CustomFilter(target)) return false;
            
            return true;
        }

        private bool PassesTeamFilter(IEffectTarget target, IAbilityOwner caster)
        {
            return TeamFilter switch
            {
                TargetTeamFilter.All => true,
                TargetTeamFilter.Allies => target.Team.IsAlly(caster.Team),
                TargetTeamFilter.Enemies => target.Team.IsEnemy(caster.Team),
                TargetTeamFilter.Self => target.Id == caster.Id,
                TargetTeamFilter.AlliesAndSelf => target.Team.IsAlly(caster.Team) || target.Id == caster.Id,
                _ => true
            };
        }

        public static TargetFilter Default => new();
        public static TargetFilter AlliesOnly => new(TargetTeamFilter.Allies);
        public static TargetFilter SelfOnly => new(TargetTeamFilter.Self, includeSelf: true);
        public static TargetFilter AlliesAndSelf => new(TargetTeamFilter.AlliesAndSelf, includeSelf: true);
    }
}





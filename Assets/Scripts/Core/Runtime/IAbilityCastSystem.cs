using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Conditions;

namespace AbilitySystem.Core.Runtime
{
    /// <summary>
    /// Main ability system interface - facade for the entire system
    /// </summary>
    public interface IAbilityCastSystem
    {
        /// <summary>
        /// Attempts to cast an ability
        /// </summary>
        AbilityCastResult TryCast(IAbilityOwner caster, AbilityId abilityId, AbilityCastContext context);
        
        /// <summary>
        /// Checks if ability can be cast without actually casting
        /// </summary>
        bool CanCast(IAbilityOwner caster, AbilityId abilityId, AbilityCastContext context);
        
        /// <summary>
        /// Gets the remaining cooldown for an ability
        /// </summary>
        float GetCooldownRemaining(IAbilityOwner owner, AbilityId abilityId);
        
        /// <summary>
        /// Checks if ability is on cooldown
        /// </summary>
        bool IsOnCooldown(IAbilityOwner owner, AbilityId abilityId);
        
        /// <summary>
        /// Updates the system (processes cooldowns, durations, etc.)
        /// </summary>
        void Update(float deltaTime);
    }
}


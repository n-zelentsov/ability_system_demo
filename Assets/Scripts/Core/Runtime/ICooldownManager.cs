using AbilitySystem.Core.Abilities;

namespace AbilitySystem.Core.Runtime
{
    /// <summary>
    /// Interface for managing ability cooldowns
    /// </summary>
    public interface ICooldownManager
    {
        void StartCooldown(IAbilityOwner owner, AbilityId abilityId, float duration);
        float GetRemainingCooldown(IAbilityOwner owner, AbilityId abilityId);
        bool IsOnCooldown(IAbilityOwner owner, AbilityId abilityId);
        void ResetCooldown(IAbilityOwner owner, AbilityId abilityId);
        void ReduceCooldown(IAbilityOwner owner, AbilityId abilityId, float amount);
        void Tick(float deltaTime);
    }
}





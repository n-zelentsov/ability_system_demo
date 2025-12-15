using AbilitySystem.Core.Abilities;

namespace AbilitySystem.Core.Effects
{
    /// <summary>
    /// Interface for effects that have a duration (buffs, debuffs, DoTs, HoTs)
    /// </summary>
    public interface IAbilityDurationAbilityEffect : IAbilityEffect
    {
        float Duration { get; }
        float RemainingTime { get; }
        bool IsExpired { get; }
        bool IsStackable { get; }
        int MaxStacks { get; }
        int CurrentStacks { get; }
        
        void Tick(float deltaTime);
        void Refresh();
        void AddStack();
        void RemoveStack();
        void OnApply(IEffectTarget target);
        void OnRemove(IEffectTarget target);
    }
}


namespace AbilitySystem.Core.Effects
{
    /// <summary>
    /// Interface for effects that tick periodically (DoT, HoT)
    /// </summary>
    public interface IPeriodicAbilityEffect : IAbilityDurationAbilityEffect
    {
        float TickInterval { get; }
        float TimeSinceLastTick { get; }
        
        void OnTick(AbilityEffectContext context);
    }
}


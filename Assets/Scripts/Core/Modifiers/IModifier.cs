namespace AbilitySystem.Core.Modifiers
{
    /// <summary>
    /// Interface for stat modifiers
    /// </summary>
    public interface IModifier
    {
        string Id { get; }
        string SourceId { get; }
        string TargetStatId { get; }
        DamageModifierType Type { get; }
        float Value { get; }
        int Priority { get; }
        bool IsExpired { get; }
        
        float Apply(float baseValue, float currentValue);
    }
}





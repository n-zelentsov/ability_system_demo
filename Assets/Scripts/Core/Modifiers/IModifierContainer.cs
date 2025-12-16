using System.Collections.Generic;

namespace AbilitySystem.Core.Modifiers
{
    /// <summary>
    /// Interface for entities that can hold modifiers
    /// </summary>
    public interface IModifierContainer
    {
        IEnumerable<IModifier> GetModifiers(string statId);
        void AddModifier(IModifier modifier);
        void RemoveModifier(IModifier modifier);
        void RemoveModifiersBySource(string sourceId);
        void ClearExpiredModifiers();
        float CalculateModifiedValue(string statId, float baseValue);
    }
}



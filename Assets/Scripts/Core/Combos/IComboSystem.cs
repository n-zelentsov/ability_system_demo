using System.Collections.Generic;
using AbilitySystem.Core.Abilities;

namespace AbilitySystem.Core.Combos
{
    /// <summary>
    /// Interface for combo system that tracks ability sequences
    /// </summary>
    public interface IComboSystem
    {
        void RegisterAbilityCast(IAbilityOwner caster, AbilityId abilityId);
        ComboResult CheckCombo(IAbilityOwner caster, AbilityId abilityId);
        IReadOnlyList<ComboDefinition> GetAvailableCombos(IAbilityOwner caster);
        void Tick(float deltaTime);
    }
}



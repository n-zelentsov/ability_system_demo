using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Combos;

namespace AbilitySystem.Core.Events
{
    public readonly struct AbilityComboProgressEvent : IEvent
    {
        public IAbilityOwner Caster { get; }
        public ComboDefinition Combo { get; }
        public int CurrentStep { get; }
        public int TotalSteps { get; }

        public AbilityComboProgressEvent(IAbilityOwner caster, ComboDefinition combo, int currentStep, int totalSteps)
        {
            Caster = caster;
            Combo = combo;
            CurrentStep = currentStep;
            TotalSteps = totalSteps;
        }
    }
}
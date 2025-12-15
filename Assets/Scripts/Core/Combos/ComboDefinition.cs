using AbilitySystem.Core.Abilities;

namespace AbilitySystem.Core.Combos
{
    public sealed class ComboDefinition
    {
        public string Id { get; }
        public string Name { get; }
        public AbilityId[] Sequence { get; }
        public float[] StepMultipliers { get; }
        public float TimeWindow { get; }
        public string[] BonusEffectIds { get; }
        public string FinisherEffectId { get; }

        public ComboDefinition(
            string id,
            string name,
            AbilityId[] sequence,
            float[] stepMultipliers,
            float timeWindow = 3f,
            string[] bonusEffectIds = null,
            string finisherEffectId = null)
        {
            Id = id;
            Name = name;
            Sequence = sequence;
            StepMultipliers = stepMultipliers;
            TimeWindow = timeWindow;
            BonusEffectIds = bonusEffectIds ?? System.Array.Empty<string>();
            FinisherEffectId = finisherEffectId;
        }
    }
}
namespace AbilitySystem.Core.Combos
{
    public sealed class ComboResult
    {
        public bool IsCombo { get; }
        public ComboDefinition Combo { get; }
        public int CurrentStep { get; }
        public float DamageMultiplier { get; }
        public string[] BonusEffectIds { get; }

        private ComboResult(bool isCombo, ComboDefinition combo, int step, float multiplier, string[] bonusEffects)
        {
            IsCombo = isCombo;
            Combo = combo;
            CurrentStep = step;
            DamageMultiplier = multiplier;
            BonusEffectIds = bonusEffects;
        }

        public static ComboResult None => new(false, null, 0, 1f, System.Array.Empty<string>());

        public static ComboResult Success(ComboDefinition combo, int step, float multiplier, string[] bonusEffects = null)
            => new(true, combo, step, multiplier, bonusEffects ?? System.Array.Empty<string>());
    }
}
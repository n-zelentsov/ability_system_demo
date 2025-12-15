namespace AbilitySystem.Core.Combos
{
    public sealed class ElementalReaction
    {
        public ElementalReactionType Type { get; }
        public float DamageMultiplier { get; }
        public string BonusEffectId { get; }
        public string Description { get; }

        private ElementalReaction(ElementalReactionType type, float multiplier, string effectId, string description)
        {
            Type = type;
            DamageMultiplier = multiplier;
            BonusEffectId = effectId;
            Description = description;
        }

        public static ElementalReaction None => new(ElementalReactionType.None, 1f, null, null);
        
        public static ElementalReaction Create(ElementalReactionType type, float multiplier, string effectId,
            string description) => new(type, multiplier, effectId, description);
    }
}
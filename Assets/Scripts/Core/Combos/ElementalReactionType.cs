namespace AbilitySystem.Core.Combos
{
    public enum ElementalReactionType
    {
        None,
        
        /// <summary>Fire + Ice = Melt (2x damage)</summary>
        Melt,
        
        /// <summary>Fire + Lightning = Overload (AoE explosion)</summary>
        Overload,
        
        /// <summary>Ice + Lightning = Superconduct (defense down)</summary>
        Superconduct,
        
        /// <summary>Fire + Nature = Burning (enhanced DoT)</summary>
        Burning,
        
        /// <summary>Ice + Nature = Frozen (stun)</summary>
        Frozen,
        
        /// <summary>Lightning + Nature = Electro-charged (chain damage)</summary>
        Electrocharged
    }
}
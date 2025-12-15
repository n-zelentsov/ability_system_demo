namespace AbilitySystem.Core.Modifiers
{
    public enum DamageModifierType
    {
        /// <summary>
        /// Adds flat value to base stat: base + value
        /// </summary>
        Flat,
        
        /// <summary>
        /// Multiplies base stat: base * (1 + value)
        /// </summary>
        PercentAdd,
        
        /// <summary>
        /// Multiplies final value: final * (1 + value)
        /// </summary>
        PercentMultiply,
        
        /// <summary>
        /// Overrides value completely
        /// </summary>
        Override
    }
}
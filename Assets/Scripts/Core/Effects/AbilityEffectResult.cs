namespace AbilitySystem.Core.Effects
{
    /// <summary>
    /// Result of applying an effect
    /// </summary>
    public sealed class AbilityEffectResult
    {
        public bool Success { get; }
        public float Value { get; }
        public string Message { get; }
        public AbilityEffectResultType ResultType { get; }

        private AbilityEffectResult(bool success, float value, string message, AbilityEffectResultType resultType)
        {
            Success = success;
            Value = value;
            Message = message;
            ResultType = resultType;
        }

        public static AbilityEffectResult Succeeded(float value = 0f, string message = null) =>
            new(true, value, message, AbilityEffectResultType.Normal);

        public static AbilityEffectResult Critical(float value, string message = null) =>
            new(true, value, message, AbilityEffectResultType.Critical);

        public static AbilityEffectResult Blocked(string message = null) =>
            new(false, 0f, message, AbilityEffectResultType.Blocked);

        public static AbilityEffectResult Resisted(float reducedValue, string message = null) =>
            new(true, reducedValue, message, AbilityEffectResultType.Resisted);

        public static AbilityEffectResult Immune(string message = null) =>
            new(false, 0f, message, AbilityEffectResultType.Immune);

        public static AbilityEffectResult Failed(string message) => new(false, 0f, message, AbilityEffectResultType.Failed);
    }
}


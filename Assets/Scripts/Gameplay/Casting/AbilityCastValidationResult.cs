namespace AbilitySystem.Gameplay.Casting
{
    public sealed class AbilityCastValidationResult
    {
        public bool IsValid { get; }
        public string FailureReason { get; }

        private AbilityCastValidationResult(bool isValid, string failureReason)
        {
            IsValid = isValid;
            FailureReason = failureReason;
        }

        public static AbilityCastValidationResult Success => new(true, null);
        public static AbilityCastValidationResult Failed(string reason) => new(false, reason);
    }
}
using AbilitySystem.Core.Effects;

namespace AbilitySystem.Core.Runtime
{
    public sealed class AbilityCastResult
    {
        public bool Success { get; }
        public string FailureReason { get; }
        public AbilityEffectResult[] EffectResults { get; }

        private AbilityCastResult(bool success, string failureReason, AbilityEffectResult[] effectResults)
        {
            Success = success;
            FailureReason = failureReason;
            EffectResults = effectResults;
        }

        public static AbilityCastResult Succeeded(AbilityEffectResult[] results) => new(true, null, results);

        public static AbilityCastResult Failed(string reason) => new(false, reason, System.Array.Empty<AbilityEffectResult>());
    }
}
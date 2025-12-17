namespace AbilitySystem.Core.Runtime
{
    /// <summary>
    /// Abstraction for time to allow testing and different time scales
    /// </summary>
    public interface ITimeProvider
    {
        float CurrentTime { get; }
        float DeltaTime { get; }
        float TimeScale { get; set; }
    }
}





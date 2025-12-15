using MessagePipe;
using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Effects;
using AbilitySystem.Core.Events;
using AbilitySystem.Gameplay.Stats;

namespace AbilitySystem.Gameplay.Services.Effects
{
    /// <summary>
    /// Effect that deals damage over time (DoT)
    /// </summary>
    public sealed class DamageOverTimeAbilityEffect : IPeriodicAbilityEffect
    {
        private readonly IPublisher<DamageDealtEvent> _damagePublisher;
        
        private float _remainingTime;
        private float _timeSinceLastTick;
        private int _currentStacks;
        private IAbilityOwner _source;

        public string Id { get; }
        public string Name { get; }
        public AbilityEffectType Type => AbilityEffectType.Periodic;
        public float Duration { get; }
        public float RemainingTime => _remainingTime;
        public bool IsExpired => _remainingTime <= 0;
        public bool IsStackable { get; }
        public int MaxStacks { get; }
        public int CurrentStacks => _currentStacks;
        public float TickInterval { get; }
        public float TimeSinceLastTick => _timeSinceLastTick;
        public float DamagePerTick { get; }
        public DamageType DamageType { get; }

        public DamageOverTimeAbilityEffect(
            string id, string name, float duration, float tickInterval, float damagePerTick,
            DamageType damageType, IPublisher<DamageDealtEvent> damagePublisher,
            bool isStackable = false, int maxStacks = 1)
        {
            Id = id;
            Name = name;
            Duration = duration;
            _remainingTime = duration;
            TickInterval = tickInterval;
            DamagePerTick = damagePerTick;
            DamageType = damageType;
            _damagePublisher = damagePublisher;
            IsStackable = isStackable;
            MaxStacks = maxStacks;
            _currentStacks = 1;
        }

        public bool CanApply(AbilityEffectContext context) => context.Target != null && context.Target.IsAlive;

        public AbilityEffectResult Apply(AbilityEffectContext context)
        {
            _source = context.Source;
            return AbilityEffectResult.Succeeded(0, $"{Name} applied");
        }

        public void Tick(float deltaTime)
        {
            _remainingTime -= deltaTime;
            _timeSinceLastTick += deltaTime;
        }

        public void OnTick(AbilityEffectContext context)
        {
            if (_timeSinceLastTick >= TickInterval)
            {
                _timeSinceLastTick = 0;
                float damage = DamagePerTick * _currentStacks;
                context.Target.ModifyStat(EntityStatType.MaxHealth, -damage);
                _damagePublisher.Publish(new DamageDealtEvent(_source, context.Target, damage, DamageType, false));
            }
        }

        public void Refresh() => _remainingTime = Duration;
        public void AddStack() { if (_currentStacks < MaxStacks) _currentStacks++; }
        public void RemoveStack() { if (_currentStacks > 0) _currentStacks--; }
        public void OnApply(IEffectTarget target) { }
        public void OnRemove(IEffectTarget target) { }
    }
}

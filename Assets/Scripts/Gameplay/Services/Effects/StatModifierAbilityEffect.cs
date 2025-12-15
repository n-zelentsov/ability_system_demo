using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Effects;
using AbilitySystem.Core.Modifiers;

namespace AbilitySystem.Gameplay.Services.Effects
{
    /// <summary>
    /// Effect that applies a stat modifier (buff/debuff)
    /// </summary>
    public sealed class StatModifierAbilityEffect : IAbilityDurationAbilityEffect
    {
        private float _remainingTime;
        private int _currentStacks;
        private StatModifier _appliedModifier;

        public string Id { get; }
        public string Name { get; }
        public AbilityEffectType Type => AbilityEffectType.Duration;
        
        public float Duration { get; }
        public float RemainingTime => _remainingTime;
        public bool IsExpired => _remainingTime <= 0;
        public bool IsStackable { get; }
        public int MaxStacks { get; }
        public int CurrentStacks => _currentStacks;
        
        public string TargetStatId { get; }
        public DamageModifierType DamageModifierType { get; }
        public float ModifierValue { get; }

        public StatModifierAbilityEffect(
            string id,
            string name,
            float duration,
            string targetStatId,
            DamageModifierType damageModifierType,
            float modifierValue,
            bool isStackable = false,
            int maxStacks = 1)
        {
            Id = id;
            Name = name;
            Duration = duration;
            _remainingTime = duration;
            TargetStatId = targetStatId;
            DamageModifierType = damageModifierType;
            ModifierValue = modifierValue;
            IsStackable = isStackable;
            MaxStacks = maxStacks;
            _currentStacks = 1;
        }

        public bool CanApply(AbilityEffectContext context)
        {
            return context.Target != null;
        }

        public AbilityEffectResult Apply(AbilityEffectContext context)
        {
            return AbilityEffectResult.Succeeded(0, $"{Name} applied");
        }

        public void Tick(float deltaTime)
        {
            _remainingTime -= deltaTime;
        }

        public void Refresh()
        {
            _remainingTime = Duration;
        }

        public void AddStack()
        {
            if (_currentStacks < MaxStacks)
            {
                _currentStacks++;
                // Modifier value increases with stacks
            }
        }

        public void RemoveStack()
        {
            if (_currentStacks > 0)
                _currentStacks--;
        }

        public void OnApply(IEffectTarget target)
        {
            _appliedModifier = new StatModifier(
                $"{Id}_{target.Id}",
                Id,
                TargetStatId,
                DamageModifierType,
                ModifierValue * _currentStacks);
            
            target.ApplyModifier(_appliedModifier);
        }

        public void OnRemove(IEffectTarget target)
        {
            if (_appliedModifier != null)
            {
                target.RemoveModifier(_appliedModifier);
                _appliedModifier = null;
            }
        }
    }

    /// <summary>
    /// Concrete stat modifier implementation
    /// </summary>
    public sealed class StatModifier : IModifier
    {
        public string Id { get; }
        public string SourceId { get; }
        public string TargetStatId { get; }
        public DamageModifierType Type { get; }
        public float Value { get; }
        public int Priority { get; }
        public bool IsExpired => false; // Managed by effect

        public StatModifier(
            string id,
            string sourceId,
            string targetStatId,
            DamageModifierType type,
            float value,
            int priority = 0)
        {
            Id = id;
            SourceId = sourceId;
            TargetStatId = targetStatId;
            Type = type;
            Value = value;
            Priority = priority;
        }

        public float Apply(float baseValue, float currentValue)
        {
            return Type switch
            {
                DamageModifierType.Flat => currentValue + Value,
                DamageModifierType.PercentAdd => currentValue + (baseValue * Value),
                DamageModifierType.PercentMultiply => currentValue * (1 + Value),
                DamageModifierType.Override => Value,
                _ => currentValue
            };
        }
    }
}


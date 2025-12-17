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
        private StatModifierAbility _appliedModifierAbility;

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
            _appliedModifierAbility = new StatModifierAbility(
                $"{Id}_{target.Id}",
                Id,
                TargetStatId,
                DamageModifierType,
                ModifierValue * _currentStacks);
            
            target.ApplyModifier(_appliedModifierAbility);
        }

        public void OnRemove(IEffectTarget target)
        {
            if (_appliedModifierAbility != null)
            {
                target.RemoveModifier(_appliedModifierAbility);
                _appliedModifierAbility = null;
            }
        }
    }
}


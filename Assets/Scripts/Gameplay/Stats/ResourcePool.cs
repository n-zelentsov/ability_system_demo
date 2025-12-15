using System;
using AbilitySystem.Core.Events;

namespace AbilitySystem.Gameplay.Stats
{
    /// <summary>
    /// Represents a resource pool (health, mana, energy, etc.)
    /// </summary>
    public sealed class AbilityCostPool
    {
        private float _currentValue;
        private float _maxValue;

        public AbilityCostType Type { get; }
        public float RegenRate { get; set; }

        public float CurrentValue
        {
            get => _currentValue;
            private set => _currentValue = Math.Max(0, Math.Min(value, _maxValue));
        }

        public float MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = Math.Max(0, value);
                if (_currentValue > _maxValue)
                {
                    _currentValue = _maxValue;
                }
            }
        }

        public float Percentage => _maxValue > 0 ? _currentValue / _maxValue : 0f;
        public bool IsEmpty => _currentValue <= 0;
        public bool IsFull => _currentValue >= _maxValue;

        public event Action<float, float, float> OnValueChanged; // old, new, max

        public AbilityCostPool(AbilityCostType type, float maxValue, float? startValue = null)
        {
            Type = type;
            _maxValue = maxValue;
            _currentValue = startValue ?? maxValue;
        }

        public bool HasEnough(float amount)
        {
            return _currentValue >= amount;
        }

        public bool TryConsume(float amount)
        {
            if (!HasEnough(amount))
            {
                return false;
            }
            Modify(-amount);
            return true;
        }

        public void Consume(float amount)
        {
            Modify(-amount);
        }

        public void Restore(float amount)
        {
            Modify(amount);
        }

        public void Modify(float delta)
        {
            float oldValue = _currentValue;
            CurrentValue += delta;

            if (Math.Abs(oldValue - _currentValue) > float.Epsilon)
            {
                OnValueChanged?.Invoke(oldValue, _currentValue, _maxValue);
            }
        }

        public void SetToMax()
        {
            float oldValue = _currentValue;
            _currentValue = _maxValue;

            if (Math.Abs(oldValue - _currentValue) > float.Epsilon)
            {
                OnValueChanged?.Invoke(oldValue, _currentValue, _maxValue);
            }
        }

        public void SetToZero()
        {
            float oldValue = _currentValue;
            _currentValue = 0;

            if (Math.Abs(oldValue - _currentValue) > float.Epsilon)
            {
                OnValueChanged?.Invoke(oldValue, _currentValue, _maxValue);
            }
        }

        public void Tick(float deltaTime)
        {
            if (RegenRate != 0 && !IsFull)
            {
                Restore(RegenRate * deltaTime);
            }
        }
    }
}


using System;
using System.Collections.Generic;
using AbilitySystem.Core.Modifiers;

namespace AbilitySystem.Gameplay.Stats
{
    /// <summary>
    /// Represents a single stat with base value and modifier support
    /// </summary>
    public sealed class EntityStat
    {
        private float _baseValue;
        private readonly List<IModifier> _modifiers = new();
        private bool _isDirty = true;
        private float _cachedValue;

        public string Id { get; }

        public float BaseValue
        {
            get => _baseValue;
            set
            {
                if (Math.Abs(_baseValue - value) > float.Epsilon)
                {
                    _baseValue = value;
                    _isDirty = true;
                }
            }
        }

        public float Value
        {
            get
            {
                if (_isDirty)
                {
                    _cachedValue = CalculateValue();
                    _isDirty = false;
                }
                return _cachedValue;
            }
        }

        public float MinValue { get; set; } = float.MinValue;
        public float MaxValue { get; set; } = float.MaxValue;

        public EntityStat(string id, float baseValue = 0f)
        {
            Id = id;
            _baseValue = baseValue;
        }

        public void AddModifier(IModifier modifier)
        {
            _modifiers.Add(modifier);
            _modifiers.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            _isDirty = true;
        }

        public bool RemoveModifier(IModifier modifier)
        {
            if (_modifiers.Remove(modifier))
            {
                _isDirty = true;
                return true;
            }
            return false;
        }

        public void RemoveModifiersBySource(string sourceId)
        {
            int removed = _modifiers.RemoveAll(m => m.SourceId == sourceId);
            if (removed > 0)
            {
                _isDirty = true;
            }
        }

        public void ClearModifiers()
        {
            _modifiers.Clear();
            _isDirty = true;
        }

        public void ClearExpiredModifiers()
        {
            int removed = _modifiers.RemoveAll(m => m.IsExpired);
            if (removed > 0)
            {
                _isDirty = true;
            }
        }

        public IReadOnlyList<IModifier> GetModifiers()
        {
            return _modifiers;
        }

        private float CalculateValue()
        {
            float flatBonus = 0f;
            float percentAdd = 0f;
            float percentMultiply = 1f;
            float? overrideValue = null;

            foreach (IModifier modifier in _modifiers)
            {
                if (modifier.IsExpired)
                {
                    continue;
                }

                switch (modifier.Type)
                {
                    case DamageModifierType.Flat:
                        flatBonus += modifier.Value;
                        break;
                    case DamageModifierType.PercentAdd:
                        percentAdd += modifier.Value;
                        break;
                    case DamageModifierType.PercentMultiply:
                        percentMultiply *= (1f + modifier.Value);
                        break;
                    case DamageModifierType.Override:
                        overrideValue = modifier.Value;
                        break;
                }
            }

            if (overrideValue.HasValue)
            {
                return Clamp(overrideValue.Value);
            }
            float finalValue = (_baseValue + flatBonus) * (1f + percentAdd) * percentMultiply;
            return Clamp(finalValue);
        }

        private float Clamp(float value)
        {
            if (value < MinValue)
            {
                return MinValue;
            }
            return value > MaxValue ? MaxValue : value;
        }

        public void Invalidate() => _isDirty = true;
    }
}


using System;
using System.Collections.Generic;
using AbilitySystem.Core.Modifiers;

namespace AbilitySystem.Gameplay.Stats
{
    /// <summary>
    /// Container for managing multiple stats with modifier support
    /// </summary>
    public sealed class EntityStatContainer : IModifierContainer
    {
        private readonly Dictionary<string, EntityStat> _stats = new();

        public event Action<string, float, float> OnStatChanged;

        public void RegisterStat(string statId, float baseValue, float? minValue = null, float? maxValue = null)
        {
            if (_stats.ContainsKey(statId))
            {
                throw new InvalidOperationException($"Stat {statId} is already registered");
            }

            EntityStat stat = new(statId, baseValue);
            if (minValue.HasValue)
            {
                stat.MinValue = minValue.Value;
            }

            if (maxValue.HasValue)
            {
                stat.MaxValue = maxValue.Value;
            }
            _stats[statId] = stat;
        }

        public float GetStatValue(string statId)
        {
            return _stats.TryGetValue(statId, out EntityStat stat) ? stat.Value : 0f;
        }

        public float GetBaseValue(string statId)
        {
            return _stats.TryGetValue(statId, out EntityStat stat) ? stat.BaseValue : 0f;
        }

        public void SetBaseValue(string statId, float value)
        {
            if (!_stats.TryGetValue(statId, out EntityStat stat))
            {
                return;
            }
            float oldValue = stat.Value;
            stat.BaseValue = value;
            float newValue = stat.Value;

            if (Math.Abs(oldValue - newValue) > float.Epsilon)
            {
                OnStatChanged?.Invoke(statId, oldValue, newValue);
            }
        }

        public void ModifyBaseValue(string statId, float delta)
        {
            if (_stats.TryGetValue(statId, out EntityStat stat))
            {
                SetBaseValue(statId, stat.BaseValue + delta);
            }
        }

        public bool HasStat(string statId) => _stats.ContainsKey(statId);

        public IEnumerable<IModifier> GetModifiers(string statId)
        {
            return _stats.TryGetValue(statId, out EntityStat stat) 
                ? stat.GetModifiers() 
                : Array.Empty<IModifier>();
        }

        public void AddModifier(IModifier modifier)
        {
            if (!_stats.TryGetValue(modifier.TargetStatId, out EntityStat stat))
            {
                return;
            }
            float oldValue = stat.Value;
            stat.AddModifier(modifier);
            float newValue = stat.Value;
                
            if (Math.Abs(oldValue - newValue) > float.Epsilon)
                OnStatChanged?.Invoke(modifier.TargetStatId, oldValue, newValue);
        }

        public void RemoveModifier(IModifier modifier)
        {
            if (!_stats.TryGetValue(modifier.TargetStatId, out EntityStat stat))
            {
                return;
            }
            float oldValue = stat.Value;
            stat.RemoveModifier(modifier);
            float newValue = stat.Value;
                
            if (Math.Abs(oldValue - newValue) > float.Epsilon)
                OnStatChanged?.Invoke(modifier.TargetStatId, oldValue, newValue);
        }

        public void RemoveModifiersBySource(string sourceId)
        {
            foreach (EntityStat stat in _stats.Values)
            {
                float oldValue = stat.Value;
                stat.RemoveModifiersBySource(sourceId);
                float newValue = stat.Value;
                
                if (Math.Abs(oldValue - newValue) > float.Epsilon)
                    OnStatChanged?.Invoke(stat.Id, oldValue, newValue);
            }
        }

        public void ClearExpiredModifiers()
        {
            foreach (EntityStat stat in _stats.Values)
            {
                stat.ClearExpiredModifiers();
            }
        }

        public float CalculateModifiedValue(string statId, float baseValue)
        {
            if (!_stats.TryGetValue(statId, out var stat))
            {
                return baseValue;
            }

            // Temporarily set base value, calculate, then restore
            float originalBase = stat.BaseValue;
            stat.BaseValue = baseValue;
            stat.Invalidate();
            float result = stat.Value;
            stat.BaseValue = originalBase;
            stat.Invalidate();
            return result;
        }

        public IEnumerable<string> GetAllStatIds()
        {
            return _stats.Keys;
        }
    }
}


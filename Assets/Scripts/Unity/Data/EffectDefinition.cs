using UnityEngine;
using AbilitySystem.Core.Effects;
using AbilitySystem.Core.Events;
using AbilitySystem.Core.Modifiers;

namespace AbilitySystem.Unity.Data
{
    /// <summary>
    /// ScriptableObject for defining effects in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = "NewEffect", menuName = "Ability System/Effect Definition")]
    public class EffectDefinition : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string effectId;
        [SerializeField] private string displayName;
        [SerializeField] private EffectCategory category;
        
        [Header("Type Settings")]
        [SerializeField] private AbilityEffectType _abilityEffectType = AbilityEffectType.Instant;
        [SerializeField] private float duration = 0f;
        [SerializeField] private float tickInterval = 1f;
        
        [Header("Stacking")]
        [SerializeField] private bool isStackable = false;
        [SerializeField] private int maxStacks = 1;
        
        [Header("Damage Settings")]
        [SerializeField] private float baseDamage = 0f;
        [SerializeField] private DamageType damageType = DamageType.Physical;
        
        [Header("Heal Settings")]
        [SerializeField] private float baseHeal = 0f;
        
        [Header("Stat Modifier Settings")]
        [SerializeField] private string targetStatId;
        [SerializeField] private DamageModifierType _damageModifierType = DamageModifierType.Flat;
        [SerializeField] private float modifierValue = 0f;
        
        [Header("Scaling")]
        [SerializeField] private float scalingFactor = 0f;
        [SerializeField] private string scalingStat = "spell_power";

        public string EffectId => effectId;
        public string DisplayName => displayName;
        public EffectCategory Category => category;
        public AbilityEffectType AbilityEffectType => _abilityEffectType;
        public float Duration => duration;
        public float TickInterval => tickInterval;
        public bool IsStackable => isStackable;
        public int MaxStacks => maxStacks;
        public float BaseDamage => baseDamage;
        public DamageType DamageType => damageType;
        public float BaseHeal => baseHeal;
        public string TargetStatId => targetStatId;
        public DamageModifierType DamageModifierType => _damageModifierType;
        public float ModifierValue => modifierValue;
        public float ScalingFactor => scalingFactor;
        public string ScalingStat => scalingStat;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(effectId))
                effectId = name.ToLower().Replace(" ", "_");
        }
    }

    public enum EffectCategory
    {
        Damage,
        Heal,
        DamageOverTime,
        HealOverTime,
        Buff,
        Debuff,
        Shield,
        Knockback,
        Stun,
        Slow,
        Root
    }
}


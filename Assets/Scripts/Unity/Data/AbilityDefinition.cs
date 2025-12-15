using UnityEngine;
using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Events;

namespace AbilitySystem.Unity.Data
{
    /// <summary>
    /// ScriptableObject for defining abilities in the Unity Editor
    /// </summary>
    [CreateAssetMenu(fileName = "NewAbility", menuName = "Ability System/Ability Definition")]
    public class AbilityDefinition : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string abilityId;
        [SerializeField] private string displayName;
        [SerializeField, TextArea] private string description;
        [SerializeField] private Sprite icon;
        
        [Header("Targeting")]
        [SerializeField] private TargetingType targetingType = TargetingType.SingleTarget;
        
        [Header("Timing")]
        [SerializeField] private float cooldown = 1f;
        [SerializeField] private float castTime = 0f;
        [SerializeField] private bool canCastWhileMoving = true;
        
        [Header("Channeling")]
        [SerializeField] private bool isChanneled = false;
        [SerializeField] private float channelDuration = 0f;
        
        [Header("Range")]
        [SerializeField] private float range = 10f;
        [SerializeField] private float areaRadius = 0f;
        
        [Header("Charges")]
        [SerializeField] private int maxCharges = 1;
        [SerializeField] private float chargeRestoreTime = 0f;
        
        [Header("Resource Costs")]
        [SerializeField] private ResourceCostData[] resourceCosts;
        
        [Header("Effects")]
        [SerializeField] private EffectDefinition[] effects;

        public string AbilityId => abilityId;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
        public TargetingType TargetingType => targetingType;
        public float Cooldown => cooldown;
        public float CastTime => castTime;
        public bool CanCastWhileMoving => canCastWhileMoving;
        public bool IsChanneled => isChanneled;
        public float ChannelDuration => channelDuration;
        public float Range => range;
        public float AreaRadius => areaRadius;
        public int MaxCharges => maxCharges;
        public float ChargeRestoreTime => chargeRestoreTime;
        public ResourceCostData[] ResourceCosts => resourceCosts;
        public EffectDefinition[] Effects => effects;

        public AbilityData ToAbilityData()
        {
            var costs = new AbilityCost[resourceCosts?.Length ?? 0];
            for (int i = 0; i < costs.Length; i++)
            {
                costs[i] = resourceCosts[i].ToResourceCost();
            }

            return new AbilityData(
                cooldown: cooldown,
                castTime: castTime,
                range: range,
                areaRadius: areaRadius,
                resourceCosts: costs,
                canCastWhileMoving: canCastWhileMoving,
                isChanneled: isChanneled,
                channelDuration: channelDuration,
                maxCharges: maxCharges,
                chargeRestoreTime: chargeRestoreTime);
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(abilityId))
                abilityId = name.ToLower().Replace(" ", "_");
        }
    }

    [System.Serializable]
    public class ResourceCostData
    {
        public AbilityCostType type;
        public float amount;

        public AbilityCost ToResourceCost() => new AbilityCost(type, amount);
    }
}


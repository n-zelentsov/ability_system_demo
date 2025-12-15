namespace AbilitySystem.Core.Abilities
{
    /// <summary>
    /// Immutable data structure containing ability parameters
    /// </summary>
    public sealed class AbilityData
    {
        public float Cooldown { get; }
        public float CastTime { get; }
        public float Range { get; }
        public float AreaRadius { get; }
        public AbilityCost[] ResourceCosts { get; }
        public bool CanCastWhileMoving { get; }
        public bool IsChanneled { get; }
        public float ChannelDuration { get; }
        public int MaxCharges { get; }
        public float ChargeRestoreTime { get; }

        public AbilityData(float cooldown = 0f,
            float castTime = 0f,
            float range = 0f,
            float areaRadius = 0f,
            AbilityCost[] resourceCosts = null,
            bool canCastWhileMoving = true,
            bool isChanneled = false,
            float channelDuration = 0f,
            int maxCharges = 1,
            float chargeRestoreTime = 0f)
        {
            Cooldown = cooldown;
            CastTime = castTime;
            Range = range;
            AreaRadius = areaRadius;
            ResourceCosts = resourceCosts ?? System.Array.Empty<AbilityCost>();
            CanCastWhileMoving = canCastWhileMoving;
            IsChanneled = isChanneled;
            ChannelDuration = channelDuration;
            MaxCharges = maxCharges;
            ChargeRestoreTime = chargeRestoreTime;
        }
    }
}


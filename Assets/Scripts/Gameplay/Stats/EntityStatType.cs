namespace AbilitySystem.Gameplay.Stats
{
    /// <summary>
    /// Common stat types used in the ability system
    /// </summary>
    public static class EntityStatType
    {
        // Primary Stats
        public const string MaxHealth = "max_health";
        public const string MaxMana = "max_mana";
        public const string MaxEnergy = "max_energy";
        
        // Offensive Stats
        public const string AttackPower = "attack_power";
        public const string SpellPower = "spell_power";
        public const string CriticalChance = "crit_chance";
        public const string CriticalDamage = "crit_damage";
        public const string AttackSpeed = "attack_speed";
        public const string CastSpeed = "cast_speed";
        
        // Defensive Stats
        public const string Armor = "armor";
        public const string MagicResist = "magic_resist";
        public const string DodgeChance = "dodge_chance";
        public const string BlockChance = "block_chance";
        
        // Utility Stats
        public const string MovementSpeed = "move_speed";
        public const string CooldownReduction = "cooldown_reduction";
        public const string HealthRegen = "health_regen";
        public const string ManaRegen = "mana_regen";
        
        // Resistance Stats
        public const string FireResist = "fire_resist";
        public const string IceResist = "ice_resist";
        public const string LightningResist = "lightning_resist";
    }
}


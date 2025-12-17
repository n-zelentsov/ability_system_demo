using UnityEngine;
using AbilitySystem.Core.Events;

namespace AbilitySystem.Unity.Data
{
    /// <summary>
    /// ScriptableObject for defining entity base stats
    /// </summary>
    [CreateAssetMenu(fileName = "NewEntityStats", menuName = "Ability System/Entity Stats Definition")]
    public class EntityStatsDefinition : ScriptableObject
    {
        [Header("Resources")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float maxMana = 100f;
        [SerializeField] private float maxEnergy = 100f;
        [SerializeField] private float healthRegen = 1f;
        [SerializeField] private float manaRegen = 5f;
        
        [Header("Offensive Stats")]
        [SerializeField] private float attackPower = 10f;
        [SerializeField] private float spellPower = 10f;
        [SerializeField] private float criticalChance = 0.05f;
        [SerializeField] private float criticalDamage = 0.5f;
        [SerializeField] private float attackSpeed = 1f;
        [SerializeField] private float castSpeed = 1f;
        
        [Header("Defensive Stats")]
        [SerializeField] private float armor = 0f;
        [SerializeField] private float magicResist = 0f;
        [SerializeField] private float dodgeChance = 0f;
        [SerializeField] private float blockChance = 0f;
        
        [Header("Utility Stats")]
        [SerializeField] private float movementSpeed = 5f;
        [SerializeField] private float cooldownReduction = 0f;
        
        [Header("Elemental Resistances")]
        [SerializeField] private float fireResist = 0f;
        [SerializeField] private float iceResist = 0f;
        [SerializeField] private float lightningResist = 0f;

        public float MaxHealth => maxHealth;
        public float MaxMana => maxMana;
        public float MaxEnergy => maxEnergy;
        public float HealthRegen => healthRegen;
        public float ManaRegen => manaRegen;
        public float AttackPower => attackPower;
        public float SpellPower => spellPower;
        public float CriticalChance => criticalChance;
        public float CriticalDamage => criticalDamage;
        public float AttackSpeed => attackSpeed;
        public float CastSpeed => castSpeed;
        public float Armor => armor;
        public float MagicResist => magicResist;
        public float DodgeChance => dodgeChance;
        public float BlockChance => blockChance;
        public float MovementSpeed => movementSpeed;
        public float CooldownReduction => cooldownReduction;
        public float FireResist => fireResist;
        public float IceResist => iceResist;
        public float LightningResist => lightningResist;
    }
}





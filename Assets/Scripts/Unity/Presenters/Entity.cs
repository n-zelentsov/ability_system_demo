using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;
using UniRx;
using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Events;
using AbilitySystem.Core.Modifiers;
using AbilitySystem.Gameplay.Stats;
using AbilitySystem.Unity.Data;
using AbilitySystem.Unity.Factories;

namespace AbilitySystem.Unity.Presenters
{
    /// <summary>
    /// Entity with UniRx reactive properties and detailed Gizmos
    /// </summary>
    public class Entity : MonoBehaviour, IAbilityOwner
    {
        [Header("Configuration")]
        [SerializeField] private string entityId;
        [SerializeField] private string entityName = "Entity";
        [SerializeField] private int teamId = 1;
        [SerializeField] private EntityStatsDefinition statsDefinition;
        [SerializeField] private AbilityDefinition[] startingAbilities;
        
        private AbilityFactory _abilityFactory;

        private readonly EntityStatContainer _entityStats = new();
        private readonly Dictionary<AbilityCostType, AbilityCostPool> _resources = new();
        private readonly List<IAbility> _abilities = new();
        private bool _isAlive = true;
        private bool _initialized;

        // UniRx Reactive properties
        private readonly ReactiveProperty<float> _health = new();
        private readonly ReactiveProperty<float> _mana = new();
        private readonly ReactiveProperty<bool> _alive = new(true);

        public IReadOnlyReactiveProperty<float> Health => _health;
        public IReadOnlyReactiveProperty<float> Mana => _mana;
        public IReadOnlyReactiveProperty<bool> Alive => _alive;

        public string Id => entityId;
        public string Name => entityName;
        public bool IsAlive => _isAlive;
        public TeamId Team => new(teamId);
        public (float x, float y, float z) Position => (transform.position.x, transform.position.y, transform.position.z);
        public IReadOnlyList<IAbility> Abilities => _abilities;

        public event Action<Entity> OnDeath;

        private void Awake()
        {
            if (string.IsNullOrEmpty(entityId))
                entityId = Guid.NewGuid().ToString();
        }

        [Inject]
        public void Construct(AbilityFactory abilityFactory)
        {
            _abilityFactory = abilityFactory;
            Initialize();
        }

        private void Start()
        {
            if (!_initialized) Initialize();
        }

        private void Initialize()
        {
            if (_initialized) return;
            _initialized = true;
            
            InitializeStats();
            InitializeResources();
            InitializeAbilities();
        }

        private void InitializeStats()
        {
            if (statsDefinition == null) return;

            _entityStats.RegisterStat(EntityStatType.MaxHealth, statsDefinition.MaxHealth, 0);
            _entityStats.RegisterStat(EntityStatType.MaxMana, statsDefinition.MaxMana, 0);
            _entityStats.RegisterStat(EntityStatType.MaxEnergy, statsDefinition.MaxEnergy, 0);
            _entityStats.RegisterStat(EntityStatType.HealthRegen, statsDefinition.HealthRegen);
            _entityStats.RegisterStat(EntityStatType.ManaRegen, statsDefinition.ManaRegen);
            _entityStats.RegisterStat(EntityStatType.AttackPower, statsDefinition.AttackPower, 0);
            _entityStats.RegisterStat(EntityStatType.SpellPower, statsDefinition.SpellPower, 0);
            _entityStats.RegisterStat(EntityStatType.CriticalChance, statsDefinition.CriticalChance, 0, 1);
            _entityStats.RegisterStat(EntityStatType.CriticalDamage, statsDefinition.CriticalDamage, 0);
            _entityStats.RegisterStat(EntityStatType.Armor, statsDefinition.Armor, 0);
            _entityStats.RegisterStat(EntityStatType.MagicResist, statsDefinition.MagicResist, 0);
            _entityStats.RegisterStat(EntityStatType.CooldownReduction, statsDefinition.CooldownReduction, 0, 0.8f);
            _entityStats.RegisterStat(EntityStatType.FireResist, statsDefinition.FireResist);
            _entityStats.RegisterStat(EntityStatType.IceResist, statsDefinition.IceResist);
            _entityStats.RegisterStat(EntityStatType.LightningResist, statsDefinition.LightningResist);
        }

        private void InitializeResources()
        {
            if (statsDefinition == null) return;

            var health = new AbilityCostPool(AbilityCostType.Health, statsDefinition.MaxHealth);
            health.RegenRate = statsDefinition.HealthRegen;
            health.OnValueChanged += (old, current, max) => 
            {
                _health.Value = current;
                if (current <= 0 && _isAlive) Die(null);
            };
            _resources[AbilityCostType.Health] = health;
            _health.Value = health.CurrentValue;

            var mana = new AbilityCostPool(AbilityCostType.Mana, statsDefinition.MaxMana);
            mana.RegenRate = statsDefinition.ManaRegen;
            mana.OnValueChanged += (_, current, __) => _mana.Value = current;
            _resources[AbilityCostType.Mana] = mana;
            _mana.Value = mana.CurrentValue;

            _resources[AbilityCostType.Energy] = new AbilityCostPool(AbilityCostType.Energy, statsDefinition.MaxEnergy);
        }

        private void InitializeAbilities()
        {
            if (startingAbilities == null || _abilityFactory == null) return;
            foreach (var def in startingAbilities)
                if (def != null) _abilities.Add(_abilityFactory.CreateAbility(def));
        }

        private void Update()
        {
            // Don't regen if dead!
            if (_isAlive)
            {
                foreach (var resource in _resources.Values) 
                    resource.Tick(Time.deltaTime);
            }
            _entityStats.ClearExpiredModifiers();
        }

        public bool HasResource(AbilityCostType type, float amount) =>
            _resources.TryGetValue(type, out var pool) && pool.HasEnough(amount);

        public void ConsumeResource(AbilityCostType type, float amount)
        {
            if (_resources.TryGetValue(type, out var pool)) pool.Consume(amount);
        }

        public float GetResource(AbilityCostType type) =>
            _resources.TryGetValue(type, out var pool) ? pool.CurrentValue : 0;

        public float GetMaxResource(AbilityCostType type) =>
            _resources.TryGetValue(type, out var pool) ? pool.MaxValue : 0;

        public void ModifyResource(AbilityCostType type, float delta)
        {
            if (_resources.TryGetValue(type, out var pool)) pool.Modify(delta);
        }

        public void AddAbility(IAbility ability)
        {
            if (_abilities.All(a => a.Id != ability.Id))
            {
                _abilities.Add(ability);
            }
        }

        public void RemoveAbility(AbilityId id) => _abilities.RemoveAll(a => a.Id == id);
        public IAbility GetAbility(AbilityId id) => _abilities.FirstOrDefault(a => a.Id == id);
        public bool HasAbility(AbilityId id) => _abilities.Any(a => a.Id == id);

        public float GetStat(string statId) => _entityStats.GetStatValue(statId);

        public void ModifyStat(string statId, float delta)
        {
            if (statId == EntityStatType.MaxHealth) { ModifyResource(AbilityCostType.Health, delta); return; }
            _entityStats.ModifyBaseValue(statId, delta);
        }

        public void SetStat(string statId, float value) => _entityStats.SetBaseValue(statId, value);
        public void ApplyModifier(IModifier modifier) => _entityStats.AddModifier(modifier);
        public void RemoveModifier(IModifier modifier) => _entityStats.RemoveModifier(modifier);

        public bool HasModifier(string modifierId)
        {
            foreach (var statId in _entityStats.GetAllStatIds())
                if (_entityStats.GetModifiers(statId).Any(m => m.Id == modifierId)) return true;
            return false;
        }

        public void Die(IAbilityOwner killer)
        {
            _isAlive = false;
            _alive.Value = false;
            OnDeath?.Invoke(this);
            Debug.Log($"<color=red>💀 {entityName} died!</color>");
        }

        public void Revive(float healthPercent = 1f)
        {
            _isAlive = true;
            _alive.Value = true;
            if (_resources.TryGetValue(AbilityCostType.Health, out var health))
                health.Restore(health.MaxValue * healthPercent);
            Debug.Log($"<color=green>✨ {entityName} revived!</color>");
        }

        private void OnDestroy()
        {
            _health.Dispose();
            _mana.Dispose();
            _alive.Dispose();
        }
    }
}

using UnityEngine;
using VContainer;
using VContainer.Unity;
using MessagePipe;
using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Combos;
using AbilitySystem.Core.Events;
using AbilitySystem.Core.Runtime;
using AbilitySystem.Gameplay.Casting;
using AbilitySystem.Gameplay.Casting.Conditions;
using AbilitySystem.Gameplay.Services;
using AbilitySystem.Gameplay.Targeting;
using AbilitySystem.Unity.Factories;
using AbilitySystem.Unity.Presenters;

namespace AbilitySystem.Unity.Installers
{
    /// <summary>
    /// VContainer LifetimeScope with MessagePipe pub/sub
    /// </summary>
    public class AbilitySystemLifetimeScope : LifetimeScope
    {
        [SerializeField] private bool autoInjectSceneObjects = true;

        protected override void Configure(IContainerBuilder builder)
        {
            // MessagePipe - register all event types
            MessagePipeOptions options = builder.RegisterMessagePipe();
            RegisterEvents(builder, options);

            // Core Services
            builder.Register<AbilityCooldownManager>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<AbilityEffectApplyService>(Lifetime.Singleton).AsImplementedInterfaces();
            
            // Combo & Elemental Systems
            builder.Register<AbilityComboSystem>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
            builder.Register<AbilityElementalReactionSystem>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

            builder.Register<TargetingService>(Lifetime.Singleton);

            // Casting Pipeline
            builder.Register<AbilityCastingConditionService>(Lifetime.Singleton);

            // Factories
            builder.Register<EffectFactory>(Lifetime.Singleton);
            builder.Register<AbilityFactory>(Lifetime.Singleton);

            // Main Ability System
            builder.Register<AbilityCastCastService>(Lifetime.Singleton).AsImplementedInterfaces();

            // Entry points
            builder.RegisterEntryPoint<CastingPipelineInitializer>();
            builder.RegisterEntryPoint<ComboRegistrar>();
            
            // Auto-inject scene MonoBehaviours
            if (autoInjectSceneObjects)
            {
                builder.RegisterBuildCallback(container =>
                {
                    // Inject into all Entities in scene
                    foreach (Entity entity in FindObjectsByType<Entity>(FindObjectsSortMode.None))
                    {
                        container.Inject(entity);
                    }
                    
                    // Inject into Bootstrapper
                    AbilitySystemBootstrapper bootstrapper = FindFirstObjectByType<AbilitySystemBootstrapper>();
                    if (bootstrapper != null)
                    {
                        container.Inject(bootstrapper);
                    }
                });
            }
        }

        private void RegisterEvents(IContainerBuilder builder, MessagePipeOptions options)
        {
            // Ability Events
            builder.RegisterMessageBroker<AbilityCastStartedEvent>(options);
            builder.RegisterMessageBroker<AbilityCastCompletedEvent>(options);
            builder.RegisterMessageBroker<AbilityCastCancelledEvent>(options);
            builder.RegisterMessageBroker<AbilityCooldownStartedEvent>(options);
            builder.RegisterMessageBroker<AbilityCooldownCompletedEvent>(options);

            // Effect Events
            builder.RegisterMessageBroker<AbilityEffectAppliedEvent>(options);
            builder.RegisterMessageBroker<AbilityEffectRemovedEvent>(options);
            builder.RegisterMessageBroker<DamageDealtEvent>(options);
            builder.RegisterMessageBroker<HealingDealtEvent>(options);

            // Stat Events
            builder.RegisterMessageBroker<StatChangedEvent>(options);
            builder.RegisterMessageBroker<ResourceChangedEvent>(options);
            builder.RegisterMessageBroker<EntityDiedEvent>(options);

            // Combo Events
            builder.RegisterMessageBroker<AbilityComboProgressEvent>(options);
            builder.RegisterMessageBroker<AbilityComboCompletedEvent>(options);
            builder.RegisterMessageBroker<AbilityComboDroppedEvent>(options);
            builder.RegisterMessageBroker<AbilityElementalReactionEvent>(options);
        }
    }

    public class CastingPipelineInitializer : IStartable
    {
        private readonly AbilityCastingConditionService _conditionService;
        private readonly ICooldownManager _cooldownManager;

        public CastingPipelineInitializer(AbilityCastingConditionService conditionService, ICooldownManager cooldownManager)
        {
            _conditionService = conditionService;
            _cooldownManager = cooldownManager;
        }

        public void Start()
        {
            _conditionService.AddGlobalCondition(new AliveAbilityCondition());
            _conditionService.AddGlobalCondition(new CooldownAbilityCondition(_cooldownManager));
            _conditionService.AddGlobalCondition(new ResourceAbilityCondition());
            _conditionService.AddGlobalCondition(new RangeAbilityCondition());
            
            Debug.Log("<color=green>✅ CastingPipeline initialized with conditions</color>");
        }
    }

    public class ComboRegistrar : IStartable
    {
        private readonly AbilityComboSystem _abilityComboSystem;

        public ComboRegistrar(AbilityComboSystem abilityComboSystem)
        {
            _abilityComboSystem = abilityComboSystem;
        }

        public void Start()
        {
            _abilityComboSystem.RegisterCombo(new ComboDefinition(
                "elemental_storm", "Elemental Storm",
                new AbilityId[] { new AbilityId("lightning_bolt"), new AbilityId("fireball"), new AbilityId("frost_nova") },
                new float[] { 1.0f, 1.3f, 1.8f }, timeWindow: 4f));

            _abilityComboSystem.RegisterCombo(new ComboDefinition(
                "chain_lightning", "Chain Lightning",
                new AbilityId[] { new AbilityId("lightning_bolt"), new AbilityId("lightning_bolt") },
                new float[] { 1.0f, 1.5f }, timeWindow: 2f));

            _abilityComboSystem.RegisterCombo(new ComboDefinition(
                "meteor_combo", "Meteor Storm",
                new AbilityId[] { new AbilityId("fireball"), new AbilityId("fireball"), new AbilityId("fireball") },
                new float[] { 1.0f, 1.2f, 2.0f }, timeWindow: 5f));

            _abilityComboSystem.RegisterCombo(new ComboDefinition(
                "last_stand", "Last Stand",
                new AbilityId[] { new AbilityId("flash_heal"), new AbilityId("battle_cry") },
                new float[] { 1.5f, 1.0f }, timeWindow: 3f));

            Debug.Log("<color=green>✅ Combo system initialized with 4 combos</color>");
        }
    }
}

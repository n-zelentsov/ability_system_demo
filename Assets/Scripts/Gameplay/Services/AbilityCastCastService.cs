using System.Collections.Generic;
using MessagePipe;
using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Combos;
using AbilitySystem.Core.Conditions;
using AbilitySystem.Core.Effects;
using AbilitySystem.Core.Events;
using AbilitySystem.Core.Runtime;
using AbilitySystem.Gameplay.Casting;
using AbilitySystem.Gameplay.Targeting;

namespace AbilitySystem.Gameplay.Services
{
    /// <summary>
    /// Main ability system implementation with combo support
    /// </summary>
    public sealed class AbilityCastCastService : IAbilityCastSystem
    {
        private readonly ICooldownManager _cooldownManager;
        private readonly IEffectProcessor _effectProcessor;
        private readonly AbilityCastingConditionService _abilityCastingConditionService;
        private readonly TargetingService _targetingService;
        private readonly IComboSystem _comboSystem;
        private readonly IElementalReactionSystem _elementalSystem;
        
        private readonly IPublisher<AbilityCastStartedEvent> _castStartedPublisher;
        private readonly IPublisher<AbilityCastCompletedEvent> _castCompletedPublisher;

        public AbilityCastCastService(
            ICooldownManager cooldownManager,
            IEffectProcessor effectProcessor,
            AbilityCastingConditionService abilityCastingConditionService,
            TargetingService targetingService,
            IComboSystem comboSystem,
            IElementalReactionSystem elementalSystem,
            IPublisher<AbilityCastStartedEvent> castStartedPublisher,
            IPublisher<AbilityCastCompletedEvent> castCompletedPublisher)
        {
            _cooldownManager = cooldownManager;
            _effectProcessor = effectProcessor;
            _abilityCastingConditionService = abilityCastingConditionService;
            _targetingService = targetingService;
            _comboSystem = comboSystem;
            _elementalSystem = elementalSystem;
            _castStartedPublisher = castStartedPublisher;
            _castCompletedPublisher = castCompletedPublisher;
        }

        public AbilityCastResult TryCast(IAbilityOwner caster, AbilityId abilityId, AbilityCastContext context)
        {
            IAbility ability = caster.GetAbility(abilityId);
            if (ability == null)
            {
                return AbilityCastResult.Failed($"Ability {abilityId} not found");
            }

            AbilityCastValidationResult validation = _abilityCastingConditionService.Validate(caster, ability, context);
            if (!validation.IsValid)
            {
                return AbilityCastResult.Failed(validation.FailureReason);
            }

            float comboMultiplier = 1f;
            if (_comboSystem != null)
            {
                ComboResult comboResult = _comboSystem.CheckCombo(caster, abilityId);
                if (comboResult.IsCombo)
                {
                    comboMultiplier = comboResult.DamageMultiplier;
                }
            }

            _castStartedPublisher.Publish(new AbilityCastStartedEvent(caster, abilityId));

            ConsumeResources(caster, ability);

            TargetingRequest targetRequest = new(caster, ability, context.Target, context.TargetPoint, context.Direction);
            IReadOnlyList<IEffectTarget> targets = _targetingService.ResolveTargets(targetRequest);
            AbilityEffectResult[] results = ApplyEffects(caster, ability, targets, context, comboMultiplier);

            _comboSystem?.RegisterAbilityCast(caster, abilityId);

            if (ability.Data.Cooldown > 0)
            {
                float cooldown = CalculateCooldown(caster, ability.Data.Cooldown);
                _cooldownManager.StartCooldown(caster, abilityId, cooldown);
            }

            _castCompletedPublisher.Publish(new AbilityCastCompletedEvent(caster, abilityId));
            return AbilityCastResult.Succeeded(results);
        }

        public bool CanCast(IAbilityOwner caster, AbilityId abilityId, AbilityCastContext context)
        {
            IAbility ability = caster.GetAbility(abilityId);
            return ability != null 
                   && _abilityCastingConditionService.Validate(caster, ability, context).IsValid;
        }

        public float GetCooldownRemaining(IAbilityOwner owner, AbilityId abilityId)
        {
            return _cooldownManager.GetRemainingCooldown(owner, abilityId);
        }

        public bool IsOnCooldown(IAbilityOwner owner, AbilityId abilityId)
        {
            return _cooldownManager.IsOnCooldown(owner, abilityId);
        }

        public void Update(float deltaTime)
        {
            _cooldownManager.Tick(deltaTime);
            _effectProcessor.Tick(deltaTime);
            _comboSystem?.Tick(deltaTime);
            _elementalSystem?.Tick(deltaTime);
        }

        private static void ConsumeResources(IAbilityOwner caster, IAbility ability)
        {
            foreach (AbilityCost cost in ability.Data.ResourceCosts)
            {
                caster.ConsumeResource(cost.Type, cost.Amount);
            }
        }

        private AbilityEffectResult[] ApplyEffects(IAbilityOwner caster, IAbility ability, 
            IReadOnlyCollection<IEffectTarget> targets, AbilityCastContext abilityCastContext, float comboMultiplier)
        {
            List<AbilityEffectResult> results = new();

            foreach (IAbilityEffect effect in ability.Effects)
            {
                if (targets.Count == 0)
                {
                    AbilityEffectContext context = new(caster, caster, ability, abilityCastContext.TargetPoint, abilityCastContext.Direction, comboMultiplier);
                    results.Add(ProcessEffect(effect, context));
                }
                else
                {
                    foreach (IEffectTarget target in targets)
                    {
                        AbilityEffectContext context = new(caster, target, ability, abilityCastContext.TargetPoint, abilityCastContext.Direction, comboMultiplier);
                        results.Add(ProcessEffect(effect, context));
                    }
                }
            }

            return results.ToArray();
        }

        private AbilityEffectResult ProcessEffect(IAbilityEffect abilityEffect, AbilityEffectContext context)
        {
            if (abilityEffect is IAbilityDurationAbilityEffect durationEffect)
            {
                _effectProcessor.ApplyDurationEffect(durationEffect, context.Target, context);
                return AbilityEffectResult.Succeeded();
            }
            return _effectProcessor.ApplyEffect(abilityEffect, context);
        }

        private static float CalculateCooldown(IEffectTarget caster, float baseCooldown)
        {
            float cdr = caster.GetStat("cooldown_reduction");
            return baseCooldown * (1f - cdr);
        }
    }
}

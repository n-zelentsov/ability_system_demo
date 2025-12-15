using System;
using UnityEngine;
using VContainer;
using MessagePipe;
using AbilitySystem.Core.Combos;
using AbilitySystem.Core.Events;
using AbilitySystem.Core.Runtime;
using AbilitySystem.Unity.Factories;

namespace AbilitySystem.Unity.Presenters
{
    /// <summary>
    /// Bootstrapper with MessagePipe subscriptions
    /// </summary>
    public class AbilitySystemBootstrapper : MonoBehaviour
    {
        private IAbilityCastSystem _abilityCastSystem;
        private AbilityFactory _abilityFactory;
        private IDisposable _subscriptions;

        [Inject]
        public void Construct(
            IAbilityCastSystem abilityCastSystem,
            AbilityFactory abilityFactory,
            ISubscriber<DamageDealtEvent> damageSubscriber,
            ISubscriber<HealingDealtEvent> healSubscriber,
            ISubscriber<AbilityCastStartedEvent> castSubscriber,
            ISubscriber<AbilityCooldownStartedEvent> cooldownSubscriber,
            ISubscriber<AbilityComboProgressEvent> comboProgressSubscriber,
            ISubscriber<AbilityComboCompletedEvent> comboCompletedSubscriber,
            ISubscriber<AbilityElementalReactionEvent> reactionSubscriber)
        {
            _abilityCastSystem = abilityCastSystem;
            _abilityFactory = abilityFactory;

            var bag = DisposableBag.CreateBuilder();
            
            damageSubscriber.Subscribe(OnDamageDealt).AddTo(bag);
            healSubscriber.Subscribe(OnHealingDealt).AddTo(bag);
            castSubscriber.Subscribe(OnAbilityCast).AddTo(bag);
            cooldownSubscriber.Subscribe(OnCooldownStarted).AddTo(bag);
            comboProgressSubscriber.Subscribe(OnComboProgress).AddTo(bag);
            comboCompletedSubscriber.Subscribe(OnComboCompleted).AddTo(bag);
            reactionSubscriber.Subscribe(OnElementalReaction).AddTo(bag);

            _subscriptions = bag.Build();
        }

        private void OnDamageDealt(DamageDealtEvent e)
        {
            string crit = e.IsCritical ? " <color=orange>CRIT!</color>" : "";
            Debug.Log($"<color=red>⚔️ {e.Source?.Name} → {e.Target?.Name}: {e.Amount:F0} {e.DamageType}{crit}</color>");
        }

        private void OnHealingDealt(HealingDealtEvent e)
        {
            string crit = e.IsCritical ? " <color=orange>CRIT!</color>" : "";
            Debug.Log($"<color=green>💚 {e.Source?.Name} → {e.Target?.Name}: +{e.Amount:F0}{crit}</color>");
        }

        private void OnAbilityCast(AbilityCastStartedEvent e) =>
            Debug.Log($"<color=yellow>✨ {e.Caster?.Name} casts {e.AbilityId}</color>");

        private void OnCooldownStarted(AbilityCooldownStartedEvent e) =>
            Debug.Log($"<color=cyan>⏱️ {e.AbilityId} cooldown: {e.Duration:F1}s</color>");

        private void OnComboProgress(AbilityComboProgressEvent e) =>
            Debug.Log($"<color=magenta>🔥 COMBO: {e.Combo.Name} [{e.CurrentStep}/{e.TotalSteps}]</color>");

        private void OnComboCompleted(AbilityComboCompletedEvent e) =>
            Debug.Log($"<color=#FF00FF>💥 COMBO COMPLETE: {e.Combo.Name}! x{e.TotalDamageMultiplier:F1}</color>");

        private void OnElementalReaction(AbilityElementalReactionEvent e)
        {
            string color = e.ReactionType switch
            {
                ElementalReactionType.Melt => "#FF6600",
                ElementalReactionType.Overload => "#FF0000",
                ElementalReactionType.Superconduct => "#00FFFF",
                _ => "white"
            };
            Debug.Log($"<color={color}>⚡ {e.ReactionType}!</color>");
        }

        private void Update() => _abilityCastSystem?.Update(Time.deltaTime);

        private void OnDestroy() => _subscriptions?.Dispose();
    }
}

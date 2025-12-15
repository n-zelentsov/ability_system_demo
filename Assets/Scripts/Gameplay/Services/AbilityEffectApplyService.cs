using System.Collections.Generic;
using System.Linq;
using MessagePipe;
using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Effects;
using AbilitySystem.Core.Events;
using AbilitySystem.Core.Runtime;

namespace AbilitySystem.Gameplay.Services
{
    /// <summary>
    /// Processes instant effects and manages duration effects
    /// </summary>
    public sealed class AbilityEffectApplyService : IEffectProcessor
    {
        private readonly Dictionary<string, List<AbilityActiveEffect>> _activeEffects = new();
        
        private readonly IPublisher<AbilityEffectAppliedEvent> _effectAppliedPublisher;
        private readonly IPublisher<AbilityEffectRemovedEvent> _effectRemovedPublisher;

        public AbilityEffectApplyService(
            IPublisher<AbilityEffectAppliedEvent> effectAppliedPublisher,
            IPublisher<AbilityEffectRemovedEvent> effectRemovedPublisher)
        {
            _effectAppliedPublisher = effectAppliedPublisher;
            _effectRemovedPublisher = effectRemovedPublisher;
        }

        AbilityEffectResult IEffectProcessor.ApplyEffect(IAbilityEffect abilityEffect, AbilityEffectContext context)
        {
            if (!abilityEffect.CanApply(context))
            {
                return AbilityEffectResult.Failed("Effect conditions not met");
            }

            AbilityEffectResult result = abilityEffect.Apply(context);
            _effectAppliedPublisher.Publish(new AbilityEffectAppliedEvent(context.Target, abilityEffect, result));
            return result;
        }

        void IEffectProcessor.ApplyDurationEffect(IAbilityDurationAbilityEffect abilityEffect, IEffectTarget target, AbilityEffectContext context)
        {
            string targetId = target.Id;
            
            if (!_activeEffects.TryGetValue(targetId, out var effects))
            {
                effects = new List<AbilityActiveEffect>();
                _activeEffects[targetId] = effects;
            }

            AbilityActiveEffect existing = effects.FirstOrDefault(e => e.AbilityEffect.Id == abilityEffect.Id);
            if (existing != null)
            {
                if (abilityEffect.IsStackable && abilityEffect.CurrentStacks < abilityEffect.MaxStacks)
                {
                    abilityEffect.AddStack();
                    abilityEffect.Refresh();
                }
                else
                {
                    abilityEffect.Refresh();
                }
            }
            else
            {
                abilityEffect.OnApply(target);
                effects.Add(new AbilityActiveEffect(abilityEffect, context));
                _effectAppliedPublisher.Publish(new AbilityEffectAppliedEvent(target, abilityEffect, AbilityEffectResult.Succeeded()));
            }
        }

        void IEffectProcessor.RemoveEffect(IAbilityDurationAbilityEffect abilityEffect, IEffectTarget target)
        {
            if (_activeEffects.TryGetValue(target.Id, out List<AbilityActiveEffect> effects))
            {
                AbilityActiveEffect abilityActiveEffect = effects.FirstOrDefault(e => e.AbilityEffect == abilityEffect);
                if (abilityActiveEffect != null)
                {
                    abilityEffect.OnRemove(target);
                    effects.Remove(abilityActiveEffect);
                    
                    _effectRemovedPublisher.Publish(new AbilityEffectRemovedEvent(target, abilityEffect));
                }
            }
        }

        IReadOnlyList<IAbilityDurationAbilityEffect> IEffectProcessor.GetActiveEffects(IEffectTarget target)
        {
            if (_activeEffects.TryGetValue(target.Id, out List<AbilityActiveEffect> effects))
                return effects.Select(e => e.AbilityEffect).ToList();
            return System.Array.Empty<IAbilityDurationAbilityEffect>();
        }

        void IEffectProcessor.Tick(float deltaTime)
        {
            List<(string targetId, AbilityActiveEffect effect)> expiredEffects = new();

            foreach (KeyValuePair<string, List<AbilityActiveEffect>> pair in _activeEffects)
            {
                foreach (AbilityActiveEffect activeEffect in pair.Value)
                {
                    activeEffect.AbilityEffect.Tick(deltaTime);

                    if (activeEffect.AbilityEffect is IPeriodicAbilityEffect periodic)
                        periodic.OnTick(activeEffect.Context);

                    if (activeEffect.AbilityEffect.IsExpired)
                        expiredEffects.Add((pair.Key, activeEffect));
                }
            }

            foreach ((string targetId, AbilityActiveEffect activeEffect) in expiredEffects)
            {
                if (_activeEffects.TryGetValue(targetId, out var effects))
                {
                    effects.Remove(activeEffect);
                    _effectRemovedPublisher.Publish(new AbilityEffectRemovedEvent(null, activeEffect.AbilityEffect));
                }
            }
        }

        void IEffectProcessor.ClearEffects(IEffectTarget target)
        {
            if (_activeEffects.TryGetValue(target.Id, out List<AbilityActiveEffect> effects))
            {
                foreach (AbilityActiveEffect activeEffect in effects)
                {
                    activeEffect.AbilityEffect.OnRemove(target);
                    _effectRemovedPublisher.Publish(new AbilityEffectRemovedEvent(target, activeEffect.AbilityEffect));
                }
                effects.Clear();
            }
        }

        private sealed class AbilityActiveEffect
        {
            public IAbilityDurationAbilityEffect AbilityEffect { get; }
            public AbilityEffectContext Context { get; }

            public AbilityActiveEffect(IAbilityDurationAbilityEffect abilityEffect, AbilityEffectContext context)
            {
                AbilityEffect = abilityEffect;
                Context = context;
            }
        }
    }
}

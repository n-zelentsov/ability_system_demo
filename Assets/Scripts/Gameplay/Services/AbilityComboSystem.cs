using System.Collections.Generic;
using System.Linq;
using MessagePipe;
using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Combos;
using AbilitySystem.Core.Events;

namespace AbilitySystem.Gameplay.Services
{
    /// <summary>
    /// Tracks ability sequences and triggers combos
    /// </summary>
    public sealed class AbilityComboSystem : IComboSystem
    {
        private readonly IPublisher<AbilityComboProgressEvent> _progressPublisher;
        private readonly IPublisher<AbilityComboCompletedEvent> _completedPublisher;
        private readonly IPublisher<AbilityComboDroppedEvent> _droppedPublisher;
        
        private readonly List<ComboDefinition> _combos = new();
        private readonly Dictionary<string, AbilityComboTracker> _activeTrackers = new();

        public AbilityComboSystem(
            IPublisher<AbilityComboProgressEvent> progressPublisher,
            IPublisher<AbilityComboCompletedEvent> completedPublisher,
            IPublisher<AbilityComboDroppedEvent> droppedPublisher)
        {
            _progressPublisher = progressPublisher;
            _completedPublisher = completedPublisher;
            _droppedPublisher = droppedPublisher;
        }

        public void RegisterCombo(ComboDefinition combo)
        {
            if (_combos.Any(c => c.Id == combo.Id))
            {
                return;
            }
            _combos.Add(combo);
        }

        public void RegisterAbilityCast(IAbilityOwner caster, AbilityId abilityId)
        {
            string casterId = caster.Id;
            if (_activeTrackers.TryGetValue(casterId, out var tracker))
            {
                int nextStep = tracker.CurrentStep + 1;
                if (nextStep < tracker.Combo.Sequence.Length && 
                    tracker.Combo.Sequence[nextStep] == abilityId)
                {
                    tracker.CurrentStep = nextStep;
                    tracker.TimeRemaining = tracker.Combo.TimeWindow;

                    _progressPublisher.Publish(new AbilityComboProgressEvent(
                        caster, tracker.Combo, nextStep + 1, tracker.Combo.Sequence.Length));

                    if (nextStep == tracker.Combo.Sequence.Length - 1)
                    {
                        float totalMultiplier = 1f;
                        foreach (float mult in tracker.Combo.StepMultipliers)
                        {
                            totalMultiplier *= mult;
                        }
                        _completedPublisher.Publish(new AbilityComboCompletedEvent(caster, tracker.Combo, totalMultiplier));
                        _activeTrackers.Remove(casterId);
                    }
                }
                else
                {
                    _droppedPublisher.Publish(new AbilityComboDroppedEvent(caster, tracker.Combo, "Wrong ability"));
                    _activeTrackers.Remove(casterId);
                    TryStartNewCombo(caster, abilityId);
                }
            }
            else
            {
                TryStartNewCombo(caster, abilityId);
            }
        }

        private void TryStartNewCombo(IAbilityOwner caster, AbilityId abilityId)
        {
            foreach (ComboDefinition combo in _combos)
            {
                if (combo.Sequence.Length > 0 && combo.Sequence[0] == abilityId)
                {
                    _activeTrackers[caster.Id] = new AbilityComboTracker(combo);
                    _progressPublisher.Publish(new AbilityComboProgressEvent(caster, combo, 1, combo.Sequence.Length));
                    break;
                }
            }
        }

        public ComboResult CheckCombo(IAbilityOwner caster, AbilityId abilityId)
        {
            if (!_activeTrackers.TryGetValue(caster.Id, out AbilityComboTracker tracker))
            {
                return ComboResult.None;
            }

            int nextStep = tracker.CurrentStep + 1;
            if (nextStep < tracker.Combo.Sequence.Length && 
                tracker.Combo.Sequence[nextStep] == abilityId)
            {
                float multiplier = tracker.Combo.StepMultipliers.Length > nextStep 
                    ? tracker.Combo.StepMultipliers[nextStep] : 1f;

                string[] bonusEffects = nextStep == tracker.Combo.Sequence.Length - 1 
                    ? tracker.Combo.BonusEffectIds : null;

                return ComboResult.Success(tracker.Combo, nextStep + 1, multiplier, bonusEffects);
            }

            return ComboResult.None;
        }

        public IReadOnlyList<ComboDefinition> GetAvailableCombos(IAbilityOwner caster)
        {
            if (_activeTrackers.TryGetValue(caster.Id, out AbilityComboTracker tracker))
            {
                return new[] {tracker.Combo};
            }
            return _combos.Where(c => c.Sequence.Length > 0 && caster.HasAbility(c.Sequence[0])).ToList();
        }

        public void Tick(float deltaTime)
        {
            List<string> expiredTrackers = new();
            foreach (KeyValuePair<string, AbilityComboTracker> pair in _activeTrackers)
            {
                pair.Value.TimeRemaining -= deltaTime;
                if (pair.Value.TimeRemaining <= 0)
                {
                    expiredTrackers.Add(pair.Key);
                }
            }
            foreach (string id in expiredTrackers)
            {
                AbilityComboTracker tracker = _activeTrackers[id];
                _activeTrackers.Remove(id);
                _droppedPublisher.Publish(new AbilityComboDroppedEvent(null, tracker.Combo, "Timeout"));
            }
        }

        private sealed class AbilityComboTracker
        {
            public ComboDefinition Combo { get; }
            public int CurrentStep { get; set; }
            public float TimeRemaining { get; set; }

            public AbilityComboTracker(ComboDefinition combo)
            {
                Combo = combo;
                CurrentStep = 0;
                TimeRemaining = combo.TimeWindow;
            }
        }
    }
}

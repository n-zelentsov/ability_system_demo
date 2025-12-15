using System.Collections.Generic;
using MessagePipe;
using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Events;
using AbilitySystem.Core.Runtime;

namespace AbilitySystem.Gameplay.Services
{
    /// <summary>
    /// Manages ability cooldowns for all entities
    /// </summary>
    public sealed class AbilityCooldownManager : ICooldownManager
    {
        private readonly Dictionary<string, Dictionary<AbilityId, AbilityCooldownEntry>> _cooldowns = new();
        
        private readonly IPublisher<AbilityCooldownStartedEvent> _cooldownStartedPublisher;
        private readonly IPublisher<AbilityCooldownCompletedEvent> _cooldownCompletedPublisher;

        public AbilityCooldownManager(
            IPublisher<AbilityCooldownStartedEvent> cooldownStartedPublisher,
            IPublisher<AbilityCooldownCompletedEvent> cooldownCompletedPublisher)
        {
            _cooldownStartedPublisher = cooldownStartedPublisher;
            _cooldownCompletedPublisher = cooldownCompletedPublisher;
        }

        void ICooldownManager.StartCooldown(IAbilityOwner owner, AbilityId abilityId, float duration)
        {
            if (duration <= 0)
            {
                return;
            }

            string ownerId = owner.Id;
            if (!_cooldowns.TryGetValue(ownerId, out Dictionary<AbilityId, AbilityCooldownEntry> ownerCooldowns))
            {
                ownerCooldowns = new Dictionary<AbilityId, AbilityCooldownEntry>();
                _cooldowns[ownerId] = ownerCooldowns;
            }

            ownerCooldowns[abilityId] = new AbilityCooldownEntry(duration, duration);
            _cooldownStartedPublisher.Publish(new AbilityCooldownStartedEvent(owner, abilityId, duration));
        }

        float ICooldownManager.GetRemainingCooldown(IAbilityOwner owner, AbilityId abilityId)
        {
            if (!_cooldowns.TryGetValue(owner.Id, out Dictionary<AbilityId, AbilityCooldownEntry> ownerCooldowns))
            {
                return 0f;
            }
            return ownerCooldowns.TryGetValue(abilityId, out AbilityCooldownEntry entry) ? entry.Remaining : 0f;
        }

        bool ICooldownManager.IsOnCooldown(IAbilityOwner owner, AbilityId abilityId)
        {
            return ((ICooldownManager) this).GetRemainingCooldown(owner, abilityId) > 0;
        }

        void ICooldownManager.ResetCooldown(IAbilityOwner owner, AbilityId abilityId)
        {
            if (_cooldowns.TryGetValue(owner.Id, out Dictionary<AbilityId, AbilityCooldownEntry> ownerCooldowns))
            {
                if (ownerCooldowns.Remove(abilityId))
                    _cooldownCompletedPublisher.Publish(new AbilityCooldownCompletedEvent(owner, abilityId));
            }
        }

        void ICooldownManager.ReduceCooldown(IAbilityOwner owner, AbilityId abilityId, float amount)
        {
            if (_cooldowns.TryGetValue(owner.Id, out Dictionary<AbilityId, AbilityCooldownEntry> ownerCooldowns))
            {
                if (ownerCooldowns.TryGetValue(abilityId, out AbilityCooldownEntry entry))
                {
                    entry.Remaining -= amount;
                    if (entry.Remaining <= 0)
                    {
                        ownerCooldowns.Remove(abilityId);
                        _cooldownCompletedPublisher.Publish(new AbilityCooldownCompletedEvent(owner, abilityId));
                    }
                }
            }
        }

        void ICooldownManager.Tick(float deltaTime)
        {
            List<(string ownerId, AbilityId abilityId)> completedCooldowns = new();
            foreach (KeyValuePair<string, Dictionary<AbilityId, AbilityCooldownEntry>> ownerPair in _cooldowns)
            {
                foreach (KeyValuePair<AbilityId, AbilityCooldownEntry> cooldownPair in ownerPair.Value)
                {
                    cooldownPair.Value.Remaining -= deltaTime;
                    if (cooldownPair.Value.Remaining <= 0)
                    {
                        completedCooldowns.Add((ownerPair.Key, cooldownPair.Key));
                    }
                }
            }
            foreach ((string ownerId, AbilityId abilityId) in completedCooldowns)
            {
                _cooldowns[ownerId].Remove(abilityId);
            }
        }

        private sealed class AbilityCooldownEntry
        {
            public float TotalDuration { get; }
            public float Remaining { get; set; }

            public AbilityCooldownEntry(float total, float remaining)
            {
                TotalDuration = total;
                Remaining = remaining;
            }
        }
    }
}

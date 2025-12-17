using NUnit.Framework;
using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Events;
using AbilitySystem.Core.Modifiers;
using AbilitySystem.Core.Runtime;
using AbilitySystem.Gameplay.Services;
using MessagePipe;

namespace AbilitySystem.Tests.Gameplay
{
    [TestFixture]
    public class CooldownManagerTests
    {
        private ICooldownManager _cooldownManager;
        private MockPublisher<AbilityCooldownStartedEvent> _startedPublisher;
        private MockPublisher<AbilityCooldownCompletedEvent> _completedPublisher;
        private MockAbilityOwner _owner;

        [SetUp]
        public void SetUp()
        {
            _startedPublisher = new MockPublisher<AbilityCooldownStartedEvent>();
            _completedPublisher = new MockPublisher<AbilityCooldownCompletedEvent>();
            _cooldownManager = new AbilityCooldownManager(_startedPublisher, _completedPublisher);
            _owner = new MockAbilityOwner("player");
        }

        [Test]
        public void StartCooldown_SetsAbilityOnCooldown()
        {
            // Arrange
            AbilityId abilityId = new AbilityId("fireball");

            // Act
            _cooldownManager.StartCooldown(_owner, abilityId, 5f);

            // Assert
            Assert.IsTrue(_cooldownManager.IsOnCooldown(_owner, abilityId));
            Assert.AreEqual(5f, _cooldownManager.GetRemainingCooldown(_owner, abilityId));
        }

        [Test]
        public void StartCooldown_PublishesCooldownStartedEvent()
        {
            // Arrange
            AbilityId abilityId = new AbilityId("frost_nova");

            // Act
            _cooldownManager.StartCooldown(_owner, abilityId, 3f);

            // Assert
            Assert.AreEqual(1, _startedPublisher.PublishCount);
        }

        [Test]
        public void StartCooldown_WithZeroDuration_DoesNotSetCooldown()
        {
            // Arrange
            AbilityId abilityId = new AbilityId("instant_spell");

            // Act
            _cooldownManager.StartCooldown(_owner, abilityId, 0f);

            // Assert
            Assert.IsFalse(_cooldownManager.IsOnCooldown(_owner, abilityId));
        }

        [Test]
        public void Tick_ReducesCooldown()
        {
            // Arrange
            AbilityId abilityId = new AbilityId("fireball");
            _cooldownManager.StartCooldown(_owner, abilityId, 5f);

            // Act
            _cooldownManager.Tick(2f);

            // Assert
            Assert.AreEqual(3f, _cooldownManager.GetRemainingCooldown(_owner, abilityId));
        }

        [Test]
        public void Tick_WhenCooldownExpires_RemovesCooldown()
        {
            // Arrange
            AbilityId abilityId = new AbilityId("fireball");
            _cooldownManager.StartCooldown(_owner, abilityId, 3f);

            // Act
            _cooldownManager.Tick(4f);

            // Assert
            Assert.IsFalse(_cooldownManager.IsOnCooldown(_owner, abilityId));
            Assert.AreEqual(0f, _cooldownManager.GetRemainingCooldown(_owner, abilityId));
        }

        [Test]
        public void ResetCooldown_RemovesCooldown()
        {
            // Arrange
            AbilityId abilityId = new AbilityId("heal");
            _cooldownManager.StartCooldown(_owner, abilityId, 10f);

            // Act
            _cooldownManager.ResetCooldown(_owner, abilityId);

            // Assert
            Assert.IsFalse(_cooldownManager.IsOnCooldown(_owner, abilityId));
        }

        [Test]
        public void ResetCooldown_PublishesCooldownCompletedEvent()
        {
            // Arrange
            AbilityId abilityId = new AbilityId("heal");
            _cooldownManager.StartCooldown(_owner, abilityId, 10f);

            // Act
            _cooldownManager.ResetCooldown(_owner, abilityId);

            // Assert
            Assert.AreEqual(1, _completedPublisher.PublishCount);
        }

        [Test]
        public void ReduceCooldown_ReducesRemainingTime()
        {
            // Arrange
            AbilityId abilityId = new AbilityId("burst");
            _cooldownManager.StartCooldown(_owner, abilityId, 10f);

            // Act
            _cooldownManager.ReduceCooldown(_owner, abilityId, 4f);

            // Assert
            Assert.AreEqual(6f, _cooldownManager.GetRemainingCooldown(_owner, abilityId));
        }

        [Test]
        public void ReduceCooldown_WhenReducesToZero_RemovesCooldown()
        {
            // Arrange
            AbilityId abilityId = new AbilityId("burst");
            _cooldownManager.StartCooldown(_owner, abilityId, 5f);

            // Act
            _cooldownManager.ReduceCooldown(_owner, abilityId, 10f);

            // Assert
            Assert.IsFalse(_cooldownManager.IsOnCooldown(_owner, abilityId));
        }

        [Test]
        public void GetRemainingCooldown_NonExistingOwner_ReturnsZero()
        {
            // Arrange
            MockAbilityOwner otherOwner = new MockAbilityOwner("other");

            // Act
            float remaining = _cooldownManager.GetRemainingCooldown(otherOwner, new AbilityId("test"));

            // Assert
            Assert.AreEqual(0f, remaining);
        }

        [Test]
        public void IsOnCooldown_AbilityNotOnCooldown_ReturnsFalse()
        {
            // Arrange
            AbilityId abilityId = new AbilityId("unknown");

            // Act & Assert
            Assert.IsFalse(_cooldownManager.IsOnCooldown(_owner, abilityId));
        }

        [Test]
        public void MultipleOwners_IndependentCooldowns()
        {
            // Arrange
            MockAbilityOwner player1 = new MockAbilityOwner("player1");
            MockAbilityOwner player2 = new MockAbilityOwner("player2");
            AbilityId abilityId = new AbilityId("shared_ability");

            // Act
            _cooldownManager.StartCooldown(player1, abilityId, 10f);
            _cooldownManager.StartCooldown(player2, abilityId, 5f);

            // Assert
            Assert.AreEqual(10f, _cooldownManager.GetRemainingCooldown(player1, abilityId));
            Assert.AreEqual(5f, _cooldownManager.GetRemainingCooldown(player2, abilityId));
        }

        private class MockPublisher<T> : IPublisher<T>
        {
            public int PublishCount { get; private set; }

            public void Publish(T message)
            {
                PublishCount++;
            }
        }

        private class MockAbilityOwner : IAbilityOwner
        {
            public string Id { get; }
            public string Name => Id;
            public bool IsAlive => true;
            public TeamId Team => TeamId.Player;
            public (float x, float y, float z) Position => (0, 0, 0);
            public System.Collections.Generic.IReadOnlyList<IAbility> Abilities => System.Array.Empty<IAbility>();

            public MockAbilityOwner(string id)
            {
                Id = id;
            }

            public bool HasResource(AbilityCostType type, float amount) => true;
            public void ConsumeResource(AbilityCostType type, float amount) { }
            public float GetResource(AbilityCostType type) => 100f;
            public float GetMaxResource(AbilityCostType type) => 100f;
            public void ModifyResource(AbilityCostType type, float delta) { }
            public void AddAbility(IAbility ability) { }
            public void RemoveAbility(AbilityId abilityId) { }
            public IAbility GetAbility(AbilityId abilityId) => null;
            public bool HasAbility(AbilityId abilityId) => false;
            public float GetStat(string statId) => 0f;
            public void ModifyStat(string statId, float delta) { }
            public void SetStat(string statId, float value) { }
            public void ApplyModifier(IModifier modifier) { }
            public void RemoveModifier(IModifier modifier) { }
            public bool HasModifier(string modifierId) => false;
        }
    }
}


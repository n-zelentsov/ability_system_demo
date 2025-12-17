using System.Collections.Generic;
using NUnit.Framework;
using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Combos;
using AbilitySystem.Core.Events;
using AbilitySystem.Core.Modifiers;
using AbilitySystem.Gameplay.Services;
using MessagePipe;

namespace AbilitySystem.Tests.Gameplay
{
    [TestFixture]
    public class ComboSystemTests
    {
        private AbilityComboSystem _comboSystem;
        private MockPublisher<AbilityComboProgressEvent> _progressPublisher;
        private MockPublisher<AbilityComboCompletedEvent> _completedPublisher;
        private MockPublisher<AbilityComboDroppedEvent> _droppedPublisher;
        private MockAbilityOwner _caster;
        private ComboDefinition _testCombo;

        [SetUp]
        public void SetUp()
        {
            _progressPublisher = new MockPublisher<AbilityComboProgressEvent>();
            _completedPublisher = new MockPublisher<AbilityComboCompletedEvent>();
            _droppedPublisher = new MockPublisher<AbilityComboDroppedEvent>();
            _comboSystem = new AbilityComboSystem(_progressPublisher, _completedPublisher, _droppedPublisher);
            _caster = new MockAbilityOwner("caster");

            _testCombo = new ComboDefinition(
                "test_combo",
                "Test Combo",
                new AbilityId[] { new AbilityId("ability_1"), new AbilityId("ability_2"), new AbilityId("ability_3") },
                new float[] { 1.0f, 1.2f, 1.5f },
                timeWindow: 3f);

            _comboSystem.RegisterCombo(_testCombo);
        }

        [Test]
        public void RegisterCombo_AddsComboToSystem()
        {
            // Arrange
            ComboDefinition newCombo = new ComboDefinition(
                "new_combo", "New Combo",
                new AbilityId[] { new AbilityId("spell_a") },
                new float[] { 1.0f });

            // Act
            _comboSystem.RegisterCombo(newCombo);

            // Assert - combo should be available
            IReadOnlyList<ComboDefinition> combos = _comboSystem.GetAvailableCombos(_caster);
            Assert.IsTrue(combos.Count >= 1);
        }

        [Test]
        public void RegisterCombo_DuplicateCombo_DoesNotAdd()
        {
            // Arrange - same combo registered twice
            int initialCount = _comboSystem.GetAvailableCombos(_caster).Count;

            // Act
            _comboSystem.RegisterCombo(_testCombo);

            // Assert
            Assert.AreEqual(initialCount, _comboSystem.GetAvailableCombos(_caster).Count);
        }

        [Test]
        public void RegisterAbilityCast_FirstAbilityOfCombo_StartsComboTracking()
        {
            // Arrange
            AbilityId firstAbility = new AbilityId("ability_1");

            // Act
            _comboSystem.RegisterAbilityCast(_caster, firstAbility);

            // Assert
            Assert.AreEqual(1, _progressPublisher.PublishCount);
        }

        [Test]
        public void RegisterAbilityCast_SecondAbilityOfCombo_ContinuesCombo()
        {
            // Arrange
            _comboSystem.RegisterAbilityCast(_caster, new AbilityId("ability_1"));

            // Act
            _comboSystem.RegisterAbilityCast(_caster, new AbilityId("ability_2"));

            // Assert
            Assert.AreEqual(2, _progressPublisher.PublishCount);
        }

        [Test]
        public void RegisterAbilityCast_CompleteCombo_PublishesCompletedEvent()
        {
            // Arrange & Act
            _comboSystem.RegisterAbilityCast(_caster, new AbilityId("ability_1"));
            _comboSystem.RegisterAbilityCast(_caster, new AbilityId("ability_2"));
            _comboSystem.RegisterAbilityCast(_caster, new AbilityId("ability_3"));

            // Assert
            Assert.AreEqual(1, _completedPublisher.PublishCount);
        }

        [Test]
        public void RegisterAbilityCast_WrongAbility_DropsCombo()
        {
            // Arrange
            _comboSystem.RegisterAbilityCast(_caster, new AbilityId("ability_1"));

            // Act
            _comboSystem.RegisterAbilityCast(_caster, new AbilityId("wrong_ability"));

            // Assert
            Assert.AreEqual(1, _droppedPublisher.PublishCount);
        }

        [Test]
        public void CheckCombo_NextCorrectAbility_ReturnsComboResult()
        {
            // Arrange
            _comboSystem.RegisterAbilityCast(_caster, new AbilityId("ability_1"));

            // Act
            ComboResult result = _comboSystem.CheckCombo(_caster, new AbilityId("ability_2"));

            // Assert
            Assert.IsTrue(result.IsCombo);
            Assert.AreEqual(_testCombo, result.Combo);
            Assert.AreEqual(2, result.CurrentStep);
            Assert.AreEqual(1.2f, result.DamageMultiplier);
        }

        [Test]
        public void CheckCombo_WrongAbility_ReturnsNone()
        {
            // Arrange
            _comboSystem.RegisterAbilityCast(_caster, new AbilityId("ability_1"));

            // Act
            ComboResult result = _comboSystem.CheckCombo(_caster, new AbilityId("wrong"));

            // Assert
            Assert.IsFalse(result.IsCombo);
        }

        [Test]
        public void CheckCombo_NoActiveCombo_ReturnsNone()
        {
            // Act
            ComboResult result = _comboSystem.CheckCombo(_caster, new AbilityId("ability_1"));

            // Assert
            Assert.IsFalse(result.IsCombo);
        }

        [Test]
        public void Tick_ComboTimeout_DropsCombo()
        {
            // Arrange
            _comboSystem.RegisterAbilityCast(_caster, new AbilityId("ability_1"));

            // Act - simulate time passing beyond window
            _comboSystem.Tick(4f);

            // Assert
            Assert.AreEqual(1, _droppedPublisher.PublishCount);
        }

        [Test]
        public void Tick_WithinTimeWindow_ComboStaysActive()
        {
            // Arrange
            _comboSystem.RegisterAbilityCast(_caster, new AbilityId("ability_1"));

            // Act
            _comboSystem.Tick(1f);
            ComboResult result = _comboSystem.CheckCombo(_caster, new AbilityId("ability_2"));

            // Assert
            Assert.IsTrue(result.IsCombo);
        }

        [Test]
        public void GetAvailableCombos_NoActiveCombo_ReturnsAllAvailable()
        {
            // Act
            IReadOnlyList<ComboDefinition> combos = _comboSystem.GetAvailableCombos(_caster);

            // Assert
            Assert.AreEqual(1, combos.Count);
        }

        [Test]
        public void GetAvailableCombos_ActiveCombo_ReturnsOnlyActiveCombo()
        {
            // Arrange
            _comboSystem.RegisterAbilityCast(_caster, new AbilityId("ability_1"));

            // Act
            IReadOnlyList<ComboDefinition> combos = _comboSystem.GetAvailableCombos(_caster);

            // Assert
            Assert.AreEqual(1, combos.Count);
            Assert.AreEqual(_testCombo, combos[0]);
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
            private readonly List<AbilityId> _abilities = new List<AbilityId>
            {
                new AbilityId("ability_1"),
                new AbilityId("ability_2"),
                new AbilityId("ability_3")
            };

            public string Id { get; }
            public string Name => Id;
            public bool IsAlive => true;
            public TeamId Team => TeamId.Player;
            public (float x, float y, float z) Position => (0, 0, 0);
            public IReadOnlyList<IAbility> Abilities => System.Array.Empty<IAbility>();

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
            public bool HasAbility(AbilityId abilityId) => _abilities.Contains(abilityId);
            public float GetStat(string statId) => 0f;
            public void ModifyStat(string statId, float delta) { }
            public void SetStat(string statId, float value) { }
            public void ApplyModifier(IModifier modifier) { }
            public void RemoveModifier(IModifier modifier) { }
            public bool HasModifier(string modifierId) => false;
        }
    }
}


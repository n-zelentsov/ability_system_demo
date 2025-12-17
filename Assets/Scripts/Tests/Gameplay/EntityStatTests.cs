using NUnit.Framework;
using AbilitySystem.Core.Modifiers;
using AbilitySystem.Gameplay.Stats;

namespace AbilitySystem.Tests.Gameplay
{
    [TestFixture]
    public class EntityStatTests
    {
        [Test]
        public void Constructor_WithBaseValue_SetsBaseValue()
        {
            // Arrange & Act
            EntityStat stat = new EntityStat("test_stat", 100f);

            // Assert
            Assert.AreEqual("test_stat", stat.Id);
            Assert.AreEqual(100f, stat.BaseValue);
            Assert.AreEqual(100f, stat.Value);
        }

        [Test]
        public void Constructor_WithDefaultValue_SetsZero()
        {
            // Arrange & Act
            EntityStat stat = new EntityStat("test_stat");

            // Assert
            Assert.AreEqual(0f, stat.BaseValue);
            Assert.AreEqual(0f, stat.Value);
        }

        [Test]
        public void BaseValue_WhenChanged_UpdatesValue()
        {
            // Arrange
            EntityStat stat = new EntityStat("test_stat", 100f);

            // Act
            stat.BaseValue = 150f;

            // Assert
            Assert.AreEqual(150f, stat.BaseValue);
            Assert.AreEqual(150f, stat.Value);
        }

        [Test]
        public void AddModifier_FlatModifier_AddsToValue()
        {
            // Arrange
            EntityStat stat = new EntityStat("test_stat", 100f);
            IModifier modifier = new TestModifier("mod1", "test_stat", DamageModifierType.Flat, 20f);

            // Act
            stat.AddModifier(modifier);

            // Assert
            Assert.AreEqual(100f, stat.BaseValue);
            Assert.AreEqual(120f, stat.Value);
        }

        [Test]
        public void AddModifier_PercentAddModifier_MultipliesBase()
        {
            // Arrange
            EntityStat stat = new EntityStat("test_stat", 100f);
            IModifier modifier = new TestModifier("mod1", "test_stat", DamageModifierType.PercentAdd, 0.5f);

            // Act
            stat.AddModifier(modifier);

            // Assert
            Assert.AreEqual(150f, stat.Value);
        }

        [Test]
        public void AddModifier_PercentMultiplyModifier_MultipliesFinal()
        {
            // Arrange
            EntityStat stat = new EntityStat("test_stat", 100f);
            IModifier flatMod = new TestModifier("mod1", "test_stat", DamageModifierType.Flat, 50f);
            IModifier percentMod = new TestModifier("mod2", "test_stat", DamageModifierType.PercentMultiply, 0.2f);

            // Act
            stat.AddModifier(flatMod);
            stat.AddModifier(percentMod);

            // Assert - (100 + 50) * 1.2 = 180
            Assert.AreEqual(180f, stat.Value);
        }

        [Test]
        public void AddModifier_OverrideModifier_OverridesValue()
        {
            // Arrange
            EntityStat stat = new EntityStat("test_stat", 100f);
            IModifier modifier = new TestModifier("mod1", "test_stat", DamageModifierType.Override, 999f);

            // Act
            stat.AddModifier(modifier);

            // Assert
            Assert.AreEqual(999f, stat.Value);
        }

        [Test]
        public void RemoveModifier_ExistingModifier_RemovesFromCalculation()
        {
            // Arrange
            EntityStat stat = new EntityStat("test_stat", 100f);
            IModifier modifier = new TestModifier("mod1", "test_stat", DamageModifierType.Flat, 50f);
            stat.AddModifier(modifier);

            // Act
            bool removed = stat.RemoveModifier(modifier);

            // Assert
            Assert.IsTrue(removed);
            Assert.AreEqual(100f, stat.Value);
        }

        [Test]
        public void RemoveModifier_NonExistingModifier_ReturnsFalse()
        {
            // Arrange
            EntityStat stat = new EntityStat("test_stat", 100f);
            IModifier modifier = new TestModifier("mod1", "test_stat", DamageModifierType.Flat, 50f);

            // Act
            bool removed = stat.RemoveModifier(modifier);

            // Assert
            Assert.IsFalse(removed);
        }

        [Test]
        public void ClearModifiers_RemovesAllModifiers()
        {
            // Arrange
            EntityStat stat = new EntityStat("test_stat", 100f);
            stat.AddModifier(new TestModifier("mod1", "test_stat", DamageModifierType.Flat, 10f));
            stat.AddModifier(new TestModifier("mod2", "test_stat", DamageModifierType.Flat, 20f));

            // Act
            stat.ClearModifiers();

            // Assert
            Assert.AreEqual(100f, stat.Value);
            Assert.AreEqual(0, stat.GetModifiers().Count);
        }

        [Test]
        public void MinValue_ClampsResult()
        {
            // Arrange
            EntityStat stat = new EntityStat("test_stat", 100f);
            stat.MinValue = 50f;
            IModifier modifier = new TestModifier("mod1", "test_stat", DamageModifierType.Flat, -100f);

            // Act
            stat.AddModifier(modifier);

            // Assert
            Assert.AreEqual(50f, stat.Value);
        }

        [Test]
        public void MaxValue_ClampsResult()
        {
            // Arrange
            EntityStat stat = new EntityStat("test_stat", 100f);
            stat.MaxValue = 120f;
            IModifier modifier = new TestModifier("mod1", "test_stat", DamageModifierType.Flat, 50f);

            // Act
            stat.AddModifier(modifier);

            // Assert
            Assert.AreEqual(120f, stat.Value);
        }

        [Test]
        public void RemoveModifiersBySource_RemovesAllFromSource()
        {
            // Arrange
            EntityStat stat = new EntityStat("test_stat", 100f);
            stat.AddModifier(new TestModifier("mod1", "test_stat", DamageModifierType.Flat, 10f, "buff_spell"));
            stat.AddModifier(new TestModifier("mod2", "test_stat", DamageModifierType.Flat, 20f, "buff_spell"));
            stat.AddModifier(new TestModifier("mod3", "test_stat", DamageModifierType.Flat, 30f, "other_spell"));

            // Act
            stat.RemoveModifiersBySource("buff_spell");

            // Assert
            Assert.AreEqual(130f, stat.Value); // 100 + 30 from other_spell
        }

        private class TestModifier : IModifier
        {
            public string Id { get; }
            public string SourceId { get; }
            public string TargetStatId { get; }
            public DamageModifierType Type { get; }
            public float Value { get; }
            public int Priority { get; }
            public bool IsExpired => false;

            public TestModifier(string id, string targetStatId, DamageModifierType type, float value, string sourceId = "test")
            {
                Id = id;
                SourceId = sourceId;
                TargetStatId = targetStatId;
                Type = type;
                Value = value;
                Priority = (int)type;
            }

            public float Apply(float baseValue, float currentValue)
            {
                return Type switch
                {
                    DamageModifierType.Flat => currentValue + Value,
                    DamageModifierType.PercentAdd => currentValue + (baseValue * Value),
                    DamageModifierType.PercentMultiply => currentValue * (1 + Value),
                    DamageModifierType.Override => Value,
                    _ => currentValue
                };
            }
        }
    }
}


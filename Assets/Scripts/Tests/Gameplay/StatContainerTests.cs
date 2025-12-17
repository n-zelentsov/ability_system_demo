using System;
using NUnit.Framework;
using AbilitySystem.Core.Modifiers;
using AbilitySystem.Gameplay.Stats;

namespace AbilitySystem.Tests.Gameplay
{
    [TestFixture]
    public class StatContainerTests
    {
        [Test]
        public void RegisterStat_NewStat_AddsToContainer()
        {
            // Arrange
            EntityStatContainer container = new EntityStatContainer();

            // Act
            container.RegisterStat("strength", 10f);

            // Assert
            Assert.IsTrue(container.HasStat("strength"));
            Assert.AreEqual(10f, container.GetStatValue("strength"));
        }

        [Test]
        public void RegisterStat_DuplicateStat_ThrowsException()
        {
            // Arrange
            EntityStatContainer container = new EntityStatContainer();
            container.RegisterStat("strength", 10f);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => container.RegisterStat("strength", 20f));
        }

        [Test]
        public void RegisterStat_WithMinMax_ClampsStat()
        {
            // Arrange
            EntityStatContainer container = new EntityStatContainer();

            // Act
            container.RegisterStat("crit_chance", 0.5f, minValue: 0f, maxValue: 1f);

            // Assert
            Assert.AreEqual(0.5f, container.GetStatValue("crit_chance"));
        }

        [Test]
        public void GetStatValue_NonExistingStat_ReturnsZero()
        {
            // Arrange
            EntityStatContainer container = new EntityStatContainer();

            // Act
            float value = container.GetStatValue("non_existing");

            // Assert
            Assert.AreEqual(0f, value);
        }

        [Test]
        public void GetBaseValue_ReturnsBaseValue()
        {
            // Arrange
            EntityStatContainer container = new EntityStatContainer();
            container.RegisterStat("strength", 10f);

            // Act
            float baseValue = container.GetBaseValue("strength");

            // Assert
            Assert.AreEqual(10f, baseValue);
        }

        [Test]
        public void SetBaseValue_UpdatesStat()
        {
            // Arrange
            EntityStatContainer container = new EntityStatContainer();
            container.RegisterStat("agility", 10f);

            // Act
            container.SetBaseValue("agility", 15f);

            // Assert
            Assert.AreEqual(15f, container.GetStatValue("agility"));
        }

        [Test]
        public void ModifyBaseValue_AddsToDelta()
        {
            // Arrange
            EntityStatContainer container = new EntityStatContainer();
            container.RegisterStat("stamina", 100f);

            // Act
            container.ModifyBaseValue("stamina", 25f);

            // Assert
            Assert.AreEqual(125f, container.GetStatValue("stamina"));
        }

        [Test]
        public void AddModifier_AffectsStatValue()
        {
            // Arrange
            EntityStatContainer container = new EntityStatContainer();
            container.RegisterStat("attack_power", 100f);
            IModifier modifier = new TestModifier("buff", "attack_power", DamageModifierType.Flat, 50f);

            // Act
            container.AddModifier(modifier);

            // Assert
            Assert.AreEqual(150f, container.GetStatValue("attack_power"));
        }

        [Test]
        public void RemoveModifier_RestoresOriginalValue()
        {
            // Arrange
            EntityStatContainer container = new EntityStatContainer();
            container.RegisterStat("spell_power", 100f);
            IModifier modifier = new TestModifier("buff", "spell_power", DamageModifierType.Flat, 30f);
            container.AddModifier(modifier);

            // Act
            container.RemoveModifier(modifier);

            // Assert
            Assert.AreEqual(100f, container.GetStatValue("spell_power"));
        }

        [Test]
        public void RemoveModifiersBySource_RemovesAllFromSource()
        {
            // Arrange
            EntityStatContainer container = new EntityStatContainer();
            container.RegisterStat("armor", 50f);
            container.RegisterStat("magic_resist", 30f);
            container.AddModifier(new TestModifier("mod1", "armor", DamageModifierType.Flat, 10f, "buff_spell"));
            container.AddModifier(new TestModifier("mod2", "magic_resist", DamageModifierType.Flat, 5f, "buff_spell"));
            container.AddModifier(new TestModifier("mod3", "armor", DamageModifierType.Flat, 20f, "other_spell"));

            // Act
            container.RemoveModifiersBySource("buff_spell");

            // Assert
            Assert.AreEqual(70f, container.GetStatValue("armor")); // 50 + 20
            Assert.AreEqual(30f, container.GetStatValue("magic_resist")); // Original
        }

        [Test]
        public void OnStatChanged_FiresWhenStatChanges()
        {
            // Arrange
            EntityStatContainer container = new EntityStatContainer();
            container.RegisterStat("health", 100f);
            string changedStatId = null;
            float oldVal = 0f;
            float newVal = 0f;
            container.OnStatChanged += (id, old, current) =>
            {
                changedStatId = id;
                oldVal = old;
                newVal = current;
            };

            // Act
            container.SetBaseValue("health", 80f);

            // Assert
            Assert.AreEqual("health", changedStatId);
            Assert.AreEqual(100f, oldVal);
            Assert.AreEqual(80f, newVal);
        }

        [Test]
        public void GetModifiers_ReturnsModifiersForStat()
        {
            // Arrange
            EntityStatContainer container = new EntityStatContainer();
            container.RegisterStat("damage", 100f);
            IModifier mod1 = new TestModifier("mod1", "damage", DamageModifierType.Flat, 10f);
            IModifier mod2 = new TestModifier("mod2", "damage", DamageModifierType.PercentAdd, 0.1f);
            container.AddModifier(mod1);
            container.AddModifier(mod2);

            // Act
            int count = 0;
            foreach (IModifier mod in container.GetModifiers("damage"))
            {
                count++;
            }

            // Assert
            Assert.AreEqual(2, count);
        }

        [Test]
        public void GetAllStatIds_ReturnsAllRegisteredStats()
        {
            // Arrange
            EntityStatContainer container = new EntityStatContainer();
            container.RegisterStat("str", 10f);
            container.RegisterStat("agi", 15f);
            container.RegisterStat("int", 20f);

            // Act
            int count = 0;
            foreach (string id in container.GetAllStatIds())
            {
                count++;
            }

            // Assert
            Assert.AreEqual(3, count);
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
                return currentValue;
            }
        }
    }
}


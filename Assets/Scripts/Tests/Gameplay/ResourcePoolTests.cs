using NUnit.Framework;
using AbilitySystem.Core.Events;
using AbilitySystem.Gameplay.Stats;

namespace AbilitySystem.Tests.Gameplay
{
    [TestFixture]
    public class ResourcePoolTests
    {
        [Test]
        public void Constructor_WithMaxValue_InitializesFull()
        {
            // Arrange & Act
            AbilityCostPool pool = new AbilityCostPool(AbilityCostType.Health, 100f);

            // Assert
            Assert.AreEqual(AbilityCostType.Health, pool.Type);
            Assert.AreEqual(100f, pool.MaxValue);
            Assert.AreEqual(100f, pool.CurrentValue);
            Assert.AreEqual(1f, pool.Percentage);
            Assert.IsTrue(pool.IsFull);
            Assert.IsFalse(pool.IsEmpty);
        }

        [Test]
        public void Constructor_WithStartValue_InitializesAtStart()
        {
            // Arrange & Act
            AbilityCostPool pool = new AbilityCostPool(AbilityCostType.Mana, 100f, startValue: 50f);

            // Assert
            Assert.AreEqual(50f, pool.CurrentValue);
            Assert.AreEqual(0.5f, pool.Percentage);
        }

        [Test]
        public void HasEnough_WhenHasEnough_ReturnsTrue()
        {
            // Arrange
            AbilityCostPool pool = new AbilityCostPool(AbilityCostType.Mana, 100f);

            // Act & Assert
            Assert.IsTrue(pool.HasEnough(50f));
            Assert.IsTrue(pool.HasEnough(100f));
        }

        [Test]
        public void HasEnough_WhenNotEnough_ReturnsFalse()
        {
            // Arrange
            AbilityCostPool pool = new AbilityCostPool(AbilityCostType.Mana, 100f);

            // Act & Assert
            Assert.IsFalse(pool.HasEnough(150f));
        }

        [Test]
        public void TryConsume_WhenHasEnough_ConsumesAndReturnsTrue()
        {
            // Arrange
            AbilityCostPool pool = new AbilityCostPool(AbilityCostType.Mana, 100f);

            // Act
            bool result = pool.TryConsume(30f);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(70f, pool.CurrentValue);
        }

        [Test]
        public void TryConsume_WhenNotEnough_ReturnsFalseAndDoesNotConsume()
        {
            // Arrange
            AbilityCostPool pool = new AbilityCostPool(AbilityCostType.Mana, 100f, startValue: 20f);

            // Act
            bool result = pool.TryConsume(30f);

            // Assert
            Assert.IsFalse(result);
            Assert.AreEqual(20f, pool.CurrentValue);
        }

        [Test]
        public void Consume_ReducesCurrentValue()
        {
            // Arrange
            AbilityCostPool pool = new AbilityCostPool(AbilityCostType.Energy, 100f);

            // Act
            pool.Consume(25f);

            // Assert
            Assert.AreEqual(75f, pool.CurrentValue);
        }

        [Test]
        public void Consume_DoesNotGoBelowZero()
        {
            // Arrange
            AbilityCostPool pool = new AbilityCostPool(AbilityCostType.Health, 100f);

            // Act
            pool.Consume(150f);

            // Assert
            Assert.AreEqual(0f, pool.CurrentValue);
            Assert.IsTrue(pool.IsEmpty);
        }

        [Test]
        public void Restore_IncreasesCurrentValue()
        {
            // Arrange
            AbilityCostPool pool = new AbilityCostPool(AbilityCostType.Mana, 100f, startValue: 50f);

            // Act
            pool.Restore(30f);

            // Assert
            Assert.AreEqual(80f, pool.CurrentValue);
        }

        [Test]
        public void Restore_DoesNotExceedMax()
        {
            // Arrange
            AbilityCostPool pool = new AbilityCostPool(AbilityCostType.Health, 100f, startValue: 80f);

            // Act
            pool.Restore(50f);

            // Assert
            Assert.AreEqual(100f, pool.CurrentValue);
            Assert.IsTrue(pool.IsFull);
        }

        [Test]
        public void Modify_PositiveDelta_Restores()
        {
            // Arrange
            AbilityCostPool pool = new AbilityCostPool(AbilityCostType.Health, 100f, startValue: 50f);

            // Act
            pool.Modify(20f);

            // Assert
            Assert.AreEqual(70f, pool.CurrentValue);
        }

        [Test]
        public void Modify_NegativeDelta_Consumes()
        {
            // Arrange
            AbilityCostPool pool = new AbilityCostPool(AbilityCostType.Mana, 100f);

            // Act
            pool.Modify(-40f);

            // Assert
            Assert.AreEqual(60f, pool.CurrentValue);
        }

        [Test]
        public void SetToMax_SetsCurrentToMax()
        {
            // Arrange
            AbilityCostPool pool = new AbilityCostPool(AbilityCostType.Energy, 100f, startValue: 10f);

            // Act
            pool.SetToMax();

            // Assert
            Assert.AreEqual(100f, pool.CurrentValue);
        }

        [Test]
        public void SetToZero_SetsCurrentToZero()
        {
            // Arrange
            AbilityCostPool pool = new AbilityCostPool(AbilityCostType.Health, 100f);

            // Act
            pool.SetToZero();

            // Assert
            Assert.AreEqual(0f, pool.CurrentValue);
        }

        [Test]
        public void Tick_WithRegen_RegeneratesResource()
        {
            // Arrange
            AbilityCostPool pool = new AbilityCostPool(AbilityCostType.Health, 100f, startValue: 50f);
            pool.RegenRate = 10f; // 10 per second

            // Act
            pool.Tick(0.5f); // Half second

            // Assert
            Assert.AreEqual(55f, pool.CurrentValue);
        }

        [Test]
        public void Tick_WhenFull_DoesNotRegen()
        {
            // Arrange
            AbilityCostPool pool = new AbilityCostPool(AbilityCostType.Mana, 100f);
            pool.RegenRate = 10f;

            // Act
            pool.Tick(1f);

            // Assert
            Assert.AreEqual(100f, pool.CurrentValue);
        }

        [Test]
        public void OnValueChanged_FiresWhenValueChanges()
        {
            // Arrange
            AbilityCostPool pool = new AbilityCostPool(AbilityCostType.Health, 100f);
            float oldValue = 0f;
            float newValue = 0f;
            float maxValue = 0f;
            pool.OnValueChanged += (old, current, max) =>
            {
                oldValue = old;
                newValue = current;
                maxValue = max;
            };

            // Act
            pool.Consume(30f);

            // Assert
            Assert.AreEqual(100f, oldValue);
            Assert.AreEqual(70f, newValue);
            Assert.AreEqual(100f, maxValue);
        }

        [Test]
        public void MaxValue_WhenReduced_ClampsCurrentValue()
        {
            // Arrange
            AbilityCostPool pool = new AbilityCostPool(AbilityCostType.Health, 100f);

            // Act
            pool.MaxValue = 50f;

            // Assert
            Assert.AreEqual(50f, pool.CurrentValue);
        }
    }
}


using NUnit.Framework;
using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Events;

namespace AbilitySystem.Tests.Core
{
    [TestFixture]
    public class AbilityDataTests
    {
        [Test]
        public void Constructor_WithDefaults_SetsDefaultValues()
        {
            // Arrange & Act
            AbilityData data = new AbilityData();

            // Assert
            Assert.AreEqual(0f, data.Cooldown);
            Assert.AreEqual(0f, data.CastTime);
            Assert.AreEqual(0f, data.Range);
            Assert.AreEqual(0f, data.AreaRadius);
            Assert.IsNotNull(data.ResourceCosts);
            Assert.AreEqual(0, data.ResourceCosts.Length);
            Assert.IsTrue(data.CanCastWhileMoving);
            Assert.IsFalse(data.IsChanneled);
            Assert.AreEqual(0f, data.ChannelDuration);
            Assert.AreEqual(1, data.MaxCharges);
            Assert.AreEqual(0f, data.ChargeRestoreTime);
        }

        [Test]
        public void Constructor_WithCustomValues_SetsProvidedValues()
        {
            // Arrange
            AbilityCost[] costs = new[]
            {
                new AbilityCost(AbilityCostType.Mana, 50f)
            };

            // Act
            AbilityData data = new AbilityData(
                cooldown: 5f,
                castTime: 1.5f,
                range: 20f,
                areaRadius: 5f,
                resourceCosts: costs,
                canCastWhileMoving: false,
                isChanneled: true,
                channelDuration: 3f,
                maxCharges: 2,
                chargeRestoreTime: 10f);

            // Assert
            Assert.AreEqual(5f, data.Cooldown);
            Assert.AreEqual(1.5f, data.CastTime);
            Assert.AreEqual(20f, data.Range);
            Assert.AreEqual(5f, data.AreaRadius);
            Assert.AreEqual(1, data.ResourceCosts.Length);
            Assert.AreEqual(AbilityCostType.Mana, data.ResourceCosts[0].Type);
            Assert.AreEqual(50f, data.ResourceCosts[0].Amount);
            Assert.IsFalse(data.CanCastWhileMoving);
            Assert.IsTrue(data.IsChanneled);
            Assert.AreEqual(3f, data.ChannelDuration);
            Assert.AreEqual(2, data.MaxCharges);
            Assert.AreEqual(10f, data.ChargeRestoreTime);
        }

        [Test]
        public void Constructor_WithNullResourceCosts_InitializesEmptyArray()
        {
            // Arrange & Act
            AbilityData data = new AbilityData(resourceCosts: null);

            // Assert
            Assert.IsNotNull(data.ResourceCosts);
            Assert.AreEqual(0, data.ResourceCosts.Length);
        }
    }
}


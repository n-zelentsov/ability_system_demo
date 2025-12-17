using System;
using NUnit.Framework;
using AbilitySystem.Core.Abilities;

namespace AbilitySystem.Tests.Core
{
    [TestFixture]
    public class AbilityIdTests
    {
        [Test]
        public void Constructor_WithValidValue_CreatesInstance()
        {
            // Arrange & Act
            AbilityId id = new AbilityId("fireball");

            // Assert
            Assert.AreEqual("fireball", id.Value);
        }

        [Test]
        public void Constructor_WithNullValue_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new AbilityId(null));
        }

        [Test]
        public void Constructor_WithEmptyValue_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new AbilityId(""));
        }

        [Test]
        public void Constructor_WithWhitespaceValue_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new AbilityId("   "));
        }

        [Test]
        public void Equals_WithSameValue_ReturnsTrue()
        {
            // Arrange
            AbilityId id1 = new AbilityId("fireball");
            AbilityId id2 = new AbilityId("fireball");

            // Act & Assert
            Assert.IsTrue(id1.Equals(id2));
            Assert.IsTrue(id1 == id2);
        }

        [Test]
        public void Equals_WithDifferentValue_ReturnsFalse()
        {
            // Arrange
            AbilityId id1 = new AbilityId("fireball");
            AbilityId id2 = new AbilityId("frost_nova");

            // Act & Assert
            Assert.IsFalse(id1.Equals(id2));
            Assert.IsTrue(id1 != id2);
        }

        [Test]
        public void GetHashCode_SameValues_ReturnsSameHash()
        {
            // Arrange
            AbilityId id1 = new AbilityId("lightning_bolt");
            AbilityId id2 = new AbilityId("lightning_bolt");

            // Act & Assert
            Assert.AreEqual(id1.GetHashCode(), id2.GetHashCode());
        }

        [Test]
        public void ToString_ReturnsValue()
        {
            // Arrange
            AbilityId id = new AbilityId("heal");

            // Act & Assert
            Assert.AreEqual("heal", id.ToString());
        }

        [Test]
        public void ImplicitConversion_ToString_Works()
        {
            // Arrange
            AbilityId id = new AbilityId("buff");

            // Act
            string value = id;

            // Assert
            Assert.AreEqual("buff", value);
        }

        [Test]
        public void ExplicitConversion_FromString_Works()
        {
            // Arrange & Act
            AbilityId id = (AbilityId)"debuff";

            // Assert
            Assert.AreEqual("debuff", id.Value);
        }
    }
}


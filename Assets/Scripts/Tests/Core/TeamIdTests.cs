using NUnit.Framework;
using AbilitySystem.Core.Abilities;

namespace AbilitySystem.Tests.Core
{
    [TestFixture]
    public class TeamIdTests
    {
        [Test]
        public void Constructor_WithValue_SetsValue()
        {
            // Arrange & Act
            TeamId teamId = new TeamId(1);

            // Assert
            Assert.AreEqual(1, teamId.Value);
        }

        [Test]
        public void IsAlly_SameTeam_ReturnsTrue()
        {
            // Arrange
            TeamId team1 = new TeamId(1);
            TeamId team2 = new TeamId(1);

            // Act & Assert
            Assert.IsTrue(team1.IsAlly(team2));
        }

        [Test]
        public void IsAlly_DifferentTeam_ReturnsFalse()
        {
            // Arrange
            TeamId team1 = new TeamId(1);
            TeamId team2 = new TeamId(2);

            // Act & Assert
            Assert.IsFalse(team1.IsAlly(team2));
        }

        [Test]
        public void IsEnemy_DifferentNonNeutralTeams_ReturnsTrue()
        {
            // Arrange
            TeamId team1 = new TeamId(1);
            TeamId team2 = new TeamId(2);

            // Act & Assert
            Assert.IsTrue(team1.IsEnemy(team2));
        }

        [Test]
        public void IsEnemy_SameTeam_ReturnsFalse()
        {
            // Arrange
            TeamId team1 = new TeamId(1);
            TeamId team2 = new TeamId(1);

            // Act & Assert
            Assert.IsFalse(team1.IsEnemy(team2));
        }

        [Test]
        public void IsEnemy_WithNeutralTeam_ReturnsFalse()
        {
            // Arrange
            TeamId neutral = TeamId.Neutral;
            TeamId player = TeamId.Player;

            // Act & Assert
            Assert.IsFalse(neutral.IsEnemy(player));
            Assert.IsFalse(player.IsEnemy(neutral));
        }

        [Test]
        public void IsNeutral_WithZeroValue_ReturnsTrue()
        {
            // Arrange
            TeamId neutral = new TeamId(0);

            // Act & Assert
            Assert.IsTrue(neutral.IsNeutral);
        }

        [Test]
        public void IsNeutral_WithNonZeroValue_ReturnsFalse()
        {
            // Arrange
            TeamId team = new TeamId(1);

            // Act & Assert
            Assert.IsFalse(team.IsNeutral);
        }

        [Test]
        public void StaticNeutral_ReturnsTeamWithZeroValue()
        {
            // Arrange & Act
            TeamId neutral = TeamId.Neutral;

            // Assert
            Assert.AreEqual(0, neutral.Value);
        }

        [Test]
        public void StaticPlayer_ReturnsTeamWithValueOne()
        {
            // Arrange & Act
            TeamId player = TeamId.Player;

            // Assert
            Assert.AreEqual(1, player.Value);
        }

        [Test]
        public void StaticEnemy_ReturnsTeamWithValueTwo()
        {
            // Arrange & Act
            TeamId enemy = TeamId.Enemy;

            // Assert
            Assert.AreEqual(2, enemy.Value);
        }
    }
}


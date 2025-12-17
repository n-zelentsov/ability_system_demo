using System.Collections.Generic;
using NUnit.Framework;
using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Conditions;
using AbilitySystem.Core.Effects;
using AbilitySystem.Core.Events;
using AbilitySystem.Core.Modifiers;
using AbilitySystem.Core.Runtime;
using AbilitySystem.Gameplay.Casting.Conditions;

namespace AbilitySystem.Tests.Gameplay
{
    [TestFixture]
    public class AliveConditionTests
    {
        [Test]
        public void IsMet_CasterIsAlive_ReturnsTrue()
        {
            // Arrange
            AliveAbilityCondition condition = new AliveAbilityCondition();
            MockAbilityOwner caster = new MockAbilityOwner(isAlive: true);
            MockAbility ability = new MockAbility();

            // Act
            bool result = condition.IsMet(caster, ability, AbilityCastContext.Empty);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMet_CasterIsDead_ReturnsFalse()
        {
            // Arrange
            AliveAbilityCondition condition = new AliveAbilityCondition();
            MockAbilityOwner caster = new MockAbilityOwner(isAlive: false);
            MockAbility ability = new MockAbility();

            // Act
            bool result = condition.IsMet(caster, ability, AbilityCastContext.Empty);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void FailureMessage_ReturnsCorrectMessage()
        {
            // Arrange
            AliveAbilityCondition condition = new AliveAbilityCondition();

            // Assert
            Assert.AreEqual("Cannot cast while dead", condition.FailureMessage);
        }
    }

    [TestFixture]
    public class CooldownConditionTests
    {
        [Test]
        public void IsMet_AbilityNotOnCooldown_ReturnsTrue()
        {
            // Arrange
            MockCooldownManager cooldownManager = new MockCooldownManager(isOnCooldown: false);
            CooldownAbilityCondition condition = new CooldownAbilityCondition(cooldownManager);
            MockAbilityOwner caster = new MockAbilityOwner();
            MockAbility ability = new MockAbility();

            // Act
            bool result = condition.IsMet(caster, ability, AbilityCastContext.Empty);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMet_AbilityOnCooldown_ReturnsFalse()
        {
            // Arrange
            MockCooldownManager cooldownManager = new MockCooldownManager(isOnCooldown: true);
            CooldownAbilityCondition condition = new CooldownAbilityCondition(cooldownManager);
            MockAbilityOwner caster = new MockAbilityOwner();
            MockAbility ability = new MockAbility();

            // Act
            bool result = condition.IsMet(caster, ability, AbilityCastContext.Empty);

            // Assert
            Assert.IsFalse(result);
        }
    }

    [TestFixture]
    public class ResourceConditionTests
    {
        [Test]
        public void IsMet_HasEnoughResources_ReturnsTrue()
        {
            // Arrange
            ResourceAbilityCondition condition = new ResourceAbilityCondition();
            MockAbilityOwner caster = new MockAbilityOwner(mana: 100f);
            MockAbility ability = new MockAbility(manaCost: 50f);

            // Act
            bool result = condition.IsMet(caster, ability, AbilityCastContext.Empty);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMet_NotEnoughResources_ReturnsFalse()
        {
            // Arrange
            ResourceAbilityCondition condition = new ResourceAbilityCondition();
            MockAbilityOwner caster = new MockAbilityOwner(mana: 30f);
            MockAbility ability = new MockAbility(manaCost: 50f);

            // Act
            bool result = condition.IsMet(caster, ability, AbilityCastContext.Empty);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsMet_NoResourceCost_ReturnsTrue()
        {
            // Arrange
            ResourceAbilityCondition condition = new ResourceAbilityCondition();
            MockAbilityOwner caster = new MockAbilityOwner(mana: 0f);
            MockAbility ability = new MockAbility(manaCost: 0f);

            // Act
            bool result = condition.IsMet(caster, ability, AbilityCastContext.Empty);

            // Assert
            Assert.IsTrue(result);
        }
    }

    [TestFixture]
    public class RangeConditionTests
    {
        [Test]
        public void IsMet_SelfTargeting_ReturnsTrue()
        {
            // Arrange
            RangeAbilityCondition condition = new RangeAbilityCondition();
            MockAbilityOwner caster = new MockAbilityOwner();
            MockAbility ability = new MockAbility(targetingType: TargetingType.Self);

            // Act
            bool result = condition.IsMet(caster, ability, AbilityCastContext.Empty);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMet_NoTargeting_ReturnsTrue()
        {
            // Arrange
            RangeAbilityCondition condition = new RangeAbilityCondition();
            MockAbilityOwner caster = new MockAbilityOwner();
            MockAbility ability = new MockAbility(targetingType: TargetingType.NoTarget);

            // Act
            bool result = condition.IsMet(caster, ability, AbilityCastContext.Empty);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMet_TargetInRange_ReturnsTrue()
        {
            // Arrange
            RangeAbilityCondition condition = new RangeAbilityCondition();
            MockAbilityOwner caster = new MockAbilityOwner(position: (0, 0, 0));
            MockEffectTarget target = new MockEffectTarget(position: (5, 0, 0));
            MockAbility ability = new MockAbility(targetingType: TargetingType.SingleTarget, range: 10f);
            AbilityCastContext context = new AbilityCastContext(target);

            // Act
            bool result = condition.IsMet(caster, ability, context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMet_TargetOutOfRange_ReturnsFalse()
        {
            // Arrange
            RangeAbilityCondition condition = new RangeAbilityCondition();
            MockAbilityOwner caster = new MockAbilityOwner(position: (0, 0, 0));
            MockEffectTarget target = new MockEffectTarget(position: (20, 0, 0));
            MockAbility ability = new MockAbility(targetingType: TargetingType.SingleTarget, range: 10f);
            AbilityCastContext context = new AbilityCastContext(target);

            // Act
            bool result = condition.IsMet(caster, ability, context);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsMet_PointTargetInRange_ReturnsTrue()
        {
            // Arrange
            RangeAbilityCondition condition = new RangeAbilityCondition();
            MockAbilityOwner caster = new MockAbilityOwner(position: (0, 0, 0));
            MockAbility ability = new MockAbility(targetingType: TargetingType.PointTarget, range: 10f);
            AbilityCastContext context = new AbilityCastContext(targetPoint: (5, 0, 0));

            // Act
            bool result = condition.IsMet(caster, ability, context);

            // Assert
            Assert.IsTrue(result);
        }
    }

    [TestFixture]
    public class TargetAliveConditionTests
    {
        [Test]
        public void IsMet_TargetIsAlive_ReturnsTrue()
        {
            // Arrange
            TargetAliveAbilityCondition condition = new TargetAliveAbilityCondition();
            MockAbilityOwner caster = new MockAbilityOwner();
            MockEffectTarget target = new MockEffectTarget(isAlive: true);
            MockAbility ability = new MockAbility(targetingType: TargetingType.SingleTarget);
            AbilityCastContext context = new AbilityCastContext(target);

            // Act
            bool result = condition.IsMet(caster, ability, context);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsMet_TargetIsDead_ReturnsFalse()
        {
            // Arrange
            TargetAliveAbilityCondition condition = new TargetAliveAbilityCondition();
            MockAbilityOwner caster = new MockAbilityOwner();
            MockEffectTarget target = new MockEffectTarget(isAlive: false);
            MockAbility ability = new MockAbility(targetingType: TargetingType.SingleTarget);
            AbilityCastContext context = new AbilityCastContext(target);

            // Act
            bool result = condition.IsMet(caster, ability, context);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void IsMet_NonSingleTarget_ReturnsTrue()
        {
            // Arrange
            TargetAliveAbilityCondition condition = new TargetAliveAbilityCondition();
            MockAbilityOwner caster = new MockAbilityOwner();
            MockAbility ability = new MockAbility(targetingType: TargetingType.AreaOfEffect);

            // Act - AoE doesn't require target alive check
            bool result = condition.IsMet(caster, ability, AbilityCastContext.Empty);

            // Assert
            Assert.IsTrue(result);
        }
    }

    #region Mock Classes

    internal class MockAbilityOwner : IAbilityOwner
    {
        private readonly float _mana;
        
        public string Id => "mock_owner";
        public string Name => "Mock Owner";
        public bool IsAlive { get; }
        public TeamId Team => TeamId.Player;
        public (float x, float y, float z) Position { get; }
        public IReadOnlyList<IAbility> Abilities => System.Array.Empty<IAbility>();

        public MockAbilityOwner(bool isAlive = true, float mana = 100f, (float, float, float) position = default)
        {
            IsAlive = isAlive;
            _mana = mana;
            Position = position;
        }

        public bool HasResource(AbilityCostType type, float amount) => type == AbilityCostType.Mana && _mana >= amount;
        public void ConsumeResource(AbilityCostType type, float amount) { }
        public float GetResource(AbilityCostType type) => type == AbilityCostType.Mana ? _mana : 0f;
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

    internal class MockEffectTarget : IEffectTarget
    {
        public string Id => "mock_target";
        public string Name => "Mock Target";
        public bool IsAlive { get; }
        public TeamId Team => TeamId.Enemy;
        public (float x, float y, float z) Position { get; }

        public MockEffectTarget(bool isAlive = true, (float, float, float) position = default)
        {
            IsAlive = isAlive;
            Position = position;
        }

        public float GetStat(string statId) => 0f;
        public void ModifyStat(string statId, float delta) { }
        public void SetStat(string statId, float value) { }
        public void ApplyModifier(IModifier modifier) { }
        public void RemoveModifier(IModifier modifier) { }
        public bool HasModifier(string modifierId) => false;
    }

    internal class MockAbility : IAbility
    {
        public AbilityId Id => new AbilityId("mock_ability");
        public string Name => "Mock Ability";
        public string Description => "Test";
        public AbilityData Data { get; }
        public TargetingType TargetingType { get; }
        public IReadOnlyList<IAbilityCondition> Conditions => System.Array.Empty<IAbilityCondition>();
        public IReadOnlyList<IAbilityEffect> Effects => System.Array.Empty<IAbilityEffect>();

        public MockAbility(TargetingType targetingType = TargetingType.SingleTarget, float range = 10f, float manaCost = 0f)
        {
            TargetingType = targetingType;
            AbilityCost[] costs = manaCost > 0 
                ? new AbilityCost[] { new AbilityCost(AbilityCostType.Mana, manaCost) } 
                : System.Array.Empty<AbilityCost>();
            Data = new AbilityData(range: range, resourceCosts: costs);
        }
    }

    internal class MockCooldownManager : ICooldownManager
    {
        private readonly bool _isOnCooldown;

        public MockCooldownManager(bool isOnCooldown = false)
        {
            _isOnCooldown = isOnCooldown;
        }

        public void StartCooldown(IAbilityOwner owner, AbilityId abilityId, float duration) { }
        public float GetRemainingCooldown(IAbilityOwner owner, AbilityId abilityId) => _isOnCooldown ? 5f : 0f;
        public bool IsOnCooldown(IAbilityOwner owner, AbilityId abilityId) => _isOnCooldown;
        public void ResetCooldown(IAbilityOwner owner, AbilityId abilityId) { }
        public void ReduceCooldown(IAbilityOwner owner, AbilityId abilityId, float amount) { }
        public void Tick(float deltaTime) { }
    }

    #endregion
}


using AbilitySystem.Core.Modifiers;

namespace AbilitySystem.Core.Abilities
{
    /// <summary>
    /// Interface for entities that can be targeted by effects
    /// </summary>
    public interface IEffectTarget
    {
        string Id { get; }
        string Name { get; }
        bool IsAlive { get; }
        TeamId Team { get; }
        
        /// <summary>
        /// Position in world space (x, y, z)
        /// </summary>
        (float x, float y, float z) Position { get; }
        
        float GetStat(string statId);
        void ModifyStat(string statId, float delta);
        void SetStat(string statId, float value);
        
        void ApplyModifier(IModifier modifier);
        void RemoveModifier(IModifier modifier);
        bool HasModifier(string modifierId);
    }

    public readonly struct TeamId
    {
        public int Value { get; }

        public TeamId(int value)
        {
            Value = value;
        }

        public bool IsAlly(TeamId other)
        {
            return Value == other.Value;
        }

        public bool IsEnemy(TeamId other)
        {
            return Value != other.Value && Value != 0 && other.Value != 0;
        }

        public bool IsNeutral => Value == 0;

        public static TeamId Neutral => new(0);
        public static TeamId Player => new(1);
        public static TeamId Enemy => new(2);
    }
}


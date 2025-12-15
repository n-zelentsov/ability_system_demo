using System.Collections.Generic;
using AbilitySystem.Core.Abilities;
using AbilitySystem.Core.Combos;
using AbilitySystem.Core.Events;

namespace AbilitySystem.Gameplay.Services
{
    /// <summary>
    /// Manages elemental states and reactions
    /// </summary>
    public sealed class AbilityElementalReactionSystem : IElementalReactionSystem
    {
        private readonly Dictionary<string, AbilityElementalState> _elementalStates = new Dictionary<string, AbilityElementalState>();
        private readonly Dictionary<(DamageType, DamageType), AbilityElementalReactionConfig> _reactions;

        public AbilityElementalReactionSystem()
        {
            _reactions = InitializeReactions();
        }

        private static Dictionary<(DamageType, DamageType), AbilityElementalReactionConfig> InitializeReactions()
        {
            return new Dictionary<(DamageType, DamageType), AbilityElementalReactionConfig>
            {
                // Fire + Ice = Melt (2x damage)
                [(DamageType.Fire, DamageType.Ice)] = new(
                    ElementalReactionType.Melt, 2.0f, null, "Melt! 2x damage"),
                [(DamageType.Ice, DamageType.Fire)] = new(
                    ElementalReactionType.Melt, 1.5f, null, "Reverse Melt! 1.5x damage"),

                // Fire + Lightning = Overload (AoE explosion)
                [(DamageType.Fire, DamageType.Lightning)] = new(
                    ElementalReactionType.Overload, 1.5f, "overload_explosion", "Overload! AoE explosion"),
                [(DamageType.Lightning, DamageType.Fire)] = new(
                    ElementalReactionType.Overload, 1.5f, "overload_explosion", "Overload! AoE explosion"),

                // Ice + Lightning = Superconduct (defense down)
                [(DamageType.Ice, DamageType.Lightning)] = new(
                    ElementalReactionType.Superconduct, 1.2f, "superconduct_debuff", "Superconduct! Defense reduced"),
                [(DamageType.Lightning, DamageType.Ice)] = new(
                    ElementalReactionType.Superconduct, 1.2f, "superconduct_debuff", "Superconduct! Defense reduced"),

                // Fire + Nature = Burning (enhanced DoT)
                [(DamageType.Fire, DamageType.Nature)] = new(
                    ElementalReactionType.Burning, 1.0f, "burning_dot", "Burning! Enhanced DoT"),
                [(DamageType.Nature, DamageType.Fire)] = new(
                    ElementalReactionType.Burning, 1.0f, "burning_dot", "Burning! Enhanced DoT"),

                // Ice + Nature = Frozen (stun)
                [(DamageType.Ice, DamageType.Nature)] = new(
                    ElementalReactionType.Frozen, 1.0f, "frozen_stun", "Frozen! Target stunned"),
                [(DamageType.Nature, DamageType.Ice)] = new(
                    ElementalReactionType.Frozen, 1.0f, "frozen_stun", "Frozen! Target stunned"),

                // Lightning + Nature = Electro-charged (chain)
                [(DamageType.Lightning, DamageType.Nature)] = new(
                    ElementalReactionType.Electrocharged, 1.3f, "electrocharged", "Electro-charged! Chain damage"),
                [(DamageType.Nature, DamageType.Lightning)] = new(
                    ElementalReactionType.Electrocharged, 1.3f, "electrocharged", "Electro-charged! Chain damage"),
            };
        }

        void IElementalReactionSystem.ApplyElement(IEffectTarget target, DamageType element, float duration)
        {
            if (!IsReactiveElement(element))
            {
                return;
            }
            _elementalStates[target.Id] = new AbilityElementalState(element, duration);
        }

        DamageType? IElementalReactionSystem.GetActiveElement(IEffectTarget target)
        {
            return _elementalStates.TryGetValue(target.Id, out AbilityElementalState state) ? state.Element : null;
        }

        ElementalReaction IElementalReactionSystem.CheckReaction(IEffectTarget target, DamageType incomingElement)
        {
            if (!_elementalStates.TryGetValue(target.Id, out AbilityElementalState state))
            {
                return ElementalReaction.None;
            }

            DamageType existingElement = state.Element;
            
            if (_reactions.TryGetValue((existingElement, incomingElement), out AbilityElementalReactionConfig config))
            {
                _elementalStates.Remove(target.Id);
                return ElementalReaction.Create(config.Type, config.DamageMultiplier, config.BonusEffectId, config.Description);
            }

            if (IsReactiveElement(incomingElement))
            {
                _elementalStates[target.Id] = new AbilityElementalState(incomingElement, 10f);
            }
            return ElementalReaction.None;
        }

        public void Tick(float deltaTime)
        {
            List<string> expired = new();

            foreach (KeyValuePair<string, AbilityElementalState> pair in _elementalStates)
            {
                pair.Value.Duration -= deltaTime;
                if (pair.Value.Duration <= 0)
                {
                    expired.Add(pair.Key);
                }
            }

            foreach (string id in expired)
            {
                _elementalStates.Remove(id);
            }
        }

        private static bool IsReactiveElement(DamageType element)
        {
            return element is DamageType.Fire or DamageType.Ice or DamageType.Lightning or DamageType.Nature;
        }

        private sealed class AbilityElementalState
        {
            public DamageType Element { get; }
            public float Duration { get; set; }

            public AbilityElementalState(DamageType element, float duration)
            {
                Element = element;
                Duration = duration;
            }
        }

        private sealed class AbilityElementalReactionConfig
        {
            public ElementalReactionType Type { get; }
            public float DamageMultiplier { get; }
            public string BonusEffectId { get; }
            public string Description { get; }

            public AbilityElementalReactionConfig(ElementalReactionType type, float multiplier, string effectId, string description)
            {
                Type = type;
                DamageMultiplier = multiplier;
                BonusEffectId = effectId;
                Description = description;
            }
        }
    }
}

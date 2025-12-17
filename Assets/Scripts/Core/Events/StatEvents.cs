using AbilitySystem.Core.Abilities;

namespace AbilitySystem.Core.Events
{
    public readonly struct ResourceChangedEvent : IEvent
    {
        public IAbilityOwner Owner { get; }
        public AbilityCostType AbilityCostType { get; }
        public float OldValue { get; }
        public float NewValue { get; }
        public float MaxValue { get; }

        public ResourceChangedEvent(IAbilityOwner owner, AbilityCostType abilityCostType, 
            float oldValue, float newValue, float maxValue)
        {
            Owner = owner;
            AbilityCostType = abilityCostType;
            OldValue = oldValue;
            NewValue = newValue;
            MaxValue = maxValue;
        }
    }

    public readonly struct EntityDiedEvent : IEvent
    {
        public IEffectTarget Entity { get; }
        public IAbilityOwner Killer { get; }

        public EntityDiedEvent(IEffectTarget entity, IAbilityOwner killer)
        {
            Entity = entity;
            Killer = killer;
        }
    }

    public enum AbilityCostType
    {
        Health,
        Mana,
        Energy,
        Rage,
        Shield
    }
}





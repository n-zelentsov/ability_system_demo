# Ability System Demo

Демонстрационный проект системы способностей для Unity, реализованный с применением **Clean Architecture**, **SOLID** принципов, **Event-Driven** архитектуры и **Dependency Injection**.

## 📚 Используемые библиотеки

| Библиотека | Версия | Назначение |
|------------|--------|------------|
| **VContainer** | 1.16.8 | Dependency Injection контейнер (от Cysharp) |
| **R3** | 1.2.9 | Reactive Extensions (EventBus, Observables) |
| **UniTask** | 2.5.10 | Async/await для Unity |

## 🏗️ Архитектура

Проект разделён на три слоя в соответствии с Clean Architecture:

```
┌─────────────────────────────────────────────────┐
│                 Unity Layer                      │
│    (MonoBehaviour, ScriptableObject, Input)     │
│        Namespace: AbilitySystem.Unity           │
└────────────────────────┬────────────────────────┘
                         │ depends on
                         ▼
┌─────────────────────────────────────────────────┐
│                Gameplay Layer                    │
│     (Services, Use Cases, Implementations)      │
│       Namespace: AbilitySystem.Gameplay         │
└────────────────────────┬────────────────────────┘
                         │ depends on
                         ▼
┌─────────────────────────────────────────────────┐
│                  Core Layer                      │
│   (Interfaces, Domain Models, Value Objects)    │
│         Namespace: AbilitySystem.Core           │
│            ❌ Никаких зависимостей ❌             │
└─────────────────────────────────────────────────┘
```

### Core Layer (`Assets/Scripts/Core`)

Чистый C# без зависимостей от Unity. Содержит:

- **Abilities/** - Интерфейсы и модели способностей (`IAbility`, `AbilityData`, `AbilityId`)
- **Effects/** - Интерфейсы эффектов (`IEffect`, `IDurationEffect`, `IPeriodicEffect`)
- **Conditions/** - Интерфейсы условий (`ICondition`, `CastContext`)
- **Modifiers/** - Система модификаторов статов (`IModifier`, `IModifierContainer`)
- **Events/** - Доменные события (`IEventBus`, `DamageDealtEvent`, etc.)
- **Runtime/** - Интерфейсы рантайма (`IAbilitySystem`, `ICooldownManager`, `IEffectProcessor`)

### Gameplay Layer (`Assets/Scripts/Gameplay`)

Реализации бизнес-логики:

- **Stats/** - Система характеристик (`Stat`, `StatContainer`, `ResourcePool`)
- **Services/** - Сервисы системы (`AbilitySystemService`, `CooldownManager`, `EffectProcessor`, `EventBus`)
- **Targeting/** - Система выбора целей (`TargetingService`, `ITargetingStrategy`)
- **Casting/** - Пайплайн каста и условия (`CastingPipeline`, `CooldownCondition`, etc.)

### Unity Layer (`Assets/Scripts/Unity`)

Интеграция с Unity:

- **Data/** - ScriptableObjects (`AbilityDefinition`, `EffectDefinition`, `EntityStatsDefinition`)
- **Factories/** - Фабрики создания (`AbilityFactory`, `EffectFactory`)
- **Presenters/** - MonoBehaviour компоненты (`Entity`, `AbilitySystemBootstrapper`, `DemoController`)

## 🎯 Паттерны проектирования

| Паттерн | Применение |
|---------|-----------|
| **Strategy** | `ITargetingStrategy` - разные стратегии выбора целей |
| **Factory** | `AbilityFactory`, `EffectFactory` - создание объектов |
| **Observer** | `IEventBus` - pub/sub для событий |
| **Service Locator** | `ServiceLocator` - DI контейнер |
| **Command** | `IAbility` + `CastContext` - инкапсуляция действий |
| **Chain of Responsibility** | `CastingPipeline` - цепочка условий |
| **Template Method** | `IDurationEffect.Tick()` - шаблон обработки эффектов |
| **State** | `ElementalState` - состояние элементов на целях |

## ⚡ Combo System

Система комбо позволяет создавать цепочки способностей с бонусами:

```csharp
// Регистрация комбо
comboSystem.RegisterCombo(new ComboDefinition(
    "meteor_combo",
    "Meteor Storm",
    new AbilityId[] { "fireball", "fireball", "fireball" },
    new float[] { 1.0f, 1.2f, 2.0f },  // Множители урона
    timeWindow: 5f
));
```

### Встроенные комбо:
| Комбо | Последовательность | Бонус |
|-------|-------------------|-------|
| **Chain Lightning** | [2] → [2] | x1.5 урон |
| **Meteor Storm** | [1] → [1] → [1] | x2.0 урон |
| **Elemental Storm** | [2] → [1] → [5] | x1.8 урон |
| **Last Stand** | [3] → [4] | x1.5 хил |

## 🔮 Elemental Reactions (как в Genshin Impact)

Система элементальных реакций при комбинировании разных типов урона:

| Реакция | Элементы | Эффект |
|---------|----------|--------|
| **Melt** | Fire + Ice | x2.0 урон |
| **Overload** | Fire + Lightning | AoE взрыв |
| **Superconduct** | Ice + Lightning | -40% защиты |
| **Burning** | Fire + Nature | Усиленный DoT |
| **Frozen** | Ice + Nature | Стан цели |
| **Electrocharged** | Lightning + Nature | Цепной урон |

```csharp
// Элементы автоматически применяются при уроне
// и реагируют при следующей атаке другим элементом
_elementalSystem.ApplyElement(target, DamageType.Fire, 10f);
var reaction = _elementalSystem.CheckReaction(target, DamageType.Ice);
// reaction.Type == ElementalReactionType.Melt, x2.0 damage!
```

## 📦 Типы способностей

Система поддерживает разнообразные типы способностей:

- **Instant Damage** - мгновенный урон (Fireball, Lightning Bolt)
- **Instant Heal** - мгновенное лечение (Flash Heal)
- **DoT (Damage over Time)** - урон со временем (Ignite, Poison)
- **HoT (Heal over Time)** - лечение со временем (Regeneration)
- **Buffs** - положительные модификаторы (Attack Power Up)
- **Debuffs** - отрицательные модификаторы (Armor Break)
- **AoE** - эффекты по области (Meteor, Blizzard)
- **Self-cast** - эффекты на себя (Shield, Sprint)

## 🎮 Использование

### 1. Создание Entity Stats Definition

```
Create → Ability System → Entity Stats Definition
```

Настройте базовые характеристики (здоровье, мана, сила атаки и т.д.)

### 2. Создание Effect Definition

```
Create → Ability System → Effect Definition
```

Выберите категорию эффекта и настройте параметры.

### 3. Создание Ability Definition

```
Create → Ability System → Ability Definition
```

Задайте параметры способности и добавьте созданные эффекты.

### 4. Настройка сцены

1. Создайте GameObject с компонентом `AbilitySystemBootstrapper`
2. Создайте GameObject с компонентом `TargetProvider`
3. Создайте Entity для игрока и целей
4. Добавьте `DemoController` для тестирования

## 📊 Event-Driven Architecture (R3)

События реализованы через **R3 Subject** с поддержкой реактивных операторов:

```csharp
// R3EventBus - реактивный EventBus
public class R3EventBus : IEventBus
{
    private readonly Subject<IEvent> _subject = new Subject<IEvent>();
    
    public Observable<TEvent> OnEvent<TEvent>() where TEvent : IEvent
        => _subject.OfType<IEvent, TEvent>();
}

// Подписка через R3
_eventBus.OnEvent<DamageDealtEvent>()
    .Where(e => e.IsCritical)
    .Subscribe(e => ShowCriticalHit(e))
    .AddTo(_disposables);
```

## 🔧 Dependency Injection (VContainer)

DI реализован через **VContainer** LifetimeScope:

```csharp
// AbilitySystemLifetimeScope.cs
public class AbilitySystemLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<R3EventBus>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<AbilitySystemService>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.RegisterEntryPoint<ComboRegistrar>();
    }
}

// Инжекция через метод Construct
public class Entity : MonoBehaviour, IAbilityOwner
{
    [Inject]
    public void Construct(AbilityFactory factory, TargetProvider provider) { ... }
}
```

## 🧪 Тестируемость

Core и Gameplay слои не зависят от Unity, что позволяет:
- Писать unit-тесты без Unity Test Runner
- Мокать зависимости через интерфейсы
- Тестировать бизнес-логику изолированно

## 📁 Структура проекта

```
Assets/Scripts/
├── Core/                          # Доменный слой (чистый C#)
│   ├── Abilities/
│   │   ├── AbilityData.cs
│   │   ├── AbilityId.cs
│   │   ├── IAbility.cs
│   │   ├── IAbilityOwner.cs
│   │   └── IEffectTarget.cs
│   ├── Conditions/
│   │   └── ICondition.cs
│   ├── Effects/
│   │   ├── EffectContext.cs
│   │   ├── EffectResult.cs
│   │   ├── IDurationEffect.cs
│   │   ├── IEffect.cs
│   │   └── IPeriodicEffect.cs
│   ├── Events/
│   │   ├── AbilityEvents.cs
│   │   ├── EffectEvents.cs
│   │   ├── IEvent.cs
│   │   ├── IEventBus.cs
│   │   └── StatEvents.cs
│   ├── Modifiers/
│   │   ├── IModifier.cs
│   │   └── IModifierContainer.cs
│   └── Runtime/
│       ├── IAbilitySystem.cs
│       ├── ICooldownManager.cs
│       ├── IEffectProcessor.cs
│       ├── IServiceLocator.cs
│       └── ITimeProvider.cs
├── Gameplay/                      # Слой бизнес-логики
│   ├── Casting/
│   │   ├── CastingPipeline.cs
│   │   └── Conditions/
│   │       ├── AliveCondition.cs
│   │       ├── CooldownCondition.cs
│   │       ├── RangeCondition.cs
│   │       ├── ResourceCondition.cs
│   │       └── TargetAliveCondition.cs
│   ├── Services/
│   │   ├── AbilitySystemService.cs
│   │   ├── CooldownManager.cs
│   │   ├── EffectProcessor.cs
│   │   ├── EventBus.cs
│   │   ├── ServiceLocator.cs
│   │   └── Effects/
│   │       ├── DamageOverTimeEffect.cs
│   │       ├── HealOverTimeEffect.cs
│   │       ├── InstantDamageEffect.cs
│   │       ├── InstantHealEffect.cs
│   │       └── StatModifierEffect.cs
│   ├── Stats/
│   │   ├── ResourcePool.cs
│   │   ├── Stat.cs
│   │   ├── StatContainer.cs
│   │   └── StatType.cs
│   └── Targeting/
│       ├── ITargetingStrategy.cs
│       ├── ITargetProvider.cs
│       ├── TargetFilter.cs
│       └── TargetingService.cs
└── Unity/                         # Unity интеграция
    ├── Data/
    │   ├── AbilityDefinition.cs
    │   ├── EffectDefinition.cs
    │   └── EntityStatsDefinition.cs
    ├── Factories/
    │   ├── AbilityFactory.cs
    │   └── EffectFactory.cs
    └── Presenters/
        ├── AbilitySystemBootstrapper.cs
        ├── DemoController.cs
        ├── Entity.cs
        └── TargetProvider.cs
```

## 📝 Лицензия

MIT License

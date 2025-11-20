# ReplayLib

ReplayLib is a comprehensive utility library for Unity game development, providing core patterns, extensions, and systems used throughout "The Last Word" project. The library emphasizes memory management, performance optimization, and consistent coding patterns.

## About

This library is used in production by [The Last Word](https://thelastwordgame.com), a real-time multiplayer word puzzle game developed by [Replay Digital](https://www.replaydigital.net/). The game features deterministic networking with Photon Quantum, multiple competitive game modes, and cross-platform play. ReplayLib has been battle-tested in a live production environment serving thousands of players on iOS.

**Download:** [App Store](https://apps.apple.com/us/app/last-word-puzzle-battle-game/id6745356424)

## Table of Contents

- [Core Patterns](#core-patterns)
- [Singleton System](#singleton-system)
- [Data Persistence](#data-persistence)
- [Logging and Debugging](#logging-and-debugging)
- [Extensions](#extensions)
- [Performance Optimization](#performance-optimization)
- [UI Utilities](#ui-utilities)
- [Platform Integration](#platform-integration)
- [Usage Guidelines](#usage-guidelines)

## Core Patterns

### Singleton System

ReplayLib provides three types of singleton patterns for different use cases:

#### ComponentSingleton&lt;T&gt;

Addressable-aware singleton pattern for MonoBehaviour classes requiring Unity lifecycle management.

**Key Features:**
- Automatic Addressables handle management
- Resource loading support
- Scene persistence with DontDestroyOnLoad
- Automatic initialization and cleanup
- Warmup support for smooth animations

**Usage:**
```csharp
using Replay.Utils;

public class BackEndManager : ComponentSingleton<BackEndManager>
{
    protected override void OnSingletonInit()
    {
        // Initialization code
    }

    protected override void OnSingletonTeardown()
    {
        // Cleanup code
    }
}

// Access the singleton
BackEndManager.Instance.SomeMethod();

// Check if loaded without instantiation
if (BackEndManager.IsLoaded)
{
    // Safe to use
}

// Warm up prefab for faster instantiation
BackEndManager.WarmupPrefab();
```

**Important Lifecycle Patterns:**
- Use `Awake()` for initialization instead of `OnSingletonInit()` override
- Use `protected void OnDestroy()` without calling `base.OnDestroy()`
- Check existence with `IsLoaded` property (not `IsLoaded()` method)
- Never compare `Instance` to null (use `IsLoaded` or `WeakInstance`)

**Automatic Serialization:**
When implementing `IReplaySerialazable`, `Deserialize()` is called automatically by the framework. Do NOT manually call it in `Start()` or `Awake()`.

```csharp
public class PlayerProfileManager : ComponentSingleton<PlayerProfileManager>, IReplaySerialazable
{
    // Deserialize() called automatically - no manual call needed
    public void Deserialize()
    {
        // Load data
    }

    public void Serialize()
    {
        // Save data - call manually as needed
    }
}
```

#### ScriptableObjectSingleton&lt;T&gt;

Singleton pattern for ScriptableObject-based configuration and data.

**Usage:**
```csharp
public class GameSettings : ScriptableObjectSingleton<GameSettings>
{
    public float musicVolume;
    public float sfxVolume;
}

// Access settings
float volume = GameSettings.Instance.musicVolume;
```

#### Singleton&lt;T&gt;

Basic singleton pattern for pure C# classes without Unity dependencies.

### Loadable System

Components implementing `ILoadable` can be dynamically loaded from Resources or Addressables:

```csharp
public static string GetResourcePath() => "Prefabs/MyManager";
public static string GetAddressablesIdentifier() => "MyManagerAddressable";
```

## Data Persistence

### LocalSerializer

Wrapper around Unity's PlayerPrefs with iCloud support and type-safe operations.

**Features:**
- Automatic iCloud synchronization on iOS
- Type-safe get/set methods
- Support for bool, int, long, float, double, string
- Device account persistence option

**Usage:**
```csharp
using Replay.Utils;

// Save data
LocalSerializer.Instance.SetInt("score", 100);
LocalSerializer.Instance.SetString("playerName", "Alice");

// Save to device account (iCloud on iOS)
LocalSerializer.Instance.SetString("profileId", "12345", saveToDeviceAccount: true);

// Load data
int score = LocalSerializer.Instance.GetInt("score");
string name = LocalSerializer.Instance.GetString("playerName", "DefaultName");

// Persist to disk
LocalSerializer.Instance.Serialize();

// Clear all data
LocalSerializer.Instance.DeleteAll();
```

### IReplaySerialazable

Interface for objects that need serialization support with automatic deserialization when used with ComponentSingleton.

```csharp
public interface IReplaySerialazable
{
    void Serialize();     // Called manually when needed
    void Deserialize();   // Called automatically by ComponentSingleton
}
```

## Logging and Debugging

### Dev (Debug Logging)

Conditional debug logging that only executes in debug builds.

**Features:**
- Tagged logging for easy filtering
- Method name tracking
- Force logging option
- Custom tag support

**Usage:**
```csharp
using Replay.Utils;

// Basic logging
Dev.Log("Player spawned");

// With custom tag
Dev.Log("Connection established", "Network");

// Warnings and errors
Dev.LogWarning("Low memory detected");
Dev.LogError("Failed to load asset");

// Log method name with location
Dev.LogMethod("GameState");

// Force logging even in release builds
Dev.Log("Critical error", force: true);
```

### Logger

Persistent file-based logging system with queue management.

**Features:**
- Thread-safe logging
- Automatic file management
- Exception tracking
- Log filtering by tag
- Queue size management

**Usage:**
```csharp
// Logger initializes automatically
// All Unity logs are captured

// Get logs
string logs = Logger.Instance.GetLogs();
string networkLogs = Logger.Instance.GetLogs("Network");

// Manage log file
Logger.Instance.FlushLogsToFile();
Logger.Instance.TrimLogFile();

// For debugging
string logPath = Logger.Instance.logFilePath;
```

### IDebugLoggable

Interface for classes that need debug output with custom `ToDebugString()` pattern.

```csharp
public class PlayerProfile : IDebugLoggable
{
    public string ToDebugString()
    {
        return $"PlayerProfile [ID: {id}, Name: {name}, Level: {level}]";
    }
}
```

## Extensions

ReplayLib provides extensive extension methods for common Unity types and operations.

### List Extensions

```csharp
using Replay.Utils;

List<string> items = new List<string> { "a", "b", "c" };

// Shuffle list
items.Shuffle();

// Shuffle with seed for deterministic results
items.Shuffle(42);

// Rotate elements
items.RotateLeft();
items.RotateRight(2);

// Swap elements
items.Swap(0, 2);

// Clean up
items.RemoveNullEntries();
items.RemoveDefaultValues();

// Check index validity
if (items.HasIndex(5))
    Debug.Log(items[5]);

// Destroy MonoBehaviour list contents
List<Enemy> enemies = new List<Enemy>();
enemies.DestoryContentsAndClear();
```

### String Extensions

```csharp
using Replay.Utils;

string text = "hello world";

// Character shuffling
string shuffled = text.ShuffleCharacters();

// Null/empty checks
bool isEmpty = text.IsNullOrEmpty();
bool isWhitespace = text.IsNullOrWhiteSpace();

// Parsing with defaults
int number = "123".IntValue();
long bigNumber = "9999999999".LongValue(0L);
float decimal = "3.14".FloatValue();
double precise = "3.14159".DoubleValue();
bool flag = "true".BoolValue();

// Date parsing
DateTime? date = "2024-01-01".ToDateTime();

// Case conversions
string title = "hello world".ToTitleCase();
char upper = 'a'.ToUpper();

// URL encoding
string escaped = "hello world".ToEscapeURL();
string dataEscaped = "data string".ToEscapeDataString();

// Formatting
string bracketed = "Tag".ToBracketedString(); // "[Tag]"
string display = name.GetNAOrString(); // Returns "N/A" if null/empty
```

### GameObject Extensions

```csharp
using Replay.Utils;

GameObject obj = someGameObject;

// Scene management
obj.MoveToMainScene();
obj.MoveToActiveScene();

// Hierarchy navigation
GameObject root = obj.GetRootGameObject();
GameObject[] sceneRoots = GameObjectExtensions.GetRootGameObjectsInActiveScene();

// Check if in DontDestroyOnLoad
bool persistent = obj.isDontDestroyOnLoadActivated();

// Recursive operations
obj.SetActiveRecursively(false);

// Message broadcasting
obj.BroadcastMessageToRoot("OnGameStart");
GameObjectExtensions.BroadcastMessageToRootObjectsInActiveScene("OnLevelLoad");

// Hierarchy checks
bool isChild = parent.HasChildObject(child, recursive: true);
```

### Enum Extensions

```csharp
using Replay.Utils;

public enum GameMode { Menu, Playing, Paused, GameOver }

GameMode mode = GameMode.Playing;

// Navigation
GameMode next = mode.Next();
GameMode previous = mode.Previous();

// Position checks
bool isFirst = mode.IsFirst();
bool isLast = mode.IsLast();
int index = mode.ValueIndex();

// Conversion
string name = mode.ConvertToString();
GameMode parsed = "Playing".ConvertToEnum<GameMode>();
int value = mode.intValue();
```

### Component Extensions

```csharp
using Replay.Utils;

// Get or add component
AudioSource audio = gameObject.GetOrAddComponent<AudioSource>();

// Check component existence
bool hasRigidbody = gameObject.HasComponent<Rigidbody>();

// Fix prefab clone suffix
gameObject.FixOrAppendPrefabCloneSuffix("Singleton");
```

### Transform Extensions

```csharp
using Replay.Utils;

// Reset transformations
transform.ResetLocal();
transform.ResetWorld();

// Destroy children
transform.DestroyChildren();
transform.DestroyChildrenImmediate();

// Find children
Transform child = transform.FindDeepChild("NestedChild");
```

### Math and Numeric Extensions

```csharp
using Replay.Utils;

// Range checks
bool inRange = 5.InRange(0, 10);
bool between = value.Between(min, max);

// Clamping
int clamped = value.Clamp(0, 100);

// Rounding
float rounded = 3.7f.Round();
float ceiled = 3.2f.Ceil();
float floored = 3.8f.Floor();
```

## Performance Optimization

### PerformanceOptimizer

Central system for managing shader warmup, particle preloading, and scene optimization.

**Features:**
- Shader variant warmup (synchronous and async)
- Particle effect preloading
- Scene warmup camera support
- Progressive loading to avoid frame drops

**Usage:**
```csharp
// Shader warmup (async)
PerformanceOptimizer.Instance.WarmupShaderVariantsAsync(() =>
{
    Debug.Log("Shaders ready");
});

// Particle preloading
PerformanceOptimizer.Instance.WarmupParticles();

// Scene warmup
PerformanceOptimizer.Instance.BlitWarmupCameras(() =>
{
    Debug.Log("Scene warmed up");
});
```

### ParticleCleanup

Component for automatic particle system cleanup after playback.

### SceneWarmupCamera

Pre-renders scenes to avoid first-frame hitches.

### Memory Management

**Never call directly:**
- `Resources.UnloadUnusedAssets()`
- `GC.Collect()`

**Instead use:**
```csharp
BackEndManager.Instance.FreeUpResources();
```

## UI Utilities

### Button Components

```csharp
// Button event handling
public class MyButton : MonoBehaviour, IButtonEvents
{
    public void OnButtonClick() { }
    public void OnButtonDown() { }
    public void OnButtonUp() { }
}
```

### ScrollRect Enhancements

- `NestedScrollRect` - Handle nested scroll views
- `ScrollRectClicker` - Add click detection to scroll content
- `ScrollRectPaginator` - Snap pagination support
- `ScrollRectTouchPassthrough` - Touch input passthrough

### Sorting Utilities

- `CanvasOrderSorter` - Manage canvas sorting layers
- `SortingLayer` / `SortingOrder` - Control render order

## Platform Integration

### iCloud Support (iOS)

```csharp
// Automatic with LocalSerializer
LocalSerializer.Instance.SetString("userId", "12345", saveToDeviceAccount: true);

// Manual iCloud operations
iCloudManager.Instance.SaveToCloud("key", "value");
var (success, value) = iCloudManager.Instance.LoadFromCloud("key");
iCloudManager.Instance.Synchronze();
```

### Platform Utilities

```csharp
// Platform checks
bool isMobile = PlatformUtils.IsMobile();
bool isIOS = PlatformUtils.IsIOS();
bool isAndroid = PlatformUtils.IsAndroid();

// Screen utilities
float aspect = ScreenUtils.GetAspectRatio();
bool isPortrait = ScreenUtils.IsPortrait();
```

### Input Handling

```csharp
// Platform-specific input
InputHandler input = InputHandler.Instance;

if (input.GetTouchDown())
{
    Vector2 position = input.GetTouchPosition();
}
```

### Deep Linking

```csharp
public class MyDeepLinkReceiver : DeepLinkReceiver
{
    protected override void OnDeepLinkActivated(string url)
    {
        Debug.Log($"Deep link received: {url}");
    }
}
```

## Usage Guidelines

### Namespace

Always add the namespace when using ReplayLib:

```csharp
using Replay.Utils;
```

### Naming Conventions

**Instance Variables:**
- Private: `_variableName` (underscore prefix)
- Public: `variableName` (camelCase)

**Properties:**
- All properties use camelCase (including computed properties)

**Constants:**
- Private: `kConstantName` (k prefix, PascalCase)
- Public: `CONSTANT_NAME` (ALL_CAPS)

**Static Readonly:**
```csharp
public static readonly List<int> playerCountPerRound = new () { 12, 8, 5 };
```

### Method Naming

**Debug Methods:**
Prefix methods intended for editor/debug use with `DEBUG_`:
```csharp
public void DEBUG_ResetProgress() { }
public void DEBUG_SkipLevel() { }
```

### Code Style

**Avoid Early Returns:**
```csharp
// NEVER do this
if (object == null)
    return;

// ALWAYS do this
if (object != null)
{
    // Implementation
}
```

**Single-line conditionals:**
```csharp
// No braces for single statements
if (score > 100)
    Debug.Log("High score!");

// Braces for multiple statements
if (score > 100)
{
    Debug.Log("High score!");
    PlaySound();
}
```

### Return Value Pattern

Use `retVal` for return values:
```csharp
public List<GameMode> GetAvailableModes()
{
    var retVal = new List<GameMode>();
    retVal.Add(GameMode.WordDash);
    retVal.Add(GameMode.WordPool);
    return retVal;
}
```

### Component Singleton Best Practices

**Calling base methods:**
When inside a ComponentSingleton derived class, call methods directly:
```csharp
// DON'T do this
ComponentSingleton<BackEndManager>.ReleaseAllAddressables();

// DO this
ReleaseAllAddressables();
```

**Checking if loaded:**
```csharp
// DON'T compare Instance to null (may instantiate)
if (BackEndManager.Instance != null) { }

// DO use IsLoaded property
if (BackEndManager.IsLoaded) { }

// OR use WeakInstance
if (BackEndManager.WeakInstance != null) { }
```

### Extending the Library

When implementing general, reusable functionality, add it to ReplayLib rather than implementing inline:

```csharp
// Example: Adding a new extension method
namespace Replay.Utils
{
    public static class MyExtensions
    {
        public static bool IsEven(this int value)
        {
            return value % 2 == 0;
        }
    }
}
```

## Tools and Utilities

### ReplayTimer

High-precision timer for gameplay timing:

```csharp
ReplayTimer timer = new ReplayTimer();

timer.StartTimer();
// or
timer.ResetTimer();

// Get elapsed time
long milliseconds = timer.elapsedMilliseconds;
double seconds = timer.elapsedSeconds;

// Countdown mode
double remaining = timer.GetCountdownSeconds(60.0); // 60 second countdown
bool expired = timer.GetIsExpired(60.0);

// Pause/resume
timer.paused = true;
timer.paused = false;

timer.StopTimer();
```

### Network Utilities

```csharp
// Check connectivity
Reachability.NetworkStatus status = Reachability.GetNetworkStatus();
bool hasInternet = status != Reachability.NetworkStatus.NotReachable;

// URL encoding
string encoded = StringExtensions.ToEscapeURL(url);
```

### CSV Parsing

```csharp
// Parse CSV data
string[][] data = CSVParser.Parse(csvText);
```

## Advanced Features

### Serialization Resolvers

Custom JSON serialization support:

- `CollectionClearingContractResolver` - Clear collections before deserializing
- `ConditionalIgnorePropertiesResolver` - Conditional property serialization
- `IgnorePropertiesResolver` - Exclude specific properties

### Server Time

Synchronized server time management:

```csharp
DateTime serverTime = ServerTime.Instance.GetServerTime();
```

### Share Integration

Native sharing on mobile platforms:

```csharp
ShareSheet.Instance.ShareText("Check out this game!", "https://example.com");
ShareSheet.Instance.ShareImage(texture, "Screenshot");
```

### Notification Support

```csharp
NotificationsHelper.Schedule("Reminder", "Come back to play!", DateTime.Now.AddHours(24));
```

## Editor Utilities

### DebugLoggable Editor

Custom inspector for classes implementing `IDebugLoggable`:

```csharp
[CustomEditor(typeof(YourClass))]
public class YourClassEditor : DebugLoggableEditor
{
    // Automatic debug UI generation
}
```

## Best Practices Summary

1. **Always use ReplayLib utilities** instead of standard Unity APIs when available
2. **Prefer ComponentSingleton** for manager classes needing Unity lifecycle
3. **Use LocalSerializer** for all data persistence (never use PlayerPrefs directly)
4. **Use Dev.Log()** for debug logging instead of Debug.Log()
5. **Memory management** should go through BackEndManager.FreeUpResources()
6. **Follow naming conventions** consistently (camelCase properties, k prefix for private constants)
7. **Avoid early returns** - use inverted conditions with proper nesting
8. **Check singleton existence** with IsLoaded property, not Instance comparison
9. **Extend the library** when implementing reusable functionality
10. **Use `retVal`** for return value variables

## Dependencies

ReplayLib has minimal external dependencies:
- Unity 6 or higher
- Unity Addressables
- DOTween (for some animation utilities)
- Newtonsoft.Json (for serialization)

## Contributing

When adding new utilities to ReplayLib:

1. Follow existing code style and patterns
2. Add XML documentation comments
3. Provide usage examples in this README
4. Place new extensions in appropriate folders
5. Use the `Replay.Utils` namespace
6. Test thoroughly in both Editor and runtime contexts

## License

MIT License

Copyright (c) 2025 Replay Digital

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

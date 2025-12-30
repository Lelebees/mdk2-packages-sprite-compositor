# Mal.MdkScriptMixin.Coroutines

> [!NOTE]
> **Version:** 1.0.4  
> **Authors:** Malware  
> **Package:** `Mal.MdkScriptMixin.Coroutines`
> 
> **Description:** A lightweight coroutine scheduler for Space Engineers programmable blocks, enabling async-style programming patterns.
>
> **[Installation Instructions →](./Mal.MdkScriptMixin.Coroutines-Installation.md)** | **[Release Notes →](./Mal.MdkScriptMixin.Coroutines-ReleaseNotes.md)**

---
# Coroutines for Space Engineers

A lightweight coroutine scheduler that brings async-style programming patterns to Space Engineers programmable blocks. Write cleaner, more maintainable code by breaking complex logic into simple sequential steps.

## Quickstart

1. **Install the NuGet package** `Mal.MdkScriptMixin.Coroutines` in your script project
2. **Call `Coroutines.Main()`** in your script's `Main()` method
3. **Start coroutines** with `Coroutines.Run(YourMethod())`
4. **Use `yield return`** to pause and resume execution

```csharp
public void Main(string argument, UpdateType updateSource)
{
    Coroutines.Main(argument, updateSource); // Required - updates all running coroutines
}

public IEnumerator<When> MyCoroutine()
{
    Echo("Step 1");
    yield return When.TimePassed(1000); // Wait 1 second
    Echo("Step 2");
    yield return When.NextUpdate(UpdateFrequency.Update100); // Wait ~1.6 seconds
    Echo("Done!");
}
```

## Why Use Coroutines?

Programmable blocks run in single execution slices that must complete quickly to avoid script timeout. Coroutines let you:

- **Spread work across multiple ticks** - Process large collections without timing out
- **Write sequential logic** - No more nested state machines or complex flags
- **Wait for conditions** - Pause execution until something happens
- **Run tasks in parallel** - Multiple coroutines run independently
- **Cancel operations** - Stop running tasks cleanly with cancellation tokens

## Quick Example

```csharp
public Program()
{
    // Start a coroutine when the script loads
    Coroutines.Run(StartupSequence());
}

public IEnumerator<When> StartupSequence()
{
    Echo("Initializing systems...");
    yield return When.TimePassed(1000); // Wait 1 second
    
    Echo("Running diagnostics...");
    yield return When.TimePassed(2000); // Wait 2 more seconds
    
    Echo("Systems online!");
}

public void Main(string argument, UpdateType updateSource)
{
    Coroutines.Main(argument, updateSource);
}
```

## Core Concepts

### 1. The Coroutines Property

The mixin adds a `Coroutines` property to your `Program` class:

```csharp
public Coroutines Coroutines => _coroutines ?? (_coroutines = new Coroutines(this));
```

Use this to start coroutines and manage the scheduler.

### 2. Running Coroutines

Start a coroutine with `Coroutines.Run()`:

```csharp
Coroutines.Run(MyCoroutine());
```

This returns a `ulong` ID you can use to wait for completion.

### 3. The Main Loop

Call `Coroutines.Main()` in your script's `Main()` method to process all active coroutines:

```csharp
public void Main(string argument, UpdateType updateSource)
{
    Coroutines.Main(argument, updateSource);
    // Your other code here
}
```

### 4. Coroutine Methods

Coroutines are methods that return `IEnumerator<When>`:

```csharp
public IEnumerator<When> MyCoroutine()
{
    Echo("Step 1");
    yield return When.NextUpdate();
    Echo("Step 2");
    yield return When.TimePassed(1000);
    Echo("Done!");
}
```

## When Conditions

The `When` struct defines when a coroutine should resume:

### Update Timing

```csharp
When.NextUpdate1()      // Resume on next tick (every 16.67ms) - use sparingly!
When.NextUpdate()       // Resume every 10 ticks (~167ms) - most common
When.NextUpdate100()    // Resume every 100 ticks (~1.67s) - for slow polling
When.Next(UpdateType.Trigger)  // Resume on specific update types
```

### Time-Based

```csharp
When.TimePassed(1000)   // Wait 1000 milliseconds (1 second)
When.TimePassed(5000, UpdateType.Update1)  // Wait 5s, check every tick
```

**Note:** Timing accuracy depends on tick rate and check frequency. Default is Update10 (~167ms resolution).

### Condition-Based

```csharp
When.True(() => inventoryFull)                    // Wait until condition is true
When.True(w => someComplexCheck(w))               // With When parameter
When.True(() => foundTarget, UpdateType.Update1)  // Check every tick
```

### Wait for Completion

```csharp
var childId = Coroutines.Run(ChildTask());
yield return When.Completed(childId);  // Wait for child to finish
Echo("Child task completed!");
```

## Cancellation Tokens

Stop long-running coroutines cleanly:

```csharp
Cancellation _taskCancellation;

public Program()
{
    _taskCancellation = new Cancellation();
    Coroutines.Run(LongRunningTask(_taskCancellation.Token));
}

public IEnumerator<When> LongRunningTask(CancellationToken token)
{
    while (!token.IsCancellationRequested)
    {
        Echo("Working...");
        yield return When.NextUpdate();
    }
    Echo("Task cancelled!");
}

public void Main(string argument, UpdateType updateSource)
{
    if (argument == "stop")
        _taskCancellation.Cancel();
    
    Coroutines.Main(argument, updateSource);
}
```

## Processing Collections with ForEach

Spread collection processing across multiple ticks to avoid timeouts:

```csharp
public IEnumerator<When> ProcessAllBlocks()
{
    var blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlocks(blocks);
    
    yield return Coroutines.ForEach(
        blocks,
        block => {
            // Process each block
            Echo($"Processing: {block.CustomName}");
        },
        batchSize: 10,  // Process 10 blocks per tick
        frequency: UpdateType.Update1
    );
    
    Echo("All blocks processed!");
}
```

## Patterns and Best Practices

### Sequential Operations

```csharp
public IEnumerator<When> PowerUpSequence()
{
    yield return When.Completed(Coroutines.Run(EnableReactors()));
    yield return When.Completed(Coroutines.Run(EnableThrusters()));
    yield return When.Completed(Coroutines.Run(EnableWeapons()));
    Echo("All systems powered!");
}
```

### Parallel Operations

```csharp
public IEnumerator<When> ParallelSetup()
{
    var reactor = Coroutines.Run(ConfigureReactors());
    var thrusters = Coroutines.Run(ConfigureThrusters());
    var weapons = Coroutines.Run(ConfigureWeapons());
    
    // Wait for all three to complete
    yield return When.Completed(reactor);
    yield return When.Completed(thrusters);
    yield return When.Completed(weapons);
    
    Echo("All configurations complete!");
}
```

### Polling Loop

```csharp
public IEnumerator<When> MonitorInventory(CancellationToken token)
{
    while (!token.IsCancellationRequested)
    {
        var inventory = GetInventoryStatus();
        Echo($"Inventory: {inventory}");
        yield return When.NextUpdate100();  // Check every 100 ticks
    }
}
```

### State Machine Replacement

Instead of complex state machines:

```csharp
public IEnumerator<When> MiningOperation()
{
    // Extend drill
    SetDrills(true);
    yield return When.TimePassed(2000);
    
    // Mine until inventory full
    while (!IsInventoryFull())
    {
        Echo("Mining...");
        yield return When.NextUpdate();
    }
    
    // Retract and return to base
    SetDrills(false);
    yield return When.Completed(Coroutines.Run(ReturnToBase()));
    
    Echo("Mining complete!");
}
```

## Performance Considerations

- **Update1 vs Update10**: Use Update10 by default. Only use Update1 when timing is critical.
- **Condition checks**: Keep condition lambdas lightweight - they run every check.
- **Batch processing**: Use `ForEach` for large collections instead of processing all at once.
- **Cancellation**: Always provide cancellation tokens for long-running tasks.
- **Coroutine count**: Monitor `Coroutines.Count` if spawning many coroutines dynamically.

## Common Pitfalls

❌ **Don't forget to call Coroutines.Main()**
```csharp
public void Main(string argument, UpdateType updateSource)
{
    // Coroutines won't run without this!
    Coroutines.Main(argument, updateSource);
}
```

❌ **Don't use Thread.Sleep() or busy loops**
```csharp
// WRONG - blocks execution
Thread.Sleep(1000);

// RIGHT - yields control
yield return When.TimePassed(1000);
```

❌ **Don't assume exact timing**
```csharp
// TimePassed is approximate due to tick granularity
yield return When.TimePassed(100);  // Actual wait may vary
```

## Example: Complete Automated Miner

```csharp
public partial class Program : MyGridProgram
{
    Cancellation _miningCancellation;
    
    public Program()
    {
        _miningCancellation = new Cancellation();
        Coroutines.Run(AutoMine(_miningCancellation.Token));
    }
    
    public IEnumerator<When> AutoMine(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Echo("Starting mining run...");
            yield return When.Completed(Coroutines.Run(MiningCycle()));
            
            Echo("Returning to base...");
            yield return When.Completed(Coroutines.Run(ReturnToBase()));
            
            Echo("Unloading cargo...");
            yield return When.Completed(Coroutines.Run(UnloadCargo()));
            
            Echo("Waiting before next run...");
            yield return When.TimePassed(5000);
        }
    }
    
    public IEnumerator<When> MiningCycle()
    {
        SetDrills(true);
        while (!IsInventoryFull())
        {
            Echo("Mining...");
            yield return When.NextUpdate();
        }
        SetDrills(false);
    }
    
    public IEnumerator<When> ReturnToBase()
    {
        // Navigate to base coordinates
        yield return When.True(() => IsAtBase());
    }
    
    public IEnumerator<When> UnloadCargo()
    {
        // Unload logic here
        yield return When.TimePassed(3000);
    }
    
    public void Main(string argument, UpdateType updateSource)
    {
        if (argument == "stop")
            _miningCancellation.Cancel();
        
        Coroutines.Main(argument, updateSource);
    }
    
    // Helper methods...
    void SetDrills(bool enabled) { }
    bool IsInventoryFull() => false;
    bool IsAtBase() => false;
}
```

## API Reference

### Coroutines Class

- `Run(IEnumerator<When> coroutine)` - Start a coroutine, returns ID
- `Main(string argument, UpdateType updateSource)` - Process all coroutines
- `Count` - Number of active coroutines
- `LifetimeTicks` - Current game tick count

### When Static Methods

- `NextUpdate1()` / `NextUpdate()` / `NextUpdate100()` - Wait for next update
- `Next(UpdateType)` - Wait for specific update type
- `TimePassed(int ms, UpdateType frequency)` - Wait specified time
- `True(Func<bool> condition, UpdateType frequency)` - Wait for condition
- `Completed(ulong coroutineId, UpdateType frequency)` - Wait for coroutine

### Cancellation

- `new Cancellation()` - Create cancellation source
- `cancellation.Token` - Get cancellation token
- `cancellation.Cancel()` - Trigger cancellation
- `token.IsCancellationRequested` - Check if cancelled

### ForEach

- `Coroutines.ForEach<T>(collection, action, batchSize, frequency)` - Process collection over time

---

*Documentation auto-generated from package metadata. Last updated: 2025-12-30*

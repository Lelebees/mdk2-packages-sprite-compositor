# Mal.MdkScriptMixin.Stopwatch

> [!NOTE]
> **Version:** 1.0.0  
> **Authors:** Malware  
> **Package:** `Mal.MdkScriptMixin.Stopwatch`
> 
> **Description:** A lightweight timing utility for measuring elapsed game time across script runs.
>
> **[Installation Instructions →](./Mal.MdkScriptMixin.Stopwatch-Installation.md)** | **[Release Notes →](./Mal.MdkScriptMixin.Stopwatch-ReleaseNotes.md)**

---
# Stopwatch for Space Engineers

A lightweight timing utility for Space Engineers programmable blocks. Measure elapsed game time between script runs with a familiar API.

**IMPORTANT:** This stopwatch measures **game time** (ticks between script executions), not execution time within a single script run. It cannot be used for performance profiling or finding bottlenecks in your code.

## Quickstart

1. **Install the NuGet package** `Mal.MdkScriptMixin.Stopwatch` in your script project
2. **Create a stopwatch** with `new Stopwatch(this)`
3. **Control timing** with `Start()`, `Stop()`, `Reset()`, `Restart()`
4. **Read elapsed time** via `ElapsedTicks` or `Elapsed` property

```csharp
public void Main(string argument, UpdateType updateSource)
{
    // Start timing when door opens
    if (argument == "open")
    {
        doorTimer.Start();
        door.OpenDoor();
    }
    
    // Check how long door has been open
    if (doorTimer.Elapsed.TotalSeconds > 5)
    {
        door.CloseDoor();
        doorTimer.Reset();
    }
}
```

## Why Use Stopwatch?

Need to wait 5 seconds before closing a door? Want a weapon cooldown? Track how long your script has been running? That's what this is for.

It's useful when you need to measure game time that spans multiple script executions - things like "wait X seconds" or "how long has this been going on".

## What It Can't Do

This stopwatch tracks game ticks between script runs. It won't help you find slow code or measure performance within a single Main() call. The resolution is about 16ms (one tick), so it's not useful for precise millisecond timing.

If you need to profile your script's performance, check `Runtime.LastRunTimeMs` instead.

## Creating Stopwatches

### Standard Creation

```csharp
var stopwatch = new Stopwatch(this);
stopwatch.Start();
```

## Basic Operations

### Start, Stop, Reset

```csharp
stopwatch.Start();    // Begin timing
stopwatch.Stop();     // Pause timing (elapsed time preserved)
stopwatch.Reset();    // Clear elapsed time and stop
stopwatch.Restart();  // Clear and start timing
```

### Fluent API

All methods return the stopwatch instance for chaining:

```csharp
var timer = new Stopwatch(this).Start();
// Later:
timer.Stop().Reset().Start();
```

## Reading Time

### ElapsedTicks (long)

Raw tick count - fastest to read:

```csharp
long ticks = stopwatch.ElapsedTicks;
if (ticks > 600) // 10 seconds
    Echo("Operation took too long!");
```

### Elapsed (TimeSpan)

Converted to TimeSpan for formatting:

```csharp
TimeSpan elapsed = stopwatch.Elapsed;
Echo($"Duration: {elapsed:mm\\:ss\\.fff}");
Echo($"Milliseconds: {elapsed.TotalMilliseconds}");
```

### IsRunning (bool)

Check if stopwatch is currently timing:

```csharp
if (stopwatch.IsRunning)
    Echo("Timer is active");
```

### ToString()

Returns formatted elapsed time:

```csharp
Echo($"Time: {stopwatch}"); // Uses TimeSpan.ToString()
```

## Time Conversion

Space Engineers runs at **60 ticks per second**:

- **1 tick** ≈ 16.67 milliseconds
- **60 ticks** = 1 second
- **3,600 ticks** = 1 minute
- **216,000 ticks** = 1 hour

The `Elapsed` property automatically converts ticks to TimeSpan using this rate.

## Common Patterns

### 1. Simple Timer

```csharp
// In your class:
Stopwatch _timer;

public Program()
{
    _timer = new Stopwatch(this).Start();
    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

public void Main(string argument, UpdateType updateSource)
{
    if (_timer.Elapsed.TotalSeconds >= 10)
    {
        Echo("10 seconds have passed!");
        _timer.Restart();
    }
}
```

### 2. Cooldown System

```csharp
Stopwatch _weaponCooldown;

public void FireWeapon()
{
    if (_weaponCooldown.Elapsed.TotalSeconds < 5)
    {
        Echo("Weapon is cooling down!");
        return;
    }
    
    // Fire weapon
    Echo("Firing!");
    _weaponCooldown.Restart();
}
```

### 3. Track System Uptime

```csharp
Stopwatch _uptime;

public Program()
{
    _uptime = new Stopwatch(this);
    _uptime.Start();
}

public void Main(string argument, UpdateType updateSource)
{
    Echo($"System uptime: {_uptime.Elapsed:hh\\:mm\\:ss}");
}
```

### 4. Multi-Run Task Duration

```csharp
// Measure how long a task takes across multiple script runs
Stopwatch _taskTimer;
bool _taskRunning = false;

public void Main(string argument, UpdateType updateSource)
{
    if (argument == "start")
    {
        _taskTimer = new Stopwatch(this);
        _taskTimer.Start();
        _taskRunning = true;
    }
    
    if (_taskRunning)
    {
        // Do some work each run
        ProcessNextBatch();
        
        if (IsComplete())
        {
            _taskTimer.Stop();
            Echo($"Task took {_taskTimer.Elapsed.TotalSeconds:F1}s total");
            _taskRunning = false;
        }
    }
}
```

### 5. Timeout Detection

```csharp
public void WaitForDoorWithTimeout()
{
    var timeout = new Stopwatch(this);
    timeout.Start();
    
    while (door.Status != DoorStatus.Open && timeout.ElapsedTicks < 600) // 10 seconds
    {
        // Wait for door to open
        // NOTE: This only works if your script runs repeatedly!
    }
    
    if (door.Status != DoorStatus.Open)
        Echo("Door failed to open!");
}
```

## Time Formatting

Use standard TimeSpan format strings:

```csharp
var elapsed = stopwatch.Elapsed;

// Minutes and seconds: "01:23"
Echo(elapsed.ToString("mm\\:ss"));

// Minutes, seconds, milliseconds: "01:23.456"
Echo(elapsed.ToString("mm\\:ss\\.fff"));

// Hours, minutes, seconds: "1:23:45"
Echo(elapsed.ToString("h\\:mm\\:ss"));

// Total values
Echo($"{elapsed.TotalMilliseconds} ms");
Echo($"{elapsed.TotalSeconds:F2} seconds");
```

## Performance Characteristics

### Overhead

- **Creating**: Minimal - 3 fields (2 longs, 1 bool)
- **Start/Stop**: Very fast - simple field assignments
- **Reading ElapsedTicks**: Very fast - one arithmetic operation
- **Reading Elapsed**: Fast - includes floating-point conversion

### Timing Resolution

- **Granularity**: ~16.67ms (one game tick at 60 ticks/second)
- **Accuracy**: Depends on game simulation speed
- **Use Case**: Measuring time between script runs, not within a single run

### Best Practices

✓ **Use for game-time measurements** - Timers, cooldowns, durations across runs  
✓ **Stop when not timing** - Reduces unnecessary calculations  
✓ **Reuse instances** - Use Reset() or Restart() instead of creating new ones  

❌ **Don't use for performance profiling** - Use `Runtime.LastRunTimeMs` instead  
❌ **Don't expect sub-tick precision** - Resolution is limited to ~16.67ms  
❌ **Don't measure within a single script run** - Only measures between runs

## Implementation Details

The Stopwatch class uses the program's `Runtime.LifetimeTicks` property for accurate timing:

```csharp
public long ElapsedTicks => IsRunning
    ? _elapsedTicks + (_program.Runtime.LifetimeTicks - _startTicks)
    : _elapsedTicks;
```

- **When running**: Returns accumulated time plus current session
- **When stopped**: Returns only accumulated time
- **Tick rate**: Assumes 60 ticks/second for TimeSpan conversion

## Comparison with System.Diagnostics.Stopwatch

The API is intentionally similar to .NET's Stopwatch:

| Feature | Mal.Stopwatch | System.Stopwatch |
|---------|---------------|------------------|
| Start() | ✓ | ✓ |
| Stop() | ✓ | ✓ |
| Reset() | ✓ | ✓ |
| Restart() | ✓ | ✓ |
| ElapsedTicks | ✓ | ✓ |
| Elapsed | ✓ | ✓ |
| IsRunning | ✓ | ✓ |
| ElapsedMilliseconds | Use Elapsed.TotalMilliseconds | ✓ |
| Frequency | Fixed at 60 Hz | Varies by system |

## FAQ

**Q: How accurate is the timing?**  
A: Timing is based on game ticks (60 Hz). Resolution is ~16.67ms. Accuracy depends on stable simulation speed.

**Q: Can I use this to measure performance of my code?**  
A: No. This measures game time between script executions, not execution time within a single run. Use `Runtime.LastRunTimeMs` for performance measurement.

**Q: Can I use multiple stopwatches?**  
A: Yes! Create as many as needed. Each instance is independent.

**Q: Does simulation speed affect timing?**  
A: Yes. If the game runs slower than 60 FPS, elapsed time reflects actual game time, not real-world time.

**Q: What's the maximum time I can measure?**  
A: Practically unlimited. Uses `long` for ticks (±9.2 quintillion ticks or ±4.9 billion years at 60 Hz).

**Q: Why does the stopwatch not measure code execution within a single Main() call?**  
A: It uses `Runtime.LifetimeTicks`, which only increments between script runs, not during execution.

## Example: Complete Performance Monitor

```csharp
public partial class Program : MyGridProgram
{
    Stopwatch _totalRuntime;
    Stopwatch _lastAction;
    
    public Program()
    {
        _totalRuntime = new Stopwatch(this);
        _totalRuntime.Start();
        _lastAction = new Stopwatch(this);
        
        Runtime.UpdateFrequency = UpdateFrequency.Update10;
    }
    
    public void Main(string argument, UpdateType updateSource)
    {
        // Track how long since last action
        if (argument == "action")
        {
            Echo("Action triggered!");
            _lastAction.Restart();
        }
        
        // Report timing
        Echo("=== TIMING MONITOR ===\n");
        Echo($"Total Runtime: {_totalRuntime.Elapsed:hh\\:mm\\:ss}");
        Echo($"Last Action: {_lastAction.Elapsed:mm\\:ss} ago");
        
        // Note: Runtime.LastRunTimeMs shows actual execution time
        Echo($"\nScript Execution: {Runtime.LastRunTimeMs:F3}ms");
    }
}
```

## API Reference

### Constructor

- `Stopwatch(Program program)` - Creates a new stopped stopwatch

### Methods

- `Start()` - Starts or resumes timing. Returns this.
- `Stop()` - Stops timing, preserving elapsed time. Returns this.
- `Reset()` - Stops and clears elapsed time. Returns this.
- `Restart()` - Resets and immediately starts. Returns this.
- `ToString()` - Returns formatted elapsed time string

### Properties

- `ElapsedTicks` (long) - Total elapsed ticks
- `Elapsed` (TimeSpan) - Total elapsed time
- `IsRunning` (bool) - True if currently timing

---

*Documentation auto-generated from package metadata. Last updated: 2026-01-15*

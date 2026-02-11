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
# Stopwatch

A simple timing utility for Space Engineers scripts. Measures elapsed game time using `Runtime.LifetimeTicks`.

**Note:** This measures game time between script runs, not execution time within a single run. For performance profiling, use `Runtime.LastRunTimeMs` instead.

## Quickstart

1. Install the NuGet package `Mal.MdkScriptMixin.Stopwatch`
2. Create a stopwatch with `new Stopwatch(this)`
3. Use `Start()`, `Stop()`, `Reset()`, `Restart()` to control it
4. Read elapsed time via `ElapsedTicks` or `Elapsed`

## Usage

```csharp
var timer = new Stopwatch(this);
timer.Start();

// Later...
if (timer.Elapsed.TotalSeconds > 5)
{
    Echo("5 seconds elapsed");
    timer.Reset();
}
```

Useful for timers, cooldowns, tracking durations across multiple script executions, or anything where you need to measure how much game time has passed.

## API

- `Start()` - Start or resume timing
- `Stop()` - Pause timing
- `Reset()` - Clear elapsed time and stop
- `Restart()` - Reset and start
- `ElapsedTicks` - Raw tick count (long)
- `Elapsed` - TimeSpan (60 ticks/second)
- `IsRunning` - Whether currently timing

All methods return `this` for chaining.

---

*Documentation auto-generated from package metadata. Last updated: 2026-02-11*

using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        // Simple demonstration of the Stopwatch mixin
        // NOTE: Stopwatch measures GAME TIME (ticks), not execution time!
        
        Stopwatch _stopwatch;
        
        public Program()
        {
            // Create a stopwatch - it starts stopped at zero
            _stopwatch = new Stopwatch(this);
            
            // Run every 100 ticks so we can see the time change
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            // Control the stopwatch with simple commands
            if (argument == "start")
                _stopwatch.Start();
            else if (argument == "stop")
                _stopwatch.Stop();
            else if (argument == "reset")
                _stopwatch.Reset();
            
            // Show the current state
            Echo("=== STOPWATCH DEMO ===\n");
            Echo($"Game Time Elapsed: {_stopwatch.Elapsed:mm\\:ss\\.fff}");
            Echo($"Ticks Elapsed: {_stopwatch.ElapsedTicks}");
            Echo($"Running: {_stopwatch.IsRunning}\n");
            Echo("Commands: start, stop, reset\n");
            Echo("NOTE: This measures game time,");
            Echo("not execution time within a script run!");
        }
    }
}
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public class Program : MyGridProgram
    {
        public Program()
        {
            // Wrap the function to install the console
            using (Console.Install(this))
            {
                // Print some information to the console
                Console.Out.Print("Engineer Toolbox")
                    .Print("---")
                    .Print("Booting up...")
                    .Print("Ready.")
                    .Prompt();
            }
        }

        public void Save()
        {
            // Wrap the function to install the console
            using (Console.Install(this))
            {
                // Print some information to the console
                Console.Out.Print("Saving state...")
                    .Print("State saved.")
                    .Prompt();
            }
        }

        public void Main(string argument, UpdateType updateSource)
        {
            // Wrap the function to install the console
            using (Console.Install(this))
            {
                // Print some information to the console
                Console.Out.Print("Running main loop...")
                    .Print($"Argument: {argument}")
                    .Print($"Update Source: {updateSource}")
                    .Prompt();
            }
        }
    }
}
using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    internal partial class Program : MyGridProgram
    {
        bool _didRaiseCommand;
        string _raisedCommandGiven;
        readonly Cancellation _mainCrCancellation;

        public Program()
        {
            _mainCrCancellation = new Cancellation();
            Coroutines.Run(MainCr(_mainCrCancellation.Token));
        }

        public IEnumerator<When> MainCr(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Echo("Hello, World!");
                yield return When.Returning();
            }
        }

        public IEnumerator<When> SequenceCr()
        {
            Echo("Running sequence");
            Echo("Waiting for subsequence...");
            var result = new Result();
            yield return When.Completed(Coroutines.Run(SubsequenceCr(result)));
            Echo($"Result of subsequence: {result.Value}");
        }

        public IEnumerator<When> SubsequenceCr(Result result)
        {
            Echo("Running subsequence");
            yield return When.True(_ => _didRaiseCommand);
            result.Value = _raisedCommandGiven;
            Echo("Subsequence complete");
        }

        public void Main(string argument, UpdateType updateSource)
        {
            Coroutines.Main(argument, updateSource);

            if ((updateSource & UpdateType.Trigger) != 0)
            {
                if (string.Equals(argument, "raise", System.StringComparison.OrdinalIgnoreCase))
                    Coroutines.Run(SequenceCr());
                else if (string.Equals(argument, "command", System.StringComparison.OrdinalIgnoreCase))
                {
                    _didRaiseCommand = true;
                    _raisedCommandGiven = argument;
                }
                else if (string.Equals(argument, "cancel", System.StringComparison.OrdinalIgnoreCase))
                {
                    _mainCrCancellation.Cancel();
                }
            }
        }

        public class Result
        {
            public string Value;
        }
    }
}
using System;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    /// <summary>
    ///     The frequency a coroutine runs at.
    /// </summary>
    public enum CrFrequency
    {
        /// <summary>
        ///     A coroutine that runs at the highest frequency possible (every tick). Only use for short running coroutines or when
        ///     absolutely necessary for accuracy.
        /// </summary>
        Immediate = 1,

        /// <summary>
        ///     A coroutine that runs at a normal, safe frequency (every 10 ticks). Use this for most coroutines.
        /// </summary>
        Normal = 10,

        /// <summary>
        ///     A coroutine that runs at a slow frequency (every 100 ticks). Use this for coroutines that don't need to run often.
        /// </summary>
        Slow = 100
    }

    public struct When
    {
        /// <summary>
        ///     Returns when the given coroutine has completed. Checks at the given frequency.
        /// </summary>
        /// <param name="coroutineId">The coroutine to wait for. Easiest way is to pass in Coroutine.Run(TheCrToRun()) directly.</param>
        /// <param name="frequency">The frequency to check for completion.</param>
        /// <returns></returns>
        public static When Completed(ulong coroutineId, CrFrequency frequency = CrFrequency.Normal) => new When(frequency, 1, coroutineId);

        /// <summary>
        ///     Returns when the given condition is true. Checks at the given frequency.
        /// </summary>
        /// <param name="condition">The condition to check.</param>
        /// <param name="frequency">The frequency to check the condition.</param>
        /// <returns></returns>
        public static When True(Func<When, bool> condition, CrFrequency frequency = CrFrequency.Normal) => new When(frequency, c: condition);

        /// <summary>
        ///     Returns when the given condition is true. Checks at the given frequency.
        /// </summary>
        /// <param name="condition">The condition to check.</param>
        /// <param name="frequency">The frequency to check the condition.</param>
        /// <returns></returns>
        public static When True(Func<bool> condition, CrFrequency frequency = CrFrequency.Normal) => new When(frequency, c: _ => condition());

        /// <summary>
        ///     Returns at the next scheduled update at the given frequency.
        /// </summary>
        /// <remarks>
        ///     Warning: Be careful with <see cref="Sandbox.ModAPI.Ingame.UpdateType.Update1" /> Only use for short running
        ///     coroutines or when absolutely necessary for accuracy.
        /// </remarks>
        /// <param name="frequency">The frequency to wait for.</param>
        /// <returns></returns>
        public static When Returning(CrFrequency frequency = CrFrequency.Normal) => new When(frequency);

        /// <summary>
        ///     Returns at the next scheduled update, at the highest frequency possible.
        /// </summary>
        /// <remarks>
        ///     Warning: Be careful with <see cref="Sandbox.ModAPI.Ingame.UpdateType.Update1" /> Only use for short running
        ///     coroutines or when absolutely necessary for accuracy.
        /// </remarks>
        /// <returns></returns>
        public static When Ready() => new When(CrFrequency.Immediate);

        /// <summary>
        ///     How often to run the coroutine.
        /// </summary>
        public readonly UpdateType U;

        /// <summary>
        ///     Flag bits for special conditions, used internally by the scheduler.
        /// </summary>
        public readonly uint F;

        /// <summary>
        ///     Bit field for special conditions, used internally by the scheduler.
        /// </summary>
        public readonly ulong B;

        /// <summary>
        ///     An optional configured condition to check for.
        /// </summary>
        public readonly Func<When, bool> C;

        /// <summary>
        ///     Creates a new When object. It is recommended to use the static methods instead.
        /// </summary>
        public When(CrFrequency u, uint f = 0, ulong b = 0, Func<When, bool> c = null)
        {
            U = u == CrFrequency.Immediate ? UpdateType.Update1 : u == CrFrequency.Slow ? UpdateType.Update100 : UpdateType.Update10;
            F = f;
            B = b;
            C = c;
        }

        public UpdateFrequency GetUpdateFrequency() => U == UpdateType.Update1 ? UpdateFrequency.Update1 : U == UpdateType.Update100 ? UpdateFrequency.Update100 : UpdateFrequency.Update10;
    }
}
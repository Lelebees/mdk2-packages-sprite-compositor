using System;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public struct When
    {
        /// <summary>
        ///     Returns when the given coroutine has completed. Checks at the given frequency.
        /// </summary>
        /// <param name="coroutineId">The coroutine to wait for. Easiest way is to pass in Coroutine.Run(TheCrToRun()) directly.</param>
        /// <param name="frequency">The frequency to check for completion.</param>
        /// <returns></returns>
        public static When Completed(ulong coroutineId, UpdateType frequency = UpdateType.Update10) => new When(frequency, 1, coroutineId);

        /// <summary>
        ///     Returns when the given condition is true. Checks at the given frequency.
        /// </summary>
        /// <param name="condition">The condition to check.</param>
        /// <param name="frequency">The frequency to check the condition.</param>
        /// <returns></returns>
        public static When True(Func<When, bool> condition, UpdateType frequency = UpdateType.Update10) => new When(frequency, c: condition);

        /// <summary>
        ///     Returns when the given condition is true. Checks at the given frequency.
        /// </summary>
        /// <param name="condition">The condition to check.</param>
        /// <param name="frequency">The frequency to check the condition.</param>
        /// <returns></returns>
        public static When True(Func<bool> condition, UpdateType frequency = UpdateType.Update10) => new When(frequency, c: _ => condition());

        /// <summary>
        ///   Returns when the next update of the given type occurs.
        /// </summary>
        /// <param name="updateType"></param>
        /// <returns></returns>
        public static When Next(UpdateType updateType) => new When(updateType);
        
        /// <summary>
        /// Returns when the next update1 tick occurs.
        /// </summary>
        /// <remarks>
        /// Be careful with this method. Updating every tick is something to be deliberate about, as it can cause performance issues.
        /// </remarks>
        /// <returns></returns>
        public static When NextUpdate1() => new When(UpdateType.Update1);
        
        /// <summary>
        /// Returns when the next update10 tick occurs.
        /// </summary>
        /// <remarks>
        /// This is the most common update frequency to use.
        /// </remarks>
        /// <returns></returns>
        public static When NextUpdate() => new When(UpdateType.Update10);
        
        /// <summary>
        /// Returns when the next update100 tick occurs.
        /// </summary>
        /// <remarks>
        /// Use this for data polls where the frequency is not critical.
        /// </remarks>
        /// <returns></returns>
        public static When NextUpdate100() => new When(UpdateType.Update100);
        
        /// <summary>
        ///    Returns when the given amount of milliseconds has passed.
        /// </summary>
        /// <remarks>
        /// This method will never be perfectly accurate, as at best we can
        /// have a resolution of 1/60th of a second (16.67ms) due to the game tick rate,
        /// as well as the frequency at which this coroutine is checked. By default this is
        /// Update10 (every 10 ticks, or 166.7ms). If you need better accuracy, consider
        /// using Update1 frequency, but be aware of the potential performance implications
        /// </remarks>
        /// <param name="milliseconds"></param>
        /// <param name="frequency"></param>
        /// <returns></returns>
        public static When TimePassed(int milliseconds, UpdateType frequency = UpdateType.Update10)
        {
            return new When(frequency, b: unchecked((ulong)Coroutines.LifetimeTicks) + (ulong)(milliseconds * 60 / 1000), c: w =>
            {
                var target = (long)w.B;
                return Coroutines.LifetimeTicks >= target;
            });
        }
        
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
        public When(UpdateType u, uint f = 0, ulong b = 0, Func<When, bool> c = null)
        {
            U = u;
            F = f;
            B = b;
            C = c;
        }

        /// <summary>
        /// Gets the UpdateFrequency which corresponds to the registered UpdateType.
        /// </summary>
        /// <returns></returns>
        public UpdateFrequency GetUpdateFrequency() => U == UpdateType.Update1 ? UpdateFrequency.Update1 : U == UpdateType.Update100 ? UpdateFrequency.Update100 : U == UpdateType.Update10 ? UpdateFrequency.Update10 : UpdateFrequency.None;
    }
}
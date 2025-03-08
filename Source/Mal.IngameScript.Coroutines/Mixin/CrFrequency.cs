namespace IngameScript
{
    /// <summary>
    ///     The frequency a coroutine runs at.
    /// </summary>
    public enum CrFrequency
    {
        /// <summary>
        ///     Does not schedule ticks itself, but runs whenever the Main method is run, no matter the reason.
        /// </summary>
        Parasitic = 0,

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
}
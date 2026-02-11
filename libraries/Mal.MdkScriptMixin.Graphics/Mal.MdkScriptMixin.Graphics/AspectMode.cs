namespace IngameScript
{
    public enum AspectMode
    {
        /// <summary>
        ///     The content is displayed at its native size.
        /// </summary>
        Native,

        /// <summary>
        ///     The content is scaled to fit within the container while maintaining its aspect ratio.
        /// </summary>
        Fit,

        /// <summary>
        ///     The content is scaled to fill the entire container, potentially cropping parts of it, while maintaining its aspect
        ///     ratio.
        /// </summary>
        Fill,

        /// <summary>
        ///     The content is scaled to fit within the container, then expanded to fill any remaining space. Aspect ratio is lost.
        ///     Meant for fluid layouts.
        /// </summary>
        FitAndExpand,

        /// <summary>
        ///     The content is scaled to fill the container, then contracted to fit within it. Aspect ratio is lost. Meant for
        ///     fluid layouts.
        /// </summary>
        FillAndContract
    }
}

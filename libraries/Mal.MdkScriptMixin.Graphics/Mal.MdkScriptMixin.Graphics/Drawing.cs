using System;
using System.Collections.Generic;
using VRage.Game.GUI.TextPanel;

namespace IngameScript
{
    /// <summary>
    ///     Helper class for caching sprites with minimal overhead.
    ///     Simple components can draw directly, complex components use Drawing for caching.
    /// </summary>
    public class Drawing
    {
        readonly List<MySprite> _cachedSprites = new List<MySprite>();
        bool _isDirty = true;

        /// <summary>
        ///     Marks this drawing as needing regeneration.
        /// </summary>
        public void Invalidate() => _isDirty = true;

        /// <summary>
        ///     Draws using the provided draw action, caching sprites when dirty.
        /// </summary>
        /// <param name="dc">Drawing context</param>
        /// <param name="force">Force regeneration even if not dirty</param>
        /// <param name="drawAction">Action that performs the actual drawing</param>
        public void Draw(IDc dc, bool force, Action<IDc> drawAction)
        {
            if (force || _isDirty)
            {
                using (dc.BeginCapture(_cachedSprites)) drawAction(dc);
                _isDirty = false;
            }
            else
                dc.AddSprites(_cachedSprites);
        }
    }
}

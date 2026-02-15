using System;
using System.Collections.Generic;
using VRage.Game.GUI.TextPanel;

namespace IngameScript
{
    public class Drawing
    {
        readonly List<MySprite> _cachedSprites = new List<MySprite>();
        bool _isDirty = true;
        public IReadOnlyList<MySprite> Sprites => _cachedSprites;
        public void Invalidate() => _isDirty = true;
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

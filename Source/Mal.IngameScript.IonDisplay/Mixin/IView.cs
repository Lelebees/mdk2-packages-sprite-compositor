using System;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public interface IView
    {
        void BeginFrame(IContext context);
        void Draw(Action<MySprite> add, RectangleF viewport);
    }
}
using System;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public interface IView
    {
        void BeginFrame(IContext context);
        void Draw(DC dc);
    }
    
    public struct DC
    {
        public readonly Action<MySprite> Add;
        public readonly Action<RectangleF?> Clip;
        public readonly RectangleF Bounds;
        
        public DC(Action<MySprite> add, Action<RectangleF?> clip, RectangleF bounds)
        {
            Add = add;
            Clip = clip;
            Bounds = bounds;
        }
        
        public DC WithBounds(RectangleF bounds) => new DC(Add, Clip, bounds);
        public DC WithAdd(Action<MySprite> add) => new DC(add, Clip, Bounds);
        public DC WithClip(Action<RectangleF?> clip) => new DC(Add, clip, Bounds);
    }
}
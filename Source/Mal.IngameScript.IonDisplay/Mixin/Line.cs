using System;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public class Line : View
    {
        public Color Color { get; set; }
        public string PatternId { get; set; }
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }
        public float Thickness { get; set; }
        
        protected override void OnBeforeFrame()
        {
            Start = Vector2.Zero;
            End = Vector2.Zero;
            Color = Color.White;
            PatternId = "SquareSimple";
            Thickness = 1f;
        }

        protected override void OnDraw(Action<MySprite> add, RectangleF bounds, RectangleF viewport)
        {
            var start = bounds.Position + Start;
            var end = bounds.Position + End;
            
            var size = new Vector2((end - start).Length(), Thickness);
            var center = (start + end) / 2;
            var rotation = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
            add(new MySprite
            {
                Type = SpriteType.TEXTURE,
                Data = PatternId,
                Position = center,
                Size = size,
                Color = Color,
                RotationOrScale = rotation,
                Alignment = TextAlignment.CENTER
            });
        }
    }
}
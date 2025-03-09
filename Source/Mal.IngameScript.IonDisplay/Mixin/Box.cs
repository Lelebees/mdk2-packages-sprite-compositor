using System;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public class Box : View
    {
        float _rotation;
        float _rotationRad;
        public Color Color { get; set; }
        public string PatternId { get; set; }

        public float Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                _rotationRad = MathHelper.ToRadians(value);
            }
        }

        public Page Page { get; set; }

        protected override void OnBeforeFrame()
        {
            Color = Color.White;
            PatternId = "SquareSimple";
            Rotation = 0f;
        }

        protected override void OnDraw(Action<MySprite> add, RectangleF bounds)
        {
            add(new MySprite
            {
                Type = SpriteType.TEXTURE,
                Data = PatternId,
                Position = bounds.Center,
                Size = bounds.Size,
                Color = Color,
                RotationOrScale = _rotationRad,
                Alignment = TextAlignment.CENTER
            });
        }
    }
}
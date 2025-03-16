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
        public bool Mirror { get; set; }
        public bool Flip { get; set; }

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

        protected override void OnDraw(DC dc)
        {
            var size = dc.Bounds.Size;
            if (Mirror) size.X = -size.X;
            if (Flip) size.Y = -size.Y;
            dc.Add(new MySprite
            {
                Type = SpriteType.TEXTURE,
                Data = PatternId,
                Position = dc.Bounds.Center,
                Size = size,
                Color = Color,
                RotationOrScale = _rotationRad,
                Alignment = TextAlignment.CENTER
            });
        }
    }
}
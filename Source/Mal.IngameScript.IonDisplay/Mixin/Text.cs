using System;
using System.Text;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public class Text : View
    {
        static readonly StringBuilder M = new StringBuilder("M");
        static readonly StringBuilder Buffer = new StringBuilder();
        float _fontScale;
        string _measuredFontId;
        float _measuredFontSize;
        Vector2 _measuredSize;
        string _measuredValue;

        public TextAlignment Alignment = TextAlignment.LEFT;

        public Color Color = Color.White;

        public string FontId = "White";

        public float FontSize = 24f;

        public string Value;

        public override Vector2 Measure()
        {
            var surface = Context.Surface;
            var value = Value ?? "";
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_measuredFontSize != FontSize || _measuredFontId != FontId || _measuredValue != value)
            {
                _measuredFontSize = FontSize;
                _measuredFontId = FontId;
                _measuredValue = value;
                _fontScale =  FontSize / surface.MeasureStringInPixels(M, FontId, 1).Y;
                _measuredSize = surface.MeasureStringInPixels(Buffer.Clear().Append(value), FontId, _fontScale);
            }
            
            return _measuredSize;
        }

        protected override void OnBeforeFrame()
        {
            Alignment = TextAlignment.LEFT;
            Color = Color.White;
            FontId = "White";
            FontSize = 24f;
            Value = null;
        }

        protected override void OnDraw(Action<MySprite> add, RectangleF bounds, RectangleF viewport)
        {
            Measure();
            Vector2 position;
            switch (Alignment)
            {
                case TextAlignment.RIGHT:
                    position = new Vector2(bounds.Right, bounds.Y);
                    break;
                case TextAlignment.CENTER:
                    position = new Vector2(bounds.Center.X, bounds.Y);
                    break;
                default:
                    position = bounds.Position;
                    break;
            }
            add(new MySprite
            {
                Type = SpriteType.TEXT,
                Data = Value,
                Position = position,
                RotationOrScale = _fontScale,
                Color = Color,
                Alignment = Alignment,
                FontId = FontId,
                Size = bounds.Size
            });
        }
    }
}
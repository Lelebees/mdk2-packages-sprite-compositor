using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRageMath;

namespace IngameScript
{
    public class Text : View
    {
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
            var value = Value ?? "";
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (_measuredFontSize != FontSize || _measuredFontId != FontId || _measuredValue != value)
            {
                _measuredFontSize = FontSize;
                _measuredFontId = FontId;
                _measuredValue = value;
                _measuredSize = Context.MeasureString(new StringSegment(value), FontId, FontSize);
                _fontScale = _measuredSize.Y / FontSize;
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

        protected override void OnDraw(DC dc)
        {
            Measure();
            Vector2 position;
            switch (Alignment)
            {
                case TextAlignment.RIGHT:
                    position = new Vector2(dc.Bounds.Right, dc.Bounds.Y);
                    break;
                case TextAlignment.CENTER:
                    position = new Vector2(dc.Bounds.Center.X, dc.Bounds.Y);
                    break;
                default:
                    position = dc.Bounds.Position;
                    break;
            }

            dc.Add(new MySprite
            {
                Type = SpriteType.TEXT,
                Data = Value,
                Position = position,
                RotationOrScale = _fontScale,
                Color = Color,
                Alignment = Alignment,
                FontId = FontId,
                Size = dc.Bounds.Size
            });
        }
    }
}
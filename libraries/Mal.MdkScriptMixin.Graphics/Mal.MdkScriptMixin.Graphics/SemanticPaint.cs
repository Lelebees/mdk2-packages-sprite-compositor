using System;
using VRageMath;

namespace IngameScript
{
    public sealed class SemanticPaint : ProceduralPaint
    {
        public static readonly Color Error = new Color(220, 40, 40);
        public static readonly Color Warning = new Color(255, 190, 40);
        public static readonly Color Info = new Color(70, 160, 255);

        readonly IPaint _basePaint;
        readonly float _minSaturation, _minValueDistance;
        readonly Color _semanticColor;
        Color _lastBaseColor;
        string _lastBaseTexture;

        public SemanticPaint(IPaint basePaint, Color semanticColor, float minSaturation = 0.45f, float minValueDistance = 0.45f)
        {
            _basePaint = basePaint;
            _semanticColor = semanticColor;
            _minSaturation = MathHelper.Clamp(minSaturation, 0f, 1f);
            _minValueDistance = MathHelper.Clamp(minValueDistance, 0f, 1f);
        }

        protected override bool NeedsUpdate() => _basePaint.Color != _lastBaseColor || !ReferenceEquals(_basePaint.Texture, _lastBaseTexture);

        protected override void Update(ref Color color, ref string texture)
        {
            var baseColor = _basePaint.Color;
            var baseHsv = baseColor.ColorToHSV(); // X=H, Y=S, Z=V
            var semanticHsv = _semanticColor.ColorToHSV();
            baseHsv.X = MoveHue(baseHsv.X, semanticHsv.X);
            if (baseHsv.Y < _minSaturation) baseHsv.Y = _minSaturation;
            var dv = baseHsv.Z - baseColor.ColorToHSV().Z;
            if (Math.Abs(dv) < _minValueDistance)
            {
                if (baseHsv.Z < 0.5f)
                    baseHsv.Z = MathHelper.Clamp(baseHsv.Z + _minValueDistance, 0f, 1f);
                else
                    baseHsv.Z = MathHelper.Clamp(baseHsv.Z - _minValueDistance, 0f, 1f);
            }
            var rgb = baseHsv.HSVtoColor();
            color = new Color(rgb.R, rgb.G, rgb.B, baseColor.A);
            texture = _basePaint.Texture;
            _lastBaseColor = baseColor;
            _lastBaseTexture = texture;
        }

        static float MoveHue(float from, float to)
        {
            var d = to - from;
            if (d > 0.5f) d -= 1f;
            else if (d < -0.5f) d += 1f;
            var h = from + d;
            if (h < 0f) h += 1f;
            else if (h >= 1f) h -= 1f;
            return h;
        }
    }
}

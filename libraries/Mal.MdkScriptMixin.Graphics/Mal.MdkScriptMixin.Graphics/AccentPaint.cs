using System;
using VRageMath;

namespace IngameScript
{
    public sealed class AccentPaint : ProceduralPaint
    {
        readonly IPaint _basePaint;
        readonly Color _fallbackAccent;
        readonly float _graySatThreshold, _minSaturation, _minValueDistance;
        Color _lastBaseColor;
        string _lastBaseTexture;

        public AccentPaint(IPaint basePaint, Color fallbackAccent = default(Color), float graySatThreshold = 0.03f, float minSaturation = 0.45f, float minValueDistance = 0.45f)
        {
            _basePaint = basePaint;
            _fallbackAccent = fallbackAccent == default(Color) ? new Color(0, 180, 255) : fallbackAccent;
            _graySatThreshold = MathHelper.Clamp(graySatThreshold, 0f, 1f);
            _minSaturation = MathHelper.Clamp(minSaturation, 0f, 1f);
            _minValueDistance = MathHelper.Clamp(minValueDistance, 0f, 1f);
        }

        protected override bool NeedsUpdate() => _basePaint.Color != _lastBaseColor || !ReferenceEquals(_basePaint.Texture, _lastBaseTexture);

        protected override void Update(ref Color color, ref string texture)
        {
            var baseColor = _basePaint.Color;
            var baseHsv = baseColor.ColorToHSV();
            Vector3 accentHsv;
            if (baseHsv.Y <= _graySatThreshold)
            {
                accentHsv = _fallbackAccent.ColorToHSV();
                accentHsv.Z = baseHsv.Z;
            }
            else
            {
                accentHsv = baseHsv;
                accentHsv.X += 0.5f;
                if (accentHsv.X >= 1f)
                    accentHsv.X -= 1f;
            }

            if (accentHsv.Y < _minSaturation)
                accentHsv.Y = _minSaturation;
            var dv = accentHsv.Z - baseHsv.Z;
            if (Math.Abs(dv) < _minValueDistance)
            {
                if (baseHsv.Z < 0.5f)
                    accentHsv.Z = MathHelper.Clamp(baseHsv.Z + _minValueDistance, 0f, 1f);
                else
                    accentHsv.Z = MathHelper.Clamp(baseHsv.Z - _minValueDistance, 0f, 1f);
            }

            var rgb = accentHsv.HSVtoColor();

            color = new Color(rgb.R, rgb.G, rgb.B, baseColor.A);
            texture = _basePaint.Texture;

            _lastBaseColor = baseColor;
            _lastBaseTexture = texture;
        }
    }
}

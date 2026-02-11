using VRageMath;

namespace IngameScript
{
    public sealed class BackdropPaint : ProceduralPaint
    {
        readonly IPaint _base;
        readonly float _graySatThreshold, _maxSat, _minSat, _satScale, _valueDistance;
        Color _lastColor;
        string _lastTexture;

        public BackdropPaint(IPaint basePaint, float valueDistance = 0.55f, float satScale = 0.3f, float minSaturation = 0.08f, float maxSaturation = 0.22f, float graySatThreshold = 0.03f)
        {
            _base = basePaint;
            _valueDistance = MathHelper.Clamp(valueDistance, 0f, 1f);
            _satScale = MathHelper.Clamp(satScale, 0f, 1f);
            _minSat = MathHelper.Clamp(minSaturation, 0f, 1f);
            _maxSat = MathHelper.Clamp(maxSaturation, 0f, 1f);
            _graySatThreshold = MathHelper.Clamp(graySatThreshold, 0f, 1f);
        }

        protected override bool NeedsUpdate() => _base.Color != _lastColor || !ReferenceEquals(_base.Texture, _lastTexture);

        protected override void Update(ref Color color, ref string texture)
        {
            var bc = _base.Color;
            var hsv = bc.ColorToHSV();
            if (hsv.Y < _graySatThreshold) hsv.X = 0.55f;
            hsv.Y = MathHelper.Clamp(hsv.Y * _satScale, _minSat, _maxSat);
            hsv.Z = hsv.Z < 0.5f ? MathHelper.Clamp(hsv.Z + _valueDistance, 0f, 1f) : MathHelper.Clamp(hsv.Z - _valueDistance, 0f, 1f);
            var rgb = hsv.HSVtoColor();
            color = new Color(rgb.R, rgb.G, rgb.B, bc.A);
            texture = _base.Texture;

            _lastColor = bc;
            _lastTexture = texture;
        }
    }
}

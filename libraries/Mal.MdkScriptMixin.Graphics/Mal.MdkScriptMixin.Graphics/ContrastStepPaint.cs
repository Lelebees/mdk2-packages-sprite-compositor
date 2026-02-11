using VRageMath;

namespace IngameScript
{
    public sealed class ContrastStepPaint : ProceduralPaint
    {
        readonly float _amount;
        readonly IPaint _basePaint;
        readonly float _pivot;
        Color _lastColor;
        string _lastTexture;

        public ContrastStepPaint(IPaint basePaint, float amount, float pivot = 0.5f)
        {
            _basePaint = basePaint;
            _amount = amount < 0f ? 0f : amount;

            if (pivot < 0f) pivot = 0f;
            if (pivot > 1f) pivot = 1f;
            _pivot = pivot;
        }

        protected override bool NeedsUpdate() => _basePaint.Color != _lastColor || _basePaint.Texture != _lastTexture;

        protected override void Update(ref Color color, ref string texture)
        {
            var baseColor = _basePaint.Color;
            var hsv = baseColor.ColorToHSV();
            if (hsv.Z <= _pivot)
                hsv.Z = Clamp01(hsv.Z + _amount);
            else
                hsv.Z = Clamp01(hsv.Z - _amount);
            var rgb = hsv.HSVtoColor();
            color = new Color(rgb.R, rgb.G, rgb.B, baseColor.A);
            texture = _basePaint.Texture;
            _lastColor = baseColor;
            _lastTexture = texture;
        }

        static float Clamp01(float v)
        {
            if (v < 0f) return 0f;
            if (v > 1f) return 1f;
            return v;
        }
    }
}

using VRageMath;

namespace IngameScript
{
    public class AlphaPaint : IPaint
    {
        readonly float _alpha;
        readonly IPaint _basePaint;

        public AlphaPaint(IPaint basePaint, byte alpha)
        {
            _basePaint = basePaint;
            _alpha = alpha / 255f;
        }

        public AlphaPaint(IPaint basePaint, float alpha)
        {
            _basePaint = basePaint;
            _alpha = alpha;
        }

        public Color Color => _basePaint.Color.Alpha(_alpha);
        public string Texture => _basePaint.Texture;
    }
}

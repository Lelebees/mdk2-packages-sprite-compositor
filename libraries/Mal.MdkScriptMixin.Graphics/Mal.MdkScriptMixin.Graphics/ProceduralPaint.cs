using VRageMath;

namespace IngameScript
{
    public abstract class ProceduralPaint : IPaint
    {
        Color _color;
        string _texture;

        public Color Color
        {
            get
            {
                if (NeedsUpdate())
                    Update(ref _color, ref _texture);
                return _color;
            }
        }

        public string Texture
        {
            get
            {
                if (NeedsUpdate())
                    Update(ref _color, ref _texture);
                return _texture;
            }
        }

        protected abstract bool NeedsUpdate();
        protected abstract void Update(ref Color color, ref string texture);
    }
}

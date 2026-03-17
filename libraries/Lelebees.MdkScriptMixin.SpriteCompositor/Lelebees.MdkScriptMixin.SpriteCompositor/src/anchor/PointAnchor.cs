using VRageMath;

namespace IngameScript
{
    public struct PointAnchor : Anchor
    {
        private readonly Vector2 position;

        public PointAnchor(Vector2 position)
        {
            this.position = position;
        }

        public Vector2 GetPosition()
        {
            return position;
        }
    }
}
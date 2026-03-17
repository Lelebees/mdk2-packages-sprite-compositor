using VRageMath;

namespace IngameScript
{
    /// <summary>
    /// Classes conforming to this interface can be used as an anchor to rotate around in sprite composition.
    /// </summary>
    public interface Anchor
    {
        Vector2 GetPosition();
    }
}
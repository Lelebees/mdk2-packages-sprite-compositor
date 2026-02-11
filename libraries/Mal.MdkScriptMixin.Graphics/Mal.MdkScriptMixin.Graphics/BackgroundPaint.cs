using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace IngameScript
{
    public sealed class BackgroundPaint : IPaint
    {
        readonly IMyTextSurface _textSurface;

        public BackgroundPaint(IMyTextSurface textSurface, string texture = "SquareSimple")
        {
            _textSurface = textSurface;
            Texture = texture;
        }

        public Color Color => _textSurface.ScriptBackgroundColor;
        public string Texture { get; }
    }
}

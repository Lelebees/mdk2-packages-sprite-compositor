using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace IngameScript
{
    public sealed class ForegroundPaint : IPaint
    {
        readonly IMyTextSurface _textSurface;

        public ForegroundPaint(IMyTextSurface textSurface, string texture = "SquareSimple")
        {
            _textSurface = textSurface;
            Texture = texture;
        }

        public Color Color => _textSurface.ScriptForegroundColor;
        public string Texture { get; }
    }
}

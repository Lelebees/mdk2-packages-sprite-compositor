using VRageMath;

namespace IngameScript
{
    public class Paint : IPaint
    {
        public Paint(Color color)
        {
            Color = color;
            Texture = "SquareSimple";
        }

        public Paint(int r, int g, int b)
        {
            Color = new Color(r, g, b);
            Texture = "SquareSimple";
        }

        public Paint(string texture, Color color)
        {
            Texture = texture ?? "SquareSimple";
            Color = color;
        }

        public Color Color { get; }
        public string Texture { get; }
    }
}

using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace IngameScript
{
    public class Theme
    {
        public enum Key { Bg, Fg, AccentBg, AccentFg, HighlightBg, HighlightFg, ErrorBg, ErrorFg, WarningBg, WarningFg }
        readonly Color[] _colors = new Color[10];
        public Color Bg { get { return _colors[0]; } set { _colors[0] = value; } } 
        public Color Fg { get { return _colors[1]; } set { _colors[1] = value; } }
        public Color AccentBg { get { return _colors[2]; } set { _colors[2] = value; } }
        public Color AccentFg { get { return _colors[3]; } set { _colors[3] = value; } }
        public Color HighlightBg { get { return _colors[4]; } set { _colors[4] = value; } }
        public Color HighlightFg { get { return _colors[5]; } set { _colors[5] = value; } }
        public Color ErrorBg { get { return _colors[6]; } set { _colors[6] = value; } }
        public Color ErrorFg { get { return _colors[7]; } set { _colors[7] = value; } }
        public Color WarningBg { get { return _colors[8]; } set { _colors[8] = value; } }
        public Color WarningFg { get { return _colors[9]; } set { _colors[9] = value; } }
        public Color this[Key key] => _colors[(int)key];
        
        public void UpdateFrom(IMyTextSurface surface) => UpdateFrom(surface.ScriptBackgroundColor, surface.ScriptForegroundColor);

        public virtual void UpdateFrom(Color backgroundColor, Color foregroundColor)
        {
            _colors[0] = backgroundColor;
            _colors[1] = foregroundColor;
            _colors[2] = backgroundColor.GetComplementary();
            _colors[3] = foregroundColor.GetComplementary();
            _colors[4] = Color.Lerp(backgroundColor, foregroundColor, 0.3f);
            _colors[5] = Color.Lerp(backgroundColor, foregroundColor, 0.7f);
            _colors[6] = _colors[4].Colorize(Color.Red, 0.75f);
            _colors[7] = _colors[5].Colorize(Color.Red, 0.75f);
            _colors[8] = _colors[4].Colorize(Color.Yellow, 0.75f);
            _colors[9] = _colors[5].Colorize(Color.Yellow, 0.75f);
        }
    }
}
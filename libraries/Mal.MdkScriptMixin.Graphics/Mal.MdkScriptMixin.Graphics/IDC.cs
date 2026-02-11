using System;
using System.Collections.Generic;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRageMath;

namespace IngameScript
{
    public interface IDc
    {
        /// <summary>
        ///     Sets or gets the clipping rectangle for the drawing context.
        /// </summary>
        /// <remarks>
        ///     The clipping rectangle exists in device coordinates. Make sure you transform before setting it.
        /// </remarks>
        RectangleF ClipRect { get; set; }

        /// <summary>
        ///     Gets or sets the current transformation applied to all drawing operations.
        /// </summary>
        Transform Transform { get; set; }

        IDisposable Push();
        void Pop();

        IDc Sprite(string texture, Color color, RectangleF rect, float rotation = 0f);
        IDc Rect(IPaint paint, RectangleF rect);
        IDc Line(IPaint paint, Vector2 from, Vector2 to, float thickness, bool capped = false, string capTexture = "SemiCircle");
        IDc Text(IFont font, IPaint paint, string text, Vector2 position, float sizePx, TextAlignment alignment = TextAlignment.LEFT);
        Vector2 MeasureText(IFont font, string text, float sizePx);
        Vector2 MeasureText(IFont font, StringSegment text, float sizePx);

        void AddSprite(ref MySprite sprite);
        void AddSprites(List<MySprite> sprites);
        IDisposable BeginCapture(List<MySprite> targetList);
        void EndCapture();

        IFont GetFont(string fontName);
    }
}

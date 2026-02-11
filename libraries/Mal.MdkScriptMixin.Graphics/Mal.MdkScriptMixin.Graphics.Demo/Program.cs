using System;
using Sandbox.ModAPI.Ingame;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        readonly AccentPaint _accentPaint;
        readonly BackgroundPaint _bgPaint;
        readonly ContrastStepPaint _borderPaint;
        readonly SurfaceDc _dc;
        readonly ForegroundPaint _fgPaint;

        public Program()
        {
            var display = Me.GetSurface(0);
            display.ContentType = ContentType.SCRIPT;
            var viewport = new RectangleF(0, 0, 512, 512);
            RectangleF physicalViewport;
            _dc = SurfaceDc.CreateWithAspect(display, AspectMode.Fit, ref viewport, out physicalViewport);

            _fgPaint = new ForegroundPaint(display);
            _bgPaint = new BackgroundPaint(display);
            _accentPaint = new AccentPaint(_fgPaint);
            _borderPaint = new ContrastStepPaint(_fgPaint, 0.2f);

            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            var now = DateTime.Now;
            var hours = now.Hour % 12 + now.Minute / 60f;
            var minutes = now.Minute + now.Second / 60f;
            var seconds = now.Second + now.Millisecond / 1000f;

            using (_dc.BeginDraw())
            {
                var center = new Vector2(256, 256);

                _dc.Rect(_bgPaint, new RectangleF(0, 0, 512, 512));

                _dc.Sprite("Circle", _borderPaint.Color, new RectangleF(106, 106, 300, 300));
                _dc.Sprite("Circle", _bgPaint.Color, new RectangleF(116, 116, 280, 280));

                for (var i = 0; i < 12; i++)
                {
                    var angle = i * MathHelper.PiOver2 / 3f;
                    var pos = new Vector2(
                        center.X + (float)Math.Sin(angle) * 130f,
                        center.Y - (float)Math.Cos(angle) * 130f
                    );
                    _dc.Sprite("Circle", _fgPaint.Color, new RectangleF(pos.X - 4, pos.Y - 4, 8, 8));
                }

                DrawHand(center, hours / 12f * MathHelper.TwoPi, 70f, 8f, _fgPaint);
                DrawHand(center, minutes / 60f * MathHelper.TwoPi, 100f, 6f, _fgPaint);
                DrawHand(center, seconds / 60f * MathHelper.TwoPi, 110f, 3f, _accentPaint);

                _dc.Sprite("Circle", _fgPaint.Color, new RectangleF(246, 246, 20, 20));
            }
        }

        void DrawHand(Vector2 center, float angle, float length, float thickness, IPaint paint)
        {
            var endX = center.X + (float)Math.Sin(angle) * length;
            var endY = center.Y - (float)Math.Cos(angle) * length;
            _dc.Line(paint, center, new Vector2(endX, endY), thickness);
        }
    }
}
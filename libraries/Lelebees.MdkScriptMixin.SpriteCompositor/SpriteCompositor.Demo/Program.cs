using System.Linq;
using Sandbox.ModAPI.Ingame;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        private readonly IMyTextSurface surface;
        private readonly Sprite sunSprite;
        private readonly Sprite textLayer;
        private readonly RectangleF viewport;

        public Program()
        {
            // We prepare a screen like normal
            surface = Me.GetSurface(0);
            surface.ContentType = ContentType.SCRIPT;
            surface.Script = "";
            viewport = new RectangleF(
                (surface.TextureSize - surface.SurfaceSize) / 2f,
                surface.SurfaceSize
            );
            
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            // You can create basic sprites using the Sprites class. The Textures class contains texture name constants for basic shapes, for your writing convenience.
            var sunRayMiddle = Sprites.WithTexture(Textures.SquareSimple)
                .Size(20, 5)
                .Build();
            var sunRayCapRight = Sprites.WithTexture(Textures.SemiCircle)
                .Size(5, 20)
                // Rotations are performed using the Angle struct. Here we're using a predefined right angle (90 degrees, or 0.5 * PI)
                // You'll see how to create an arbitrary angle a little later in this demo.
                .Rotation(Angle.Right)
                .Position(9, 0)
                .Build();
            /* You can create mirrored copies of sprites by calling the Sprites.Mirrored(), Sprites.MirroredHorizontal() or Sprites.MirroredVertical() functions.
               By passing an anchor, the sprite will also move to the other side of that anchor.
               Mirroring is functionally equivalent to calling the Scale() function with -1 for the x, y or both axes.
               You'll see a Scale() invocation later in this demo */
            Sprite sunRayCapLeft = Sprites.Mirrored(sunRayCapRight, sunRayMiddle);
            // You can also duplicate a sprite you made earlier by calling Clone() on it.

            // It's possible to compose a new sprite from a collection of existing sprites and treat them as if they were one.
            var sunRay = Sprites.Compose(sunRayCapLeft, sunRayMiddle, sunRayCapRight);
            sunRay.Translate(50, 0);

            TextureSprite sunBody = Sprites.WithTexture(Textures.Circle)
                .Size(50, 50)
                .Build(); 
            
            // Since I don't feel like repeating the above steps for every sun ray, you can easily clone sprites in a circle! 
            var allRays = Sprites.RepeatRotated(sunRay, 12, sunBody).ToList();
            // The above function is also available for scaling and translation.
            allRays.Add(sunBody);
            sunSprite = Sprites.Compose(allRays.ToArray());
            sunSprite.SetColor(Color.Yellow);
            // You can extend the CompositeSprite class if you want to implement custom behavior, or override existing behavior.
            // If that is too limited you can also implement the Sprite interface directly.
            
            var praiseText = Sprites.WithText("Praise")
                .FontId("White")
                .Position(0, -150)
                .Build(); 
            /* Any sprite can be rotated using the following syntax.
               Specifying an anchor point will rotate the sprite around that point, in addition to rotating the sprite itself.
                 Text can also be rotated around an anchor point, but beware! SE does not support rotating the text itself.
               This will therefore not create slanted text! */
            praiseText.Rotate(Angle.FromDegrees(-45), sunSprite);
            
            var theSunText = Sprites.WithText("the sun")
                .FontId("White")
                .Position(0, 125)
                .Build(); 
            // Though we don't do it here, you can also use an arbitrary point in space as the anchor by creating a new PointAnchor().
            theSunText.Rotate(Angle.FromDegrees(-50), sunSprite);
            textLayer = Sprites.Compose(praiseText, theSunText);
            // You can also set an initial scale for text using the builder.
            // However, here we want to scale the distance the text has to the sun, so we supply the sun as the anchor for the scale operation.
            textLayer.Scale(1.5f, sunSprite);
            sunSprite.Scale(2);
            // It's also possible to scale X and Y separately.
        }

        public void Main(string argument, UpdateType updateSource)
        {
            textLayer.SetColor(surface.ScriptForegroundColor);
            var frame = surface.DrawFrame();
            // To simplify use, you can move the Sprite(s) so that 0,0 is the center of the viewport. 
            frame.AddRange(sunSprite.AsDrawableCollection(viewport));
            frame.AddRange(textLayer.AsDrawableCollection(viewport));
            frame.Dispose();
            // We can animate our sprite by applying transformations during runtime!
            sunSprite.Rotate(Angle.FromDegrees(1));
        }
    }
}
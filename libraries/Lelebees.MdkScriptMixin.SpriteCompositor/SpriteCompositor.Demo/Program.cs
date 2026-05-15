using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        private readonly IMyTextSurface surface;
        private readonly Sprite sunSprite;
        private readonly Sprite textGroup;
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
            // You can copy a sprite you made earlier and apply different operations to it
            TextureSprite sunRayCapLeft = (TextureSprite) sunRayCapRight.Clone();
            // You can mirror texture sprites by calling the Mirror(), MirrorHorizontal() or MirrorVertical() functions on them.
            // By passing an anchor, the sprite will also move to the other side of that anchor.
            sunRayCapLeft.Mirror(sunRayMiddle);
            // Mirroring is functionally equivalent to calling the Scale() function with -1 for the x, y or both axes.
            // You'll see a Scale() invocation later in this demo
            
            // It's possible to group sprites and treat them as if they were one.
            var sunRay = Sprites.Group(sunRayCapLeft, sunRayMiddle, sunRayCapRight);
            sunRay.Translate(50, 0);

            TextureSprite sunBody = Sprites.WithTexture(Textures.Circle)
                .Size(50, 50)
                .Build(); 
            
            // Since I don't feel like repeating the above steps for every sun ray, you can easily clone sprites in a circle! 
            var allRays = Sprites.RepeatRotated(sunRay, 12, sunBody).ToList();
            // You can create, adjust and add sprites after a group has been made by keeping a reference to a list of the group's sprites
            sunSprite = new SimpleSpriteGroup(allRays);
            /* If this simple sprite group doesn't fulfill your needs,
             you can create your own implementation by extending the SpriteGroup abstract class. 
               If you do, you must implement two methods: Clone() and GetChildren().
             The first method is expected to create a deep copy of the group.
             The second method expects to get a list of all sprites that make up the group. (not copies)
             This allows the parent class to apply operations such as rotations or translations without needing to know how the sprites are managed by you. 
               If you need even more control over the group, you should create your own class that implements the Sprite interface.
             Which allows you to override the existing transformation functions, and will likely be more performant than extending the existing SpriteGroup class. */
            allRays.Add(sunBody);
            sunSprite.SetColor(Color.Yellow);
            
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
            textGroup = Sprites.Group(praiseText, theSunText);
            // You can also set an initial scale for text using the builder.
            // However, here we want to scale the distance the text has to the sun, so we supply the sun as the anchor for the scale operation.
            textGroup.Scale(1.5f, sunSprite);
            // Any new operations will be applied to the newly grouped sprites as well, but previous operations will not be applied.
            sunSprite.Scale(2f);
            // It's also possible to scale X and Y separately.
        }

        public void Main(string argument, UpdateType updateSource)
        {
            textGroup.SetColor(surface.ScriptForegroundColor);
            var frame = surface.DrawFrame();
            // To simplify use, you can move the Sprite(s) so that 0,0 is the center of the viewport. 
            frame.AddRange(sunSprite.AsDrawableCollection(viewport));
            frame.AddRange(textGroup.AsDrawableCollection(viewport));
            frame.Dispose();
            // We can animate our sprite by applying transformations during runtime!
            sunSprite.Rotate(Angle.FromDegrees(1));
        }
    }
}
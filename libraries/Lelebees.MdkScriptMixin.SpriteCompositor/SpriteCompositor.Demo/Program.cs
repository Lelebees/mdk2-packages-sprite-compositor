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
        private readonly Sprite praiseText;
        private readonly Sprite theSunText;
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
            
            // Strictly speaking this should be an Update100, but for testing and/or animation purposes it's nice to use Update10.
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            var sunRayMiddle = TextureSprite.GetBuilder()
                .Texture("SquareSimple")
                .Size(20, 5)
                .Build();
            var sunRayCapRight = TextureSprite.GetBuilder()
                .Texture("SemiCircle")
                .Size(5, 20)
                .Rotation(Angle.Right)
                .Position(9, 0)
                .Build();
            // You can copy a sprite you made earlier and apply different operations to it
            var sunRayCapLeft = sunRayCapRight.Clone();
            // We're using the middle part of the sun ray as an anchor point for the rotation,
            // this allows us to flip the copied end to the left of the ray, without having to calculate how far it needs to go!
            sunRayCapLeft.Rotate(Angle.Straight, sunRayMiddle);
            
            // It's also possible to group sprites and treat them as if they were one.
            var sunRay = new SpriteGroup(new List<Sprite> { sunRayCapLeft, sunRayMiddle, sunRayCapRight });
            sunRay.Translate(50, 0);

            var sunBody = TextureSprite.GetBuilder()
                .Texture("Circle")
                .Size(50, 50)
                .Build(); 
            
            // Since we don't feel like repeating the above steps for every sun ray, you can easily clone sprites in a circle! 
            var allRays = Sprites.RepeatRotated(sunRay, 12, sunBody);
            
            // You can create, adjust and add sprites after a group has been made by keeping a reference to the list of the group's sprites
            allRays.Add(sunBody);
            sunSprite = new SpriteGroup(allRays);
            sunSprite.SetColor(Color.Yellow);
            
            praiseText = TextSprite.GetBuilder()
                .Text("Praise")
                .FontId("White")
                .Position(0, -150)
                .Scale(2f)
                .Build(); 
            // Text can also be rotated around an anchor point, but beware! SE does not support rotating the text itself. This will therefore not create slanted text!
            praiseText.Rotate(Angle.FromDegrees(-35), sunSprite);
            // You can also set an initial scale using the builder. However, here we want to scale the distance the text has to the sun, so we supply the sun as the anchor for the scale operation.
            praiseText.Scale(2f, sunSprite);
            theSunText = TextSprite.GetBuilder()
                .Text("the sun")
                .FontId("White")
                .Position(0, 125)
                .Build(); 
            // Though we don't do it here, you can also use an arbitrary point in space as the anchor by creating a new PointAnchor.
            theSunText.Rotate(Angle.FromDegrees(-40), sunSprite);
            theSunText.Scale(2f, sunSprite);
            
            // Any new operations will be applied to the newly grouped sprites as well, but previous operations will not be applied.
            sunSprite.Scale(2f);
            // It's also possible to scale X and Y separately. You can even mirror sprites by applying a scale of -1 in one or both directions!
        }

        public void Main(string argument, UpdateType updateSource)
        {
            var frame = surface.DrawFrame();
            // To simplify use, the Sprite(s) move so that 0,0 is the center of the viewport. 
            frame.AddRange(sunSprite.AsDrawableCollection(viewport));
            frame.AddRange(praiseText.AsDrawableCollection(viewport));
            frame.AddRange(theSunText.AsDrawableCollection(viewport));
            frame.Dispose();
            // We can animate our sprite by applying transformations during runtime!
            sunSprite.Rotate(Angle.FromDegrees(1));
        }
    }
}
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
        private readonly RectangleF viewport;

        public Program()
        {
            surface = Me.GetSurface(0);
            surface.ContentType = ContentType.SCRIPT;
            surface.Script = "";
            viewport = new RectangleF(
                (surface.TextureSize - surface.SurfaceSize) / 2f,
                surface.SurfaceSize
            );

            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            var sunRayMiddle = new TextureSprite("SquareSimple");
            sunRayMiddle.SetSize(new Vector2(20, 5));
            var sunRayCapRight = new TextureSprite("SemiCircle");
            sunRayCapRight.SetSize(new Vector2(5, 20));
            var sunRayCapLeft = sunRayCapRight.Clone();
            sunRayCapRight.Rotate(new Angle(90, AngleUnit.Degrees));
            sunRayCapRight.Translate(new Vector2(10, 0));
            sunRayCapLeft.Rotate(new Angle(-90, AngleUnit.Degrees));
            sunRayCapLeft.Translate(new Vector2(-10, 0));

            var sunRay = new SpriteGroup(new List<Sprite> { sunRayCapLeft, sunRayMiddle, sunRayCapRight });
            sunRay.Translate(new Vector2(50, 0));

            var sunBody = new TextureSprite("Circle");
            sunBody.SetSize(new Vector2(50, 50));
            var allRays = Sprites.RepeatRotated(sunRay, 12, sunBody);
            allRays.Add(sunBody);
            sunSprite = new SpriteGroup(allRays);
            sunSprite.SetColor(Color.Yellow);
        }

        public void Save()
        {
        }

        public void Main(string argument, UpdateType updateSource)
        {
            var frame = surface.DrawFrame();
            frame.AddRange(sunSprite.ToDrawableList(viewport));
            frame.Dispose();
        }
    }
}
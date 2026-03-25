using System;

namespace IngameScript
{
    /// <summary>
    /// An unambiguous angle for mathematical equations.
    /// </summary>
    public struct Angle
    {
        public static Angle Right = FromRadians(0.5 * Math.PI);
        public static Angle Straight = FromRadians(Math.PI);
        public static Angle Full = FromRadians(2 * Math.PI);
        
        private readonly double radians;

        private Angle(double radians)
        {
            // Normalize the angle to be within a single rotation
            if (radians >= 2 * Math.PI || radians <= -2 * Math.PI)
            {
                radians %= 2 * Math.PI;
            }
            this.radians = radians;
        }

        public double AsRadians()
        {
            return radians;
        }

        public double AsDegrees()
        {
            return radians * 180 / Math.PI;
        }

        public static Angle FromDegrees(double degrees)
        {
            return new Angle(degrees * Math.PI / 180);
        }

        public static Angle FromRadians(double radians)
        {
            return new Angle(radians);
        }
        
        public static Angle Add(Angle a, Angle b)
        {
            return FromRadians(a.AsRadians() + b.AsRadians());
        }
    }
}
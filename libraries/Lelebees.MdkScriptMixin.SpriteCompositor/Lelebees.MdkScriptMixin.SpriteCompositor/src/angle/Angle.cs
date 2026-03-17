using System;

namespace IngameScript
{
    /// <summary>
    /// An unambiguous angle for mathematical equations.
    /// </summary>
    public struct Angle
    {
        private readonly double radians;

        public Angle(double angle, AngleType type)
        {
            if (type != AngleType.Radians)
            {
                angle = angle * Math.PI / 180;
            }
            radians = angle;
        }

        public double AsRadians()
        {
            return radians;
        }

        public double AsDegrees()
        {
            return radians * 180 / Math.PI;
        }
    }
}
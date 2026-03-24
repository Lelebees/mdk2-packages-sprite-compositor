using System;

namespace IngameScript
{
    /// <summary>
    /// An unambiguous angle for mathematical equations.
    /// </summary>
    public struct Angle
    {
        private readonly double radians;

        public Angle(double angle, AngleUnit unit)
        {
            if (unit == AngleUnit.Degrees)
            {
                angle = angle * Math.PI / 180;
            }

            while (angle > 2 * Math.PI || angle < -2 * Math.PI)
            {
                if (angle > 0)
                {
                    angle -= 2 * Math.PI;
                }

                if (angle < 0)
                {
                    angle += 2 * Math.PI;
                }
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

        public static Angle Add(Angle a, Angle b)
        {
            return new Angle(a.AsRadians() + b.AsRadians(), AngleUnit.Radians);
        }
    }
}
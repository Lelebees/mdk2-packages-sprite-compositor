using System;

namespace IngameScript
{
    /// <summary>
    /// An unambiguous angle for mathematical equations.
    /// </summary>
    public struct Angle
    {
        /// <summary>
        /// A right angle, equal to 90 degrees or 1/2 * PI
        /// </summary>
        public static readonly Angle Right = new Angle(0.5 * Math.PI);
        /// <summary>
        /// A straight angle, equal to 180 degrees or 1 * PI
        /// </summary>
        public static readonly Angle Straight = new Angle(Math.PI);
        /// <summary>
        /// A full angle, equal to 360 degrees or 2 * PI
        /// </summary>
        public static readonly Angle Full = new Angle(2 * Math.PI);

        private readonly double radians;

        private Angle(double radians)
        {
            // Normalize the angle to be within a single rotation
            if (radians > 2 * Math.PI || radians < -2 * Math.PI)
            {
                radians %= 2 * Math.PI;
            }

            this.radians = radians;
        }
        /// <summary>
        /// This angle expressed in radians, positive or negative, limited to one full rotation (0 to 2 * PI)
        /// </summary>
        /// <returns>the angle in radians</returns>
        public double Radians() => radians;

        /// <summary>
        /// This angle expressed in degrees, positive or negative, limited to one full rotation (0 to 360)
        /// </summary>
        /// <returns>the angle in degrees</returns>
        public double Degrees() => radians * 180 / Math.PI;

        public static Angle FromDegrees(double degrees) => new Angle(degrees * Math.PI / 180);

        public static Angle FromRadians(double radians) => new Angle(radians);

        public static Angle operator +(Angle left, Angle right) => new Angle(left.radians + right.radians);

        public static Angle operator +(Angle angle) => angle;

        public static Angle operator -(Angle left, Angle right) => new Angle(left.radians - right.radians);

        public static Angle operator -(Angle angle) => new Angle(-angle.radians);

        public static Angle operator /(Angle nominator, double divisor) => new Angle(nominator.radians / divisor);

        public static Angle operator *(Angle angle, double multiplier) => new Angle(angle.radians * multiplier);

        public static bool operator <(Angle left, Angle right) => left.radians < right.radians;
        public static bool operator <=(Angle left, Angle right) => left.radians <= right.radians;

        public static bool operator >(Angle left, Angle right) => left.radians > right.radians;
        public static bool operator >=(Angle left, Angle right) => left.radians >= right.radians;
    }
}
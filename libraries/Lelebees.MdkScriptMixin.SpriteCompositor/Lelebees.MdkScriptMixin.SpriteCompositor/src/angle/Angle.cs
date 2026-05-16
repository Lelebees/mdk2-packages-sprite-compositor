/* Copyright (c) 2026 Lelebees
   This file is part of Sprite Compositor.

   Sprite Compositor is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation,
   either version 3 of the License, or (at your option) any later version.

   Sprite Compositor is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
   without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
   See the GNU Lesser General Public License for more details.

   You should have received a copy of the GNU Lesser General Public License along with Sprite Compositor.
   If not, see <https://www.gnu.org/licenses/>. */

using System;

namespace IngameScript
{
    /// <summary>
    /// An unambiguous angle for mathematical equations.
    /// </summary>
    public struct Angle
    {
        /// <summary>
        /// A right angle, equal to 90 degrees or 1/2 * PI.
        /// </summary>
        public static readonly Angle Right = new Angle(0.5 * Math.PI);
        /// <summary>
        /// A straight angle, equal to 180 degrees or 1 * PI.
        /// </summary>
        public static readonly Angle Straight = new Angle(Math.PI);
        /// <summary>
        /// A full angle, equal to 360 degrees or 2 * PI.
        /// </summary>
        public static readonly Angle Full = new Angle(2 * Math.PI);

        private Angle(double radians)
        {
            // Normalize the angle to be within a single rotation
            if (radians > 2 * Math.PI || radians < -2 * Math.PI)
            {
                radians %= 2 * Math.PI;
            }

            Radians = radians;
        }
        /// <summary>
        /// This angle expressed in radians, positive or negative, limited to one full rotation (0 to 2 * PI)
        /// </summary>
        public double Radians { get; }

        /// <summary>
        /// This angle expressed in degrees, positive or negative, limited to one full rotation (0 to 360)
        /// </summary>
        public double Degrees => Radians * 180 / Math.PI;

        public static Angle FromDegrees(double degrees) => new Angle(degrees * Math.PI / 180);

        public static Angle FromRadians(double radians) => new Angle(radians);

        public static Angle operator +(Angle left, Angle right) => new Angle(left.Radians + right.Radians);

        public static Angle operator +(Angle angle) => angle;

        public static Angle operator -(Angle left, Angle right) => new Angle(left.Radians - right.Radians);

        public static Angle operator -(Angle angle) => new Angle(-angle.Radians);

        public static Angle operator /(Angle nominator, double divisor) => new Angle(nominator.Radians / divisor);

        public static Angle operator *(Angle angle, double multiplier) => new Angle(angle.Radians * multiplier);

        public static bool operator <(Angle left, Angle right) => left.Radians < right.Radians;
        public static bool operator <=(Angle left, Angle right) => left.Radians <= right.Radians;

        public static bool operator >(Angle left, Angle right) => left.Radians > right.Radians;
        public static bool operator >=(Angle left, Angle right) => left.Radians >= right.Radians;
    }
}
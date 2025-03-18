using System;
using VRageMath;

namespace IngameScript
{
    public struct Thickness : IEquatable<Thickness>
    {
        public float Left { get; }
        public float Top { get; }
        public float Right { get; }
        public float Bottom { get; }
        public float HorizontalThickness => Left + Right;
        public float VerticalThickness => Top + Bottom;
        public Vector2 Size => new Vector2(Left + Right, Top + Bottom);
        public Vector2 Offset => new Vector2(Left, Top);

        public Thickness(float uniformSize)
        {
            Left = Top = Right = Bottom = uniformSize;
        }

        public Thickness(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public Thickness(float horizontal, float vertical)
        {
            Left = Right = horizontal;
            Top = Bottom = vertical;
        }

        public bool Equals(Thickness other) => Left.Equals(other.Left) && Top.Equals(other.Top) && Right.Equals(other.Right) && Bottom.Equals(other.Bottom);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Thickness && Equals((Thickness)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Left.GetHashCode();
                hashCode = (hashCode * 397) ^ Top.GetHashCode();
                hashCode = (hashCode * 397) ^ Right.GetHashCode();
                hashCode = (hashCode * 397) ^ Bottom.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Thickness left, Thickness right) => left.Equals(right);

        public static bool operator !=(Thickness left, Thickness right) => !left.Equals(right);

        public override string ToString() => $"{{ Left={Left}, Top={Top}, Right={Right}, Bottom={Bottom} }}";
    }
}
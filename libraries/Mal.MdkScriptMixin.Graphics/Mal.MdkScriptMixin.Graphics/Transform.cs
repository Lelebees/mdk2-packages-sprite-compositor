using System;
using VRageMath;

namespace IngameScript
{
    public struct Transform
    {
        public readonly Vector2 Translation;
        public readonly float Rotation, Scale;

        public Transform(Vector2 translation, float rotation, float scale)
        {
            Translation = translation;
            Rotation = rotation;
            Scale = scale;
        }

        public static readonly Transform Identity = new Transform(Vector2.Zero, 0f, 1f);
        public Transform WithoutTranslation() => new Transform(Vector2.Zero, Rotation, Scale);
        public Transform WithoutRotation() => new Transform(Translation, 0f, Scale);
        public Transform WithoutScale() => new Transform(Translation, Rotation, 1f);
        public Transform WithTranslation(Vector2 translation) => new Transform(translation, Rotation, Scale);
        public Transform WithRotation(float rotation) => new Transform(Translation, rotation, Scale);
        public Transform WithScale(float scale) => new Transform(Translation, Rotation, scale);
        public Transform Translate(Vector2 offset) => new Transform(Translation + offset, Rotation, Scale);
        public Vector2 TransformPoint(Vector2 p)
        {
            p *= Scale;
            if (Rotation != 0f)
            {
                var sin = (float)Math.Sin(Rotation);
                var cos = (float)Math.Cos(Rotation);
                p = new Vector2(p.X * cos - p.Y * sin, p.X * sin + p.Y * cos);
            }
            return p + Translation;
        }

        public Vector2 TransformVector(Vector2 v)
        {
            v *= Scale;
            if (Rotation != 0f)
            {
                var sin = (float)Math.Sin(Rotation);
                var cos = (float)Math.Cos(Rotation);
                v = new Vector2(v.X * cos - v.Y * sin, v.X * sin + v.Y * cos);
            }
            return v;
        }

        public RectangleF TransformAabb(RectangleF r)
        {
            var p0 = TransformPoint(new Vector2(r.X, r.Y));
            var p1 = TransformPoint(new Vector2(r.Right, r.Y));
            var p2 = TransformPoint(new Vector2(r.Right, r.Bottom));
            var p3 = TransformPoint(new Vector2(r.X, r.Bottom));
            var minX = Math.Min(Math.Min(p0.X, p1.X), Math.Min(p2.X, p3.X));
            var minY = Math.Min(Math.Min(p0.Y, p1.Y), Math.Min(p2.Y, p3.Y));
            var maxX = Math.Max(Math.Max(p0.X, p1.X), Math.Max(p2.X, p3.X));
            var maxY = Math.Max(Math.Max(p0.Y, p1.Y), Math.Max(p2.Y, p3.Y));
            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }

        public RectangleF TransformRectCenter(RectangleF r)
        {
            var tSize = r.Size * Scale;
            var tCenter = TransformPoint(r.Center) - tSize / 2f;
            return new RectangleF(tCenter, tSize);
        }

        // parent * child = apply child, then parent
        public static Transform operator *(Transform parent, Transform child)
        {
            var t = child.Translation * parent.Scale;
            if (parent.Rotation != 0f)
            {
                var sin = (float)Math.Sin(parent.Rotation);
                var cos = (float)Math.Cos(parent.Rotation);
                t = new Vector2(t.X * cos - t.Y * sin, t.X * sin + t.Y * cos);
            }
            return new Transform(parent.Translation + t, parent.Rotation + child.Rotation, parent.Scale * child.Scale);
        }

        public Transform Inverse()
        {
            if (Scale == 0f) throw new InvalidOperationException("Transform2D is not invertible when Scale is 0.");
            var invScale = 1f / Scale;
            var invRot = -Rotation;
            var t = Translation * invScale;
            if (Rotation != 0f)
            {
                var sin = Math.Sin(invRot);
                var cos = Math.Cos(invRot);
                t = new Vector2((float)(t.X * cos - t.Y * sin), (float)(t.X * sin + t.Y * cos));
            }
            return new Transform(-t, invRot, invScale);
        }

        public Vector2 InverseTransformPoint(Vector2 p)
        {
            if (Scale == 0f) throw new InvalidOperationException("Transform2D is not invertible when Scale is 0.");
            p -= Translation;
            if (Rotation != 0f)
            {
                var sin = Math.Sin(-Rotation);
                var cos = Math.Cos(-Rotation);
                p = new Vector2(
                    (float)(p.X * cos - p.Y * sin),
                    (float)(p.X * sin + p.Y * cos)
                );
            }
            p *= 1f / Scale;
            return p;
        }
    }
}

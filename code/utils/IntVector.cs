
namespace TerryBros.Utils
{
    public class IntVector3
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public IntVector3(Vector3 vector3)
        {
            X = (int) System.Math.Round(vector3.x);
            Y = (int) System.Math.Round(vector3.y);
            Z = (int) System.Math.Round(vector3.z);
        }

        public IntVector3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static IntVector3 operator +(IntVector3 a) => a;
        public static IntVector3 operator -(IntVector3 a) => new(-a.X, -a.Y, -a.Z);
        public static IntVector3 operator +(IntVector3 a, IntVector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static IntVector3 operator -(IntVector3 a, IntVector3 b) => a + (-b);
        public static IntVector3 operator /(IntVector3 a, int b) => new(a.X / b, a.Y / b, a.Z / b);
        public static Vector3 operator /(IntVector3 a, float b) => new(a.X / b, a.Y / b, a.Z / b);
        public static IntVector3 operator *(IntVector3 a, int b) => new(a.X * b, a.Y * b, a.Z * b);
        public static Vector3 operator *(IntVector3 a, float b) => new(a.X * b, a.Y * b, a.Z * b);

        public static IntVector3 Zero => new(0, 0, 0);

        public Vector3 ToVector3() => new(X, Y, Z);

        public bool Equals(IntVector3 intVector3) => X == intVector3.X && Y == intVector3.Y && Z == intVector3.Z || base.Equals(intVector3);

        public static explicit operator IntVector3(Vector3 vec) => new(vec);

        public static implicit operator Vector3(IntVector3 vec) => new(vec.X, vec.Y, vec.Z);
    }
}

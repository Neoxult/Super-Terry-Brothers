
namespace TerryBros.Utils
{
    public class IntVector3
    {
        public int x;
        public int y;
        public int z;

        public IntVector3(Vector3 vector3)
        {
            x = (int) vector3.x;
            y = (int) vector3.y;
            z = (int) vector3.z;
        }

        public IntVector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static IntVector3 operator +(IntVector3 a) => a;

        public static IntVector3 operator -(IntVector3 a)
        {
            return new IntVector3(-a.x, -a.y, -a.z);
        }

        public static IntVector3 operator +(IntVector3 a, IntVector3 b)
        {
            return new IntVector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static IntVector3 operator -(IntVector3 a, IntVector3 b) => a + (-b);

        public static IntVector3 operator /(IntVector3 a, int b)
        {
            return new IntVector3(a.x / b, a.y / b, a.z / b);
        }

        public static IntVector3 operator /(IntVector3 a, float b)
        {
            return new IntVector3((int) (a.x / b), (int) (a.y / b), (int) (a.z / b));
        }

        public static IntVector3 Zero => new IntVector3(0, 0, 0);

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public bool Equals(IntVector3 intVector3)
        {
            if (x == intVector3.x && y == intVector3.y && z == intVector3.z)
            {
                return true;
            }

            return base.Equals(intVector3);
        }
    }
}

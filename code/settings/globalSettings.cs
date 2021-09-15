namespace TerryBros.Settings
{
    //TODO: Move to a proper File
    public class intVector3
    {
        public intVector3(Vector3 vec)
        {
            x = (int) vec.x;
            y = (int) vec.y;
            z = (int) vec.z;
        }
        public intVector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public static intVector3 operator +(intVector3 a) => a;
        public static intVector3 operator -(intVector3 a)
        {
            return new intVector3(-a.x, -a.y, -a.z);
        }
        public static intVector3 operator +(intVector3 a, intVector3 b)
        {
            return new intVector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static intVector3 operator -(intVector3 a, intVector3 b) => a + (-b);
        public static intVector3 operator /(intVector3 a, int b)
        {
            return new intVector3(a.x / b, a.y / b, a.z / b);
        }
        public static intVector3 operator /(intVector3 a, float b)
        {
            return new intVector3((int) (a.x / b), (int) (a.y / b), (int) (a.z / b));
        }

        public int x;
        public int y;
        public int z;
    }
    public class intBBox
    {
        public intBBox(BBox bbox)
        {
            Mins = new intVector3(bbox.Mins);
            Maxs = new intVector3(bbox.Maxs);
            Center = new intVector3(bbox.Center);
        }
        public intBBox(intVector3 Mins, intVector3 Maxs)
        {
            this.Mins = Mins;
            this.Maxs = Maxs;
            Center = calculateCenter(Mins, Maxs);
        }
        private intVector3 calculateCenter(intVector3 Mins, intVector3 Maxs)
        {
            return Mins + (Maxs - Mins) / 2;
        }
        public intVector3 Mins;
        public intVector3 Maxs;
        public intVector3 Center;
    }

    public static partial class globalSettings
    {

        public static float blockSize = 20f;
        public static int visibleGroundBlocks = 3;

        /// <summary>
        /// World Bound given in number of Blocks
        /// </summary>
        public static intBBox worldBoundsBlocks
        {
            get => _worldBoundsBlocks;
            set
            {
                _worldBoundsBlocks = value;
                _worldBounds = new BBox(
                    GetBlockPosForGridCoordinates(value.Mins) - (forwardDir + 2 * upwardDir + lookDir) * blockSize/2,
                    GetBlockPosForGridCoordinates(value.Maxs) + (forwardDir + lookDir) * blockSize / 2
                    );
            }
        }
        public static BBox worldBounds
        {
            get => _worldBounds;
            private set
            {
                _worldBounds = value;
            }
        }

        public static Vector3 groundPos = Vector3.Zero;
        public static Vector3 forwardDir {
            get => _forwardDir;
            set
            {
                _forwardDir = value;
                updateLookDir();
            }
        }
        public static Vector3 upwardDir
        {
            get => _upwardDir;
            set
            {
                _upwardDir = value;
                updateLookDir();
            }
        }
        public static Vector3 lookDir
        {
            get => _lookDir;
            set { throw new System.InvalidOperationException("You cant set globalSettings.lookDir manually. It is calculated!"); }
        }

        /// <summary>
        /// 0, 0 is the first ground block.
        /// TODO: Maybe Move to a Library for more such functions.
        /// </summary>
        /// <param name="GridX"> Horizontal displacement </param>
        /// <param name="GridY"> Vertical displacement </param>
        /// <param name="GridZ"> Depth displacement </param>
        /// <returns></returns>
        public static Vector3 GetBlockPosForGridCoordinates(int GridX, int GridY, int GridZ = 0)
        {
            Vector3 centerBlockPos = groundPos;
            centerBlockPos -= upwardDir * blockSize / 2;
            centerBlockPos += GridX * forwardDir * blockSize;
            centerBlockPos += GridY * upwardDir * blockSize;
            centerBlockPos += GridZ * lookDir * blockSize;
            return centerBlockPos;
        }
        public static Vector3 GetBlockPosForGridCoordinates(intVector3 coordinate)
        {
            return GetBlockPosForGridCoordinates(coordinate.x, coordinate.y, coordinate.z);
        }

        //Private Fields
        private static intBBox _worldBoundsBlocks;
        private static BBox _worldBounds;

        private static Vector3 _forwardDir = Vector3.Forward;
        private static Vector3 _upwardDir = Vector3.Up;
        private static Vector3 _lookDir = Vector3.Cross(Vector3.Up, Vector3.Forward);

        private static void updateLookDir()
        {
            _lookDir = Vector3.Cross(_upwardDir, _forwardDir);
        }
    }
}

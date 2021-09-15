namespace TerryBros.Settings
{
    public static partial class globalSettings
    {

        public static float blockSize = 20f;
        public static int visibleGroundBlocks = 3;

        /// <summary>
        /// Minimal World Bound given in number of Blocks
        /// </summary>
        public static int WorldBoundMin = -30;

        /// <summary>
        /// Maximum World Bound given in number of Blocks
        /// </summary>
        public static int WorldBoundMax = 40;

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
        /// <returns></returns>
        public static Vector3 GetBlockPosForGridCoordinates(int GridX, int GridY)
        {
            Vector3 centerBlockPos = groundPos;
            centerBlockPos -= upwardDir * blockSize / 2;
            centerBlockPos += GridX * forwardDir * blockSize;
            centerBlockPos += GridY * upwardDir * blockSize;
            return centerBlockPos;
        }

        //Private Fields
        private static Vector3 _forwardDir = Vector3.Forward;
        private static Vector3 _upwardDir = Vector3.Up;
        private static Vector3 _lookDir = Vector3.Cross(Vector3.Up, Vector3.Forward);

        private static void updateLookDir()
        {
            _lookDir = Vector3.Cross(_upwardDir, _forwardDir);
        }
    }
}

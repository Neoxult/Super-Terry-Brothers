namespace TerryBros.Settings
{
    public static partial class globalSettings
    {

        public static float blockSize = 20f;
        public static int visibleGroundBlocks = 3;

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

using System;

using TerryBros.Utils;

namespace TerryBros.Settings
{
    public static partial class GlobalSettings
    {
        public static float BlockSize = 20f;

        //TODO: Get the real Height of the playermodel;
        public static float FigureHeight = 80;
        public static Vector3 GroundPos = Vector3.Zero;
        public static Vector3 ForwardDir
        {
            get => _forwardDir;
            set
            {
                _forwardDir = value;

                UpdateLookDir();
            }
        }
        private static Vector3 _forwardDir = Vector3.Forward;

        public static Vector3 UpwardDir
        {
            get => _upwardDir;
            set
            {
                _upwardDir = value;

                UpdateLookDir();
            }
        }
        private static Vector3 _upwardDir = Vector3.Up;

        public static Vector3 LookDir
        {
            get => _lookDir;
            set
            {
                throw new InvalidOperationException("You cant set globalSettings.lookDir manually. It is calculated!");
            }
        }
        private static Vector3 _lookDir = Vector3.Cross(Vector3.Up, Vector3.Forward);

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
            Vector3 centerBlockPos = GroundPos;
            centerBlockPos -= UpwardDir * BlockSize / 2;
            centerBlockPos += GridX * ForwardDir * BlockSize;
            centerBlockPos += GridY * UpwardDir * BlockSize;
            centerBlockPos += GridZ * LookDir * BlockSize;

            return centerBlockPos;
        }
        public static Vector3 GetBlockPosForGridCoordinates(IntVector3 coordinate)
        {
            return GetBlockPosForGridCoordinates(coordinate.x, coordinate.y, coordinate.z);
        }

        public static IntVector3 GetGridCoordinatesForBlockPos(Vector3 position)
        {
            Vector3 gridPos = new Vector3(position);
            gridPos -= GroundPos;
            gridPos += UpwardDir * BlockSize / 2;

            IntVector3 gridCoordinate = new IntVector3(
                (int) (gridPos.x / BlockSize),
                (int) (gridPos.z / BlockSize),
                (int) (gridPos.y / BlockSize)
            );

            return gridCoordinate;
        }

        /// <summary>
        /// The coordinate System is defined as X -> Horizontal, Y -> Vertical and Z -> Depth.
        /// Use this function to convert local to the actual used global coordinate system
        /// </summary>
        /// <param name="local">The local coordinates</param>
        /// <returns></returns>
        public static Vector3 ConvertLocalToGlobalCoordinates(Vector3 local)
        {
            return new Vector3(local.x * _forwardDir + local.y * _upwardDir + local.z * _lookDir);
        }

        /// <summary>
        /// The coordinate System is defined as X -> Horizontal, Y -> Vertical and Z -> Depth.
        /// Use this function to convert local to the actual used global coordinate system
        /// </summary>
        /// <param name="local">The local coordinates</param>
        /// <returns></returns>
        public static Vector3 ConvertLocalToGlobalCoordinates(IntVector3 local)
        {
            return ConvertLocalToGlobalCoordinates(new Vector3(local.x, local.y, local.z));
        }

        private static void UpdateLookDir()
        {
            _lookDir = Vector3.Cross(_upwardDir, _forwardDir);
        }
    }
}

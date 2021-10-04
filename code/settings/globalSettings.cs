using System;

using Sandbox;

using TerryBros.Utils;

namespace TerryBros.Settings
{
    public static partial class GlobalSettings
    {
        public static float BlockSize = 20f;

        //TODO: Get the real Height of the playermodel;
        public static float FigureHeight = 80f;
        public static Vector3 GroundPos = Vector3.Forward * 20f - Vector3.Up * 100f;
        public static Vector3 ForwardDir
        {
            get => _forwardDir;
            set
            {
                _forwardDir = value;

                UpdateTransformations();
            }
        }
        private static Vector3 _forwardDir = Vector3.Forward;

        public static Vector3 UpwardDir
        {
            get => _upwardDir;
            set
            {
                _upwardDir = value;

                UpdateTransformations();
            }
        }
        private static Vector3 _upwardDir = Vector3.Up;

        public static Vector3 LookDir
        {
            get
            {
                if (_doUpdateTransformations)
                {
                    UpdateTransformations();
                }
                return _lookDir;
            }
            set
            {
                throw new InvalidOperationException("You cant set globalSettings.lookDir manually. It is calculated!");
            }
        }
        private static Utils.Matrix LocalToGlobalTransformation
        {
            get
            {
                if (_doUpdateTransformations)
                {
                    UpdateTransformations();
                }
                return _localToGlobalTransformation;
            }
        }
        private static Utils.Matrix GlobalToLocalTransformation
        {
            get
            {
                if (_doUpdateTransformations)
                {
                    UpdateTransformations();
                }
                return _globalToLocalTransformation;
            }
        }

        private static bool _doUpdateTransformations = true;
        private static Vector3 _lookDir;
        private static Utils.Matrix _localToGlobalTransformation;
        private static Utils.Matrix _globalToLocalTransformation;

        //TODO: Maybe Move to a Library for more such functions.
        public static Vector3 GetBlockPosForLocalPos(Vector3 localPos)
        {
            return GetBlockPosForGridCoordinates((IntVector3)(localPos / BlockSize));
        }
        public static Vector3 GetBlockPosForGridCoordinates(Vector3 coordinate)
        {
            return ConvertLocalToGlobalCoordinates(coordinate * BlockSize);
        }
        public static Vector3 GetBlockPosForGridCoordinates(int GridX, int GridY, int GridZ = 0)
        {
            return GetBlockPosForGridCoordinates(new Vector3(GridX, GridY, GridZ));
        }
        public static IntVector3 GetGridCoordinatesForBlockPos(Vector3 position)
        {
            return (IntVector3) (ConvertGlobalToLocalCoordinates(position) / BlockSize);
        }

        /// <summary>
        /// The local coordinate System is defined as X' -> Horizontal, Y' -> Vertical and Z' -> Depth.
        /// And has his origin at Block-Center (0,0,0)
        /// Use this function to convert local to the actual used global coordinate system
        /// </summary>
        /// <param name="local">The local coordinates</param>
        /// <returns></returns>
        public static Vector3 ConvertLocalToGlobalCoordinates(Vector3 local)
        {
            local -= new Vector3(0, BlockSize / 2, 0);
            Vector3 global = LocalToGlobalTransformation * local;
            global += GroundPos;

            return global;
        }

        /// <summary>
        /// The global coordinate System is defined as X -> ForwardDir, Y -> UpwardDir and Z -> LookDir.
        /// And has his origin at Groundpos on top of a Block
        /// Use this function to convert the actual used global to the local coordinate system
        /// </summary>
        /// <param name="global">The global coordinates</param>
        /// <returns></returns>
        public static Vector3 ConvertGlobalToLocalCoordinates(Vector3 global)
        {
            global -= GroundPos;
            Vector3 local = GlobalToLocalTransformation * global;
            local += new Vector3(0, BlockSize / 2, 0);

            return local;
        }

        public static Vector3 ConvertLocalToGlobalScale(Vector3 localScale)
        {
            return LocalToGlobalTransformation * localScale;
        }
        public static Vector3 ConvertGlobalToLocalScale(Vector3 globalScale)
        {
            return GlobalToLocalTransformation * globalScale;
        }
        public static void UpdateTransformations()
        {
            _doUpdateTransformations = false;
            _lookDir = Vector3.Cross(_upwardDir, _forwardDir);
            _localToGlobalTransformation = new Utils.Matrix(_forwardDir , _upwardDir, _lookDir);
            _globalToLocalTransformation = new Utils.Matrix(_localToGlobalTransformation);
            _globalToLocalTransformation.Invert3x3();
        }
    }
}

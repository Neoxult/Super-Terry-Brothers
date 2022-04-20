using System;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.Settings;

namespace TerryBros
{
    public partial class SideScroller2DCamera : CameraMode
    {
        public float FreeCameraSpeed = 500f;
        public Vector3 LocalPosition
        {
            get => GlobalSettings.ConvertGlobalToLocalCoordinates(Position);
            set
            {
                Position = GlobalSettings.ConvertLocalToGlobalCoordinates(value);
            }
        }

        private readonly int _distanceInBlocks = 10;
        private readonly float _orthoSize = 0.3f;
        private readonly int _visibleGroundBlocks = 3;

        public SideScroller2DCamera()
        {
            Rotation = Rotation.LookAt(GlobalSettings.LookDir, GlobalSettings.UpwardDir);
            Ortho = true;
            Viewer = null;
        }
        public override void Update()
        {
            if (Local.Pawn is not Player player)
            {
                return;
            }

            BBox bBox = STBGame.CurrentLevel.LevelBoundsLocal;

            OrthoSize = Math.Min((bBox.Maxs.x - bBox.Mins.x) / Screen.Width, (bBox.Maxs.y - bBox.Mins.y) / Screen.Height);
            OrthoSize = Math.Min(_orthoSize, OrthoSize);

            Vector3 newPos = new(player.LocalPosition.x, GlobalSettings.BlockSize * 1, player.LocalPosition.z);
            newPos.y -= GlobalSettings.BlockSize * _visibleGroundBlocks;
            newPos.y += Screen.Height / 2 * OrthoSize;
            newPos.z -= GlobalSettings.BlockSize * _distanceInBlocks;

            try
            {
                // horizontal camera movement
                newPos.x = Math.Clamp(newPos.x, bBox.Mins.x + Screen.Width / 2 * OrthoSize, bBox.Maxs.x - Screen.Width / 2 * OrthoSize);

                // vertical camera movement
                newPos.y = Math.Clamp(newPos.y, bBox.Mins.y + Screen.Height / 2 * OrthoSize, bBox.Maxs.y - Screen.Height / 2 * OrthoSize);
            } catch (Exception) { }

            LocalPosition = newPos;
        }
    }
}

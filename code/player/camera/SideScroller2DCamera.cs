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

            float halfWidth = (float) Math.Round(Screen.Width / 2 * OrthoSize, 3);
            float halfHeight = (float) Math.Round(Screen.Height / 2 * OrthoSize, 3);

            OrthoSize = Math.Min((bBox.Maxs.x - bBox.Mins.x) / Screen.Width, (bBox.Maxs.y - bBox.Mins.y) / Screen.Height);
            OrthoSize = Math.Min(_orthoSize, OrthoSize);

            Vector3 newPos = new(player.LocalPosition);
            newPos.y -= GlobalSettings.BlockSize * _visibleGroundBlocks;
            newPos.y += halfHeight;
            newPos.z -= GlobalSettings.BlockSize * _distanceInBlocks;

            // horizontal camera movement
            newPos.x = Math.Clamp(newPos.x, bBox.Mins.x + halfWidth, bBox.Maxs.x - halfWidth);

            // vertical camera movement
            newPos.y = Math.Clamp(newPos.y, bBox.Mins.y + halfHeight, bBox.Maxs.y - halfHeight);

            LocalPosition = newPos;
        }
    }
}

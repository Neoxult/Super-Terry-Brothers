using System;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.Settings;
using TerryBros.Utils;
using TerryBros.Levels.Builder;

namespace TerryBros.Player.Camera
{
    public partial class SideScroller2DCamera : Sandbox.Camera
    {
        public float FreeCameraSpeed = 500f;
        public Vector3 LocalPosition
        {
            get => GlobalSettings.ConvertGlobalToLocalCoordinates(Pos);
            set { Pos = GlobalSettings.ConvertLocalToGlobalCoordinates(value); }
        }

        private int _distanceInBlocks = 10;
        private float _orthoSize = 0.3f;
        private int _visibleGroundBlocks = 3;

        public SideScroller2DCamera()
        {
            Rot = Rotation.LookAt(GlobalSettings.LookDir, GlobalSettings.UpwardDir);

            Ortho = true;

            Viewer = null;
        }
        public override void Update()
        {
            if (Local.Pawn is not TerryBrosPlayer player)
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

            // horizontal camera movement
            newPos.x = Math.Clamp(newPos.x, bBox.Mins.x + Screen.Width / 2 * OrthoSize, bBox.Maxs.x - Screen.Width / 2 * OrthoSize);

            // vertical camera movement
            newPos.y = Math.Clamp(newPos.y, bBox.Mins.y + Screen.Height / 2 * OrthoSize, bBox.Maxs.y - Screen.Height / 2 * OrthoSize);
                
            LocalPosition = newPos;
        }
    }
}

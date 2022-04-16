using System;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.Settings;

namespace TerryBros.Player.Camera
{
    public partial class SideScroller3DCamera : CameraMode
    {
        public Vector3 LocalPosition
        {
            get => GlobalSettings.ConvertGlobalToLocalCoordinates(Position);
            set
            {
                Position = GlobalSettings.ConvertLocalToGlobalCoordinates(value);
            }
        }

        private readonly int _distanceInBlocks = 100;
        private readonly int _visibleGroundBlocks = 3;

        private readonly bool _doHorizontalShift = false;
        private readonly float _shiftDegrees = 10;

        private readonly float _orthoSize = 0.3f;

        public SideScroller3DCamera()
        {
            Ortho = true;
            Viewer = null;
        }
        public override void Update()
        {
            if (Local.Pawn is not TerryBrosPlayer player)
            {
                return;
            }

            Rotation = Rotation.LookAt(GlobalSettings.LookDir, GlobalSettings.UpwardDir);

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

            //As shifts in multiple directions dont work good together, choose one
            //Note: Spherical coordinates would improve it,
            //but it still doesnt solve that there are either no horizontal or vertical Lines possible
            if (_doHorizontalShift)
            {
                // correct x Pos due to 3D-Effect
                newPos.x += (float) Math.Tan(-_shiftDegrees / 180f * Math.PI) * GlobalSettings.BlockSize * (_distanceInBlocks - 0.5f);
                newPos.x -= Screen.Width / 2 * OrthoSize * (1 - 1 / (float) Math.Cos(-_shiftDegrees / 180f * Math.PI));

                Rotation = Rotation.RotateAroundAxis(GlobalSettings.UpwardDir, -_shiftDegrees);
            }
            else
            {
                // correct y Pos due to 3D-Effect
                newPos.y += (float) Math.Tan(_shiftDegrees / 180f * Math.PI) * GlobalSettings.BlockSize * (_distanceInBlocks - 0.5f);
                newPos.y -= Screen.Height / 2 * OrthoSize * (1 - 1 / (float) Math.Cos(_shiftDegrees / 180f * Math.PI));

                Rotation = Rotation.RotateAroundAxis(GlobalSettings.LookDir, _shiftDegrees);
            }

            LocalPosition = newPos;
        }
    }
}

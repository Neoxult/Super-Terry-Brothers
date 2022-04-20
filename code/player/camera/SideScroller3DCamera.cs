using System;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.Settings;

namespace TerryBros
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
            if (Local.Pawn is not Player player)
            {
                return;
            }

            Rotation = Rotation.LookAt(GlobalSettings.LookDir, GlobalSettings.UpwardDir);

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

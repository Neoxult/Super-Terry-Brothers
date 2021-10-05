using System;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.Settings;

namespace TerryBros.Player.Camera
{
    public partial class SideScroller3DCamera : Sandbox.Camera
    {
        public Vector3 LocalPosition
        {
            get => GlobalSettings.ConvertGlobalToLocalCoordinates(Pos);
            set { Pos = GlobalSettings.ConvertLocalToGlobalCoordinates(value); }
        }

        private int distanceInBlocks = 100;
        private int visibleGroundBlocks = 3;

        private bool DoHorizontalShift = false;
        private float ShiftDegrees = 10;

        private float _orthoSize = 0.3f;

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

            Rot = Rotation.LookAt(GlobalSettings.LookDir, GlobalSettings.UpwardDir);

            BBox bBox = STBGame.CurrentLevel.LevelBoundsLocal;

            OrthoSize = Math.Min((bBox.Maxs.x - bBox.Mins.x) / Screen.Width, (bBox.Maxs.y - bBox.Mins.y) / Screen.Height);
            OrthoSize = Math.Min(_orthoSize, OrthoSize);

            Vector3 newPos = new(player.LocalPosition.x, GlobalSettings.BlockSize * 1, player.LocalPosition.z);
            newPos.y -= GlobalSettings.BlockSize * visibleGroundBlocks;
            newPos.y += Screen.Height / 2 * OrthoSize;
            newPos.z -= GlobalSettings.BlockSize * distanceInBlocks;

            // horizontal camera movement
            newPos.x = Math.Clamp(newPos.x, bBox.Mins.x + Screen.Width / 2 * OrthoSize, bBox.Maxs.x - Screen.Width / 2 * OrthoSize);

            // vertical camera movement
            newPos.y = Math.Clamp(newPos.y, bBox.Mins.y + Screen.Height / 2 * OrthoSize, bBox.Maxs.y - Screen.Height / 2 * OrthoSize);

            //As shifts in multiple directions dont work good together, choose one
            if (DoHorizontalShift)
            {
                // correct x Pos due to 3D-Effect
                newPos.x += (float) Math.Tan(-ShiftDegrees / 180f * Math.PI) * GlobalSettings.BlockSize * (distanceInBlocks - 0.5f);
                newPos.x -= Screen.Width / 2 * OrthoSize * (1 - 1 / (float) Math.Cos(-ShiftDegrees / 180f * Math.PI));

                Rot = Rot.RotateAroundAxis(GlobalSettings.UpwardDir, -ShiftDegrees);
            } else { 
                // correct y Pos due to 3D-Effect
                newPos.y += (float) Math.Tan(ShiftDegrees / 180f * Math.PI) * GlobalSettings.BlockSize * (distanceInBlocks - 0.5f);
                newPos.y -= Screen.Height / 2 * OrthoSize * (1 - 1 / (float) Math.Cos(ShiftDegrees / 180f * Math.PI));

                Rot = Rot.RotateAroundAxis(GlobalSettings.LookDir, ShiftDegrees);
            }

            LocalPosition = newPos;
        }
    }
}

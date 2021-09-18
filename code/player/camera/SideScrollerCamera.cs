using System;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.Settings;
using TerryBros.Utils;

namespace TerryBros.Player.Camera
{
    public partial class SideScrollerCamera : Sandbox.Camera
    {
        private int distanceInBlocks = 10;
        private float orthoSize = 0.3f;
        private int visibleGroundBlocks = 3;

        private float oldFactor = 0f;
        private bool wasForward = true;
        private float rotationFactor = 0f;

        public override void Update()
        {
            TerryBrosPlayer player = Local.Pawn as TerryBrosPlayer;

            if (player == null)
            {
                return;
            }

            Pos = new(player.Position.x, GlobalSettings.GroundPos.y, GlobalSettings.GroundPos.z);
            Pos -= GlobalSettings.UpwardDir * GlobalSettings.BlockSize * visibleGroundBlocks;
            Pos += GlobalSettings.UpwardDir * Screen.Height / 2 * orthoSize;
            Pos -= GlobalSettings.LookDir * GlobalSettings.BlockSize * distanceInBlocks;

            BBox bBox = STBGame.CurrentLevel.LevelBounds;

            if (Pos.x < bBox.Mins.x + Screen.Width / 2 * orthoSize)
            {
                Pos = new(bBox.Mins.x + Screen.Width / 2 * orthoSize, Pos.y, Pos.z);
            }
            else if (Pos.x > bBox.Maxs.x - Screen.Width / 2 * orthoSize)
            {
                Pos = new(bBox.Maxs.x - Screen.Width / 2 * orthoSize, Pos.y, Pos.z);
            }

            Rot = Rotation.LookAt(GlobalSettings.LookDir, GlobalSettings.UpwardDir);
            Rot = Rot.RotateAroundAxis(Vector3.Forward.Cross(Vector3.Up), -10f);

            if (player.Controller is Controller.MovementController movementController)
            {
                float moveDirectionFactor = Math.Clamp(movementController.MovedirectionChanged * 2f, 0f, 1f);

                if (movementController.Forward != wasForward)
                {
                    wasForward = movementController.Forward;
                    oldFactor += rotationFactor;
                }
                else if (oldFactor > 0f)
                {
                    oldFactor = Math.Max(oldFactor - moveDirectionFactor, 0f);
                }

                rotationFactor = Math.Clamp(moveDirectionFactor, -1f, 1f);
                rotationFactor -= oldFactor;

                Rot = Rot.RotateAroundAxis(GlobalSettings.UpwardDir, rotationFactor * (wasForward ? -10f : 10f));
            }

            Ortho = true;
            OrthoSize = orthoSize;

            Viewer = null;
        }
    }
}

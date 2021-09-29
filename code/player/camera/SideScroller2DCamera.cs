using System;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.Settings;

namespace TerryBros.Player.Camera
{
    public partial class SideScroller2DCamera : Sandbox.Camera
    {
        public float FreeCameraSpeed = 500f;

        private int _distanceInBlocks = 10;
        private float _orthoSize = 0.3f;
        private int _visibleGroundBlocks = 3;

        private float _oldFactor = 0f;
        private bool _wasForward = true;
        private float _rotationFactor = 0f;

        public override void Update()
        {
            if (Local.Pawn is not TerryBrosPlayer player)
            {
                return;
            }

            if (!player.IsInLevelBuilder)
            {
                Pos = new(player.Position.x, GlobalSettings.GroundPos.y, player.Position.z);
                Pos -= GlobalSettings.UpwardDir * GlobalSettings.BlockSize * _visibleGroundBlocks;
                Pos += GlobalSettings.UpwardDir * Screen.Height / 2 * _orthoSize;
                Pos -= GlobalSettings.LookDir * GlobalSettings.BlockSize * _distanceInBlocks;

                BBox bBox = STBGame.CurrentLevel.LevelBounds;

                // horizontal camera movement

                float val = 1f;
                
                if (Pos.x < bBox.Mins.x + Screen.Width / 2 * _orthoSize * val)
                {
                    Pos = new(bBox.Mins.x + Screen.Width / 2 * _orthoSize * val, Pos.y, Pos.z);
                }
                else if (Pos.x > bBox.Maxs.x - Screen.Width / 2 * _orthoSize * val)
                {
                    Pos = new(bBox.Maxs.x - Screen.Width / 2 * _orthoSize * val, Pos.y, Pos.z);
                }

                // vertical camera movement

                val = 1f;
                if (Pos.z < bBox.Mins.y + Screen.Height / 2 * _orthoSize * val)
                {
                    Pos = new(Pos.x, Pos.y, bBox.Mins.y + Screen.Height / 2 * _orthoSize * val);
                }
                else if (Pos.z > bBox.Maxs.y - Screen.Height / 2 * _orthoSize)
                {
                    Pos = new(Pos.x, Pos.y, bBox.Maxs.y - Screen.Height / 2 * _orthoSize);
                }
            }

            Rot = Rotation.LookAt(GlobalSettings.LookDir, GlobalSettings.UpwardDir);

            Ortho = true;
            OrthoSize = _orthoSize;

            Viewer = null;
        }
    }
}

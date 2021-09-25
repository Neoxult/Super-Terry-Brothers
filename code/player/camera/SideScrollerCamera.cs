using System;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.Settings;

namespace TerryBros.Player.Camera
{
    public partial class SideScrollerCamera : Sandbox.Camera
    {
        private int _distanceInBlocks = 10;
        private float _orthoSize = 0.3f;
        private int _visibleGroundBlocks = 3;

        private float _oldFactor = 0f;
        private bool _wasForward = true;
        private float _rotationFactor = 0f;

        public override void Update()
        {
            TerryBrosPlayer player = Local.Pawn as TerryBrosPlayer;

            if (player == null)
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

                float val = TerryBrosPlayer.stb_2D ? 1f : 1.15f;

                if (Pos.x < bBox.Mins.x + Screen.Width / 2 * _orthoSize * val)
                {
                    Pos = new(bBox.Mins.x + Screen.Width / 2 * _orthoSize * val, Pos.y, Pos.z);
                }
                else if (Pos.x > bBox.Maxs.x - Screen.Width / 2 * _orthoSize * val)
                {
                    Pos = new(bBox.Maxs.x - Screen.Width / 2 * _orthoSize * val, Pos.y, Pos.z);
                }

                // vertical camera movement

                val = TerryBrosPlayer.stb_2D ? 1f : 1.3f;

                if (Pos.z < bBox.Mins.z + Screen.Height / 2 * _orthoSize * val)
                {
                    Pos = new(Pos.x, Pos.y, bBox.Mins.z + Screen.Height / 2 * _orthoSize * val);
                }
                else if (Pos.z > bBox.Maxs.z - Screen.Height / 2 * _orthoSize)
                {
                    Pos = new(Pos.x, Pos.y, bBox.Maxs.z - Screen.Height / 2 * _orthoSize);
                }
            }

            Rot = Rotation.LookAt(GlobalSettings.LookDir, GlobalSettings.UpwardDir);

            // Camera 3d effect
            if (!player.IsInLevelBuilder && !TerryBrosPlayer.stb_2D)
            {
                Rot = Rot.RotateAroundAxis(Vector3.Forward.Cross(Vector3.Up), -10f);

                if (player.Controller is Controller.MovementController movementController)
                {
                    float moveDirectionFactor = Math.Clamp(movementController.MovedirectionChanged * 2f, 0f, 1f);

                    if (movementController.Forward != _wasForward)
                    {
                        _wasForward = movementController.Forward;
                        _oldFactor += _rotationFactor;
                    }
                    else if (_oldFactor > 0f)
                    {
                        _oldFactor = Math.Max(_oldFactor - moveDirectionFactor, 0f);
                    }

                    _rotationFactor = Math.Clamp(moveDirectionFactor, -1f, 1f);
                    _rotationFactor -= _oldFactor;

                    Rot = Rot.RotateAroundAxis(GlobalSettings.UpwardDir, _rotationFactor * (_wasForward ? -10f : 10f));
                }
            }

            Ortho = true;
            OrthoSize = _orthoSize;

            Viewer = null;
        }
    }
}

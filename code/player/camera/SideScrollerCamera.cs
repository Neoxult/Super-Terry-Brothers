using System;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.Settings;

namespace TerryBros.Player.Camera
{
    public partial class SideScrollerCamera : Sandbox.Camera
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

                if (!TerryBrosPlayer.camera2D)
                {
                    val += _orthoSize / 2;
                }

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

                if (!TerryBrosPlayer.camera2D)
                {
                    val += _orthoSize;
                }

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
            if (!player.IsInLevelBuilder && !TerryBrosPlayer.camera2D)
            {
                Rot = Rot.RotateAroundAxis(Vector3.Forward.Cross(Vector3.Up), -10f);

                //TODO: Discuss if we should just point in one direction all time
                // Currently the background is just a zoomed in skybox, which makes switching blurry as hell
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

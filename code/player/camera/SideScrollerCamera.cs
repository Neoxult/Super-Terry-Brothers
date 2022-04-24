using System;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.Settings;

namespace TerryBros
{
    public partial class SideScrollerCamera : CameraMode
    {
        public float FreeCameraSpeed = 500f;

        private readonly int _distanceInBlocks = 10;
        private readonly float _orthoSize = 0.3f;
        private readonly int _visibleGroundBlocks = 3;

        private float _oldFactor = 0f;
        private bool _wasForward = true;
        private float _rotationFactor = 0f;

        public override void Update()
        {
            if (Local.Pawn is not Player player)
            {
                return;
            }

            if (!Local.Client.GetValue("leveleditor", false))
            {
                Position = new(player.Position.x, GlobalSettings.GroundPos.y, player.Position.z);
                Position -= GlobalSettings.UpwardDir * GlobalSettings.BlockSize * _visibleGroundBlocks;
                Position += GlobalSettings.UpwardDir * Screen.Height / 2 * _orthoSize;
                Position -= GlobalSettings.LookDir * GlobalSettings.BlockSize * _distanceInBlocks;

                BBox bBox = STBGame.CurrentLevel.LevelBounds;

                // horizontal camera movement

                float val = 1f;

                if (!Player.Camera2D)
                {
                    val += _orthoSize / 2;
                }

                if (Position.x < bBox.Mins.x + Screen.Width / 2 * _orthoSize * val)
                {
                    Position = new(bBox.Mins.x + Screen.Width / 2 * _orthoSize * val, Position.y, Position.z);
                }
                else if (Position.x > bBox.Maxs.x - Screen.Width / 2 * _orthoSize * val)
                {
                    Position = new(bBox.Maxs.x - Screen.Width / 2 * _orthoSize * val, Position.y, Position.z);
                }

                // vertical camera movement

                val = 1f;

                if (!Player.Camera2D)
                {
                    val += _orthoSize;
                }

                if (Position.z < bBox.Mins.z + Screen.Height / 2 * _orthoSize * val)
                {
                    Position = new(Position.x, Position.y, bBox.Mins.z + Screen.Height / 2 * _orthoSize * val);
                }
                else if (Position.z > bBox.Maxs.z - Screen.Height / 2 * _orthoSize)
                {
                    Position = new(Position.x, Position.y, bBox.Maxs.z - Screen.Height / 2 * _orthoSize);
                }
            }

            Rotation = Rotation.LookAt(GlobalSettings.LookDir, GlobalSettings.UpwardDir);

            // Camera 3d effect
            if (!Local.Client.GetValue("leveleditor", false) && !Player.Camera2D)
            {
                Rotation = Rotation.RotateAroundAxis(Vector3.Forward.Cross(Vector3.Up), -10f);

                //TODO: Discuss if we should just point in one direction all time
                // Currently the background is just a zoomed in skybox, which makes switching blurry as hell
                if (player.Controller is MovementController movementController)
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

                    Rotation = Rotation.RotateAroundAxis(GlobalSettings.UpwardDir, _rotationFactor * (_wasForward ? -10f : 10f));
                }
            }

            Ortho = true;
            OrthoSize = _orthoSize;

            Viewer = null;
        }
    }
}

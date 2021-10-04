using System;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.Settings;
using TerryBros.Utils;
using TerryBros.Levels.Builder;

namespace TerryBros.Player.Camera
{
    public partial class LevelEditorCamera : Sandbox.Camera
    {
        public float FreeCameraSpeed = 500f;
        public Vector3 LocalPosition
        {
            get => GlobalSettings.ConvertGlobalToLocalCoordinates(Pos);
            set { Pos = GlobalSettings.ConvertLocalToGlobalCoordinates(value); }
        }

        private int DistanceInBlocks = 10;
        private float _orthoSize = 0.3f;
        private float _zoomDistance = 0.01f;
        private int _visibleGroundBlocks = 3;

        private IntVector3 _oldGrid = IntVector3.Zero;
        private bool _isDrawing = false;

        public LevelEditorCamera()
        {
            Pos = GlobalSettings.GroundPos;
            LocalPosition -= new Vector3(0, 0, GlobalSettings.BlockSize * DistanceInBlocks);
            Ortho = true;
            Viewer = null;
            OrthoSize = _orthoSize;
            Rot = Rotation.LookAt(GlobalSettings.LookDir, GlobalSettings.UpwardDir);
        }

        public override void Update()
        {
            Vector3 cameraPos = Pos;
            cameraPos.x -= Screen.Width * (OrthoSize / 2f);
            cameraPos.z += Screen.Height * (OrthoSize / 2f);

            Vector2 mousePosition = Mouse.Position * OrthoSize;
            Vector3 mousePos = new Vector3(mousePosition.x + cameraPos.x, 0, cameraPos.z - mousePosition.y);

            Vector3 BlockSize = GlobalSettings.BlockSize / 2f;
            IntVector3 intVector3 = GlobalSettings.GetGridCoordinatesForBlockPos(mousePos);
            Vector3 vector3 = GlobalSettings.GetBlockPosForGridCoordinates(intVector3);

            DebugOverlay.Box(vector3 - BlockSize, vector3 + BlockSize, Color.Red);
            DebugOverlay.Box(mousePos - BlockSize, mousePos + BlockSize, Color.Orange);
            DebugOverlay.Box(mousePos - new Vector3(2f, 10f, 2f), mousePos + new Vector3(2f, 10f, 2f), Color.Blue);

            DebugOverlay.Box(cameraPos.WithY(0f) + new Vector3(0, -10, 0), cameraPos.WithY(0f) + new Vector3(Screen.Width * OrthoSize, 10, -Screen.Height * OrthoSize), Color.Green);

            if (UI.LevelBuilder.Builder.Instance.IsMouseDown && !_oldGrid.Equals(intVector3))
            {
                _oldGrid = intVector3;
                _isDrawing = true;

                if (UI.LevelBuilder.Builder.Instance.IsLeftMouseButtonDown)
                {
                    Editor.ServerCreateBlock(vector3, UI.LevelBuilder.Builder.Instance.SelectedBlockData.Name);
                }
                else if (UI.LevelBuilder.Builder.Instance.IsRightMouseButtonDown)
                {
                    Editor.ServerDeleteBlock(vector3);
                }
            }
            else if (_isDrawing && !UI.LevelBuilder.Builder.Instance.IsMouseDown)
            {
                _oldGrid = IntVector3.Zero;
                _isDrawing = false;
            }
            Vector3 newPos = LocalPosition;

            newPos.x -= Input.Left * FreeCameraSpeed * Time.Delta;
            newPos.y += Input.Forward * FreeCameraSpeed * Time.Delta;

            LocalPosition = newPos;

            OrthoSize = Math.Clamp(OrthoSize - Input.MouseWheel * _zoomDistance, 0.1f, 2f);
        }
    }
}

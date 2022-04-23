using Sandbox;

using TerryBros.Settings;
using TerryBros.Utils;
using TerryBros.UI.LevelBuilder;

namespace TerryBros
{
    public partial class Player
    {
        public bool IsInLevelBuilder { get; set; }

        private IntVector3 _oldGrid = IntVector3.Zero;
        private bool _isDrawing = false;

        private void SimulateLevelEditing()
        {
            if (!IsInLevelBuilder || CameraMode is not LevelEditorCamera camera)
            {
                return;
            }

            Vector3 screenPos = GlobalSettings.ConvertGlobalToLocalCoordinates(camera.Position);
            float orthoSize = camera.OrthoSize;
            screenPos /= orthoSize;

            screenPos.x -= Screen.Width * 0.5f;
            screenPos.y += Screen.Height * 0.5f;

            Vector3 mousePos = new Vector3(screenPos.x + Mouse.Position.x, screenPos.y - Mouse.Position.y, 0) * orthoSize;
            Vector3 blockPos = PreviewAndGetBlockPlacement(mousePos);

            CreateBlockOnMouseEvent(blockPos);
        }

        public void EnableLevelEditor(bool enable)
        {
            IsInLevelBuilder = enable;

            if (!Host.IsServer)
            {
                return;
            }

            if (enable)
            {
                (Controller as MovementController).IsFreeze = true;
                CameraMode = new LevelEditorCamera();

                EnableDrawing = false;
            }
            else
            {
                (Controller as MovementController).IsFreeze = false;

                ClearCollisionLayers();
                AddCollisionLayer(CollisionLayer.Solid);
                AddCollisionLayer(CollisionLayer.PhysicsProp);

                EnableDrawing = true;

                Respawn();
            }
        }

        private static Vector3 PreviewAndGetBlockPlacement(Vector3 localPosition)
        {
            Vector3 mousePos = GlobalSettings.ConvertLocalToGlobalCoordinates(localPosition);
            Vector3 halfBlockSize = GlobalSettings.BlockSize / 2f;

            DebugOverlay.Box(mousePos - halfBlockSize, mousePos + halfBlockSize, Color.Orange);

            Vector3 blockPos = GlobalSettings.GetBlockPosForLocalPos(localPosition);

            DebugOverlay.Box(blockPos - halfBlockSize, blockPos + halfBlockSize, Color.Red);

            return blockPos;
        }

        private void CreateBlockOnMouseEvent(Vector3 pos)
        {
            if (BuildPanel.Instance == null)
            {
                return;
            }

            IntVector3 gridCoord = GlobalSettings.GetGridCoordinatesForBlockPos(pos);
            BuildPanel buildPanel = BuildPanel.Instance;

            if (buildPanel.IsMouseDown && !_oldGrid.Equals(gridCoord))
            {
                _oldGrid = gridCoord;
                _isDrawing = true;

                if (buildPanel.IsLeftMouseButtonDown)
                {
                    Levels.Editor.ServerCreateBlock(pos, buildPanel.SelectedAsset.Name);
                }
                else if (buildPanel.IsRightMouseButtonDown)
                {
                    Levels.Editor.ServerDeleteBlock(pos);
                }
            }
            else if (_isDrawing && !buildPanel.IsMouseDown)
            {
                _oldGrid = IntVector3.Zero;
                _isDrawing = false;
            }
        }
    }
}

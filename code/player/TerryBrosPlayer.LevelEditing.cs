using Sandbox;

using TerryBros.Settings;
using TerryBros.Utils;
using TerryBros.UI.LevelBuilder;

namespace TerryBros.Player
{
    using Camera;

    public partial class TerryBrosPlayer
    {
        public bool IsInLevelBuilder { get; private set; }

        private IntVector3 oldGrid = IntVector3.Zero;
        private bool isDrawing = false;

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

            if (Host.IsServer)
            {
                if (enable)
                {
                    //TODO: Solve Prediction error for Controller and Camera!!
                    Controller = null;
                    CameraMode = new LevelEditorCamera();

                    EnableAllCollisions = false;
                    EnableDrawing = false;
                }
                else
                {
                    Respawn();
                }
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

        private void CreateBlockOnMouseEvent(Vector3 BlockPos)
        {
            if (BuildPanel.Instance == null) return;

            IntVector3 gridCoord = GlobalSettings.GetGridCoordinatesForBlockPos(BlockPos);
            BuildPanel buildPanel = BuildPanel.Instance;

            if (buildPanel.IsMouseDown && !oldGrid.Equals(gridCoord))
            {
                oldGrid = gridCoord;
                isDrawing = true;

                if (buildPanel.IsLeftMouseButtonDown)
                {
                    Levels.Builder.Editor.ServerCreateBlock(BlockPos, buildPanel.SelectedBlockData.Name);
                }
                else if (buildPanel.IsRightMouseButtonDown)
                {
                    Levels.Builder.Editor.ServerDeleteBlock(BlockPos);
                }
            }
            else if (isDrawing && !buildPanel.IsMouseDown)
            {
                oldGrid = IntVector3.Zero;
                isDrawing = false;
            }
        }
    }
}

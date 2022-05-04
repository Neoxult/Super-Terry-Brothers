using System.Collections.Generic;

using Sandbox;

using TerryBros.Settings;
using TerryBros.UI.LevelBuilder;
using TerryBros.Utils;

namespace TerryBros
{
    public struct StackData
    {
        public Vector3[] Positions { get; set; }
        public string BlockName { get; set; }

        public StackData(Vector3[] positions, string blockName)
        {
            Positions = positions;
            BlockName = blockName;
        }
    }

    public partial class Player
    {
        public bool IsDrawing
        {
            get => _isDrawing;
            set
            {
                _mousePos = Vector3.Zero;
                _isDrawing = value;
            }
        }
        private bool _isDrawing = false;
        private Vector3 _mousePos = Vector3.Zero;
        private IntVector3 _oldGridPos = IntVector3.Zero;

        public Stack<StackData> Stack { get; set; } = new();

        private void SimulateLevelEditing()
        {
            if (!Client.GetValue("leveleditor", false) || CameraMode is not LevelEditorCamera camera)
            {
                return;
            }

            Vector3 screenPos = GlobalSettings.ConvertGlobalToLocalCoordinates(camera.Position);
            float orthoSize = camera.OrthoSize;
            screenPos /= orthoSize;

            screenPos.x -= Screen.Width * 0.5f;
            screenPos.y += Screen.Height * 0.5f;

            Vector3 mousePos = new Vector3(screenPos.x + Mouse.Position.x, screenPos.y - Mouse.Position.y, 0) * orthoSize;
            Vector3 finalGridPos = PreviewAndGetBlockPlacement(mousePos);
            IntVector3 currentGridPos = GlobalSettings.GetGridCoordinatesForBlockPos(finalGridPos);
            List<Vector3> blockPos = new();

            if (IsDrawing && _mousePos != mousePos)
            {
                bool smallerX = currentGridPos.X < _oldGridPos.X;
                bool smallerY = currentGridPos.Y < _oldGridPos.Y;

                for (int x = _oldGridPos.X; smallerX && x >= currentGridPos.X || !smallerX && x <= currentGridPos.X; x = smallerX ? x - 1 : x + 1)
                {
                    for (int y = _oldGridPos.Y; smallerY && y >= currentGridPos.Y || !smallerY && y <= currentGridPos.Y; y = smallerY ? y - 1 : y + 1)
                    {
                        blockPos.Add(GlobalSettings.GetBlockPosForGridCoordinates(x, y));
                    }
                }

                _mousePos = mousePos;
            }
            else
            {
                blockPos.Add(finalGridPos);
            }

            _oldGridPos = currentGridPos;

            CreateBlocksOnMouseEvent(blockPos.ToArray());
        }

        public void EnableLevelEditor(bool enable)
        {
            Host.AssertServer();

            (Controller as MovementController).IsFreeze = enable;
            EnableDrawing = !enable;
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

        private void CreateBlocksOnMouseEvent(params Vector3[] pos)
        {
            if (BuildPanel.Instance == null)
            {
                return;
            }

            BuildPanel buildPanel = BuildPanel.Instance;

            if (buildPanel.IsMouseDown)
            {
                IsDrawing = true;

                if (buildPanel.IsLeftMouseButtonDown)
                {
                    Stack.Push(new(pos, buildPanel.SelectedAsset.Name));
                }
                else if (buildPanel.IsRightMouseButtonDown)
                {
                    Stack.Push(new(pos, "__delete__"));
                }
            }
            else if (IsDrawing && !buildPanel.IsMouseDown)
            {
                IsDrawing = false;
            }
        }

        public void SimulateLevelNetworking(Client client)
        {
            if (!Host.IsClient || Local.Client != client)
            {
                return;
            }

            // delete blocks with same position
            Dictionary<Vector3, string> cleanedDict = new();

            while (Stack.TryPop(out StackData stackData))
            {
                foreach (Vector3 vector3 in stackData.Positions)
                {
                    if (cleanedDict.ContainsKey(vector3))
                    {
                        cleanedDict.Remove(vector3);
                    }

                    cleanedDict.Add(vector3, stackData.BlockName);
                }
            }

            // merge same blocks together to reduce network messages
            Dictionary<string, List<Vector3>> compressedDict = new();

            foreach (KeyValuePair<Vector3, string> keyValuePair in cleanedDict)
            {
                if (!compressedDict.TryGetValue(keyValuePair.Value, out List<Vector3> list))
                {
                    list = new();

                    compressedDict.Add(keyValuePair.Value, list);
                }

                list.Add(keyValuePair.Key);
            }

            // TODO break message in parts if it's too long
            // transmit to server
            using (Prediction.Off())
            {
                foreach (KeyValuePair<string, List<Vector3>> keyValuePair in compressedDict)
                {
                    if (keyValuePair.Key == "__delete__")
                    {
                        Levels.Editor.ServerDeleteBlocks(Compression.Compress(keyValuePair.Value).StringArray());
                    }
                    else
                    {
                        Levels.Editor.ServerCreateBlocks(Compression.Compress(keyValuePair.Value).StringArray(), keyValuePair.Key);
                    }
                }
            }
        }
    }
}

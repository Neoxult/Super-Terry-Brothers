using System.Collections.Generic;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.LevelElements;
using TerryBros.Settings;
using TerryBros.Utils;

namespace TerryBros.Levels
{
    public static partial class Editor
    {
        public static ModelEntity CreateBlock(Vector3 position, string blockName)
        {
            Level level = STBGame.CurrentLevel;
            IntVector3 intVector3 = GlobalSettings.GetGridCoordinatesForBlockPos(position);

            level.GridBlocks.TryGetValue(intVector3.X, out Dictionary<int, BlockEntity> dict);

            if (dict == null)
            {
                dict = new();

                level.GridBlocks.Add(intVector3.X, dict);
            }

            dict.TryGetValue(intVector3.Y, out BlockEntity blockEntity);

            if (blockEntity == null)
            {
                blockEntity = BlockEntity.FromName(blockName);

                if (blockEntity != null)
                {
                    blockEntity.Position = position;

                    level.RegisterBlock(blockEntity);
                }
            }

            return blockEntity;
        }

        public static void DeleteBlock(Vector3 position)
        {
            Level level = STBGame.CurrentLevel;
            IntVector3 intVector3 = GlobalSettings.GetGridCoordinatesForBlockPos(position);

            level.GridBlocks.TryGetValue(intVector3.X, out Dictionary<int, BlockEntity> dict);

            if (dict == null)
            {
                dict = new();

                level.GridBlocks.Add(intVector3.X, dict);
            }

            dict.TryGetValue(intVector3.Y, out BlockEntity blockEntity);

            if (blockEntity != null)
            {
                dict.Remove(intVector3.Y);
                blockEntity.Delete();

                // TODO unregister block and recalculate bounds
            }
        }
    }
}

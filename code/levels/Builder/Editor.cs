using System;
using System.Collections.Generic;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.LevelElements;
using TerryBros.Settings;
using TerryBros.Utils;
using TerryBros.UI.LevelBuilder;

namespace TerryBros.Levels.Builder
{
    public static partial class Editor
    {
        // Just some testing, to create blocks dynamically
        [ServerCmd(Name = "stb_block", Help = "Spawns a block in front of the player")]
        public static void ServerCreateBlock(Vector3 position, string blockName)
        {
            CreateBlock(position, blockName);
            ClientCreateBlock(position, blockName);
        }

        // Just some testing, to create blocks dynamically
        [ServerCmd(Name = "stb_block_delete", Help = "Removes a block in front of the player")]
        public static void ServerDeleteBlock(Vector3 position)
        {
            DeleteBlock(position);
            ClientDeleteBlock(position);
        }

        [ClientRpc]
        public static void ClientCreateBlock(Vector3 position, string blockName)
        {
            CreateBlock(position, blockName);
        }

        [ClientRpc]
        public static void ClientDeleteBlock(Vector3 position)
        {
            DeleteBlock(position);
        }

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

                    dict[intVector3.Y] = blockEntity;
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
            }
        }

        [ClientCmd("stb_editor")]
        public static void ClientToggleLevelEditor()
        {
            if (Local.Pawn is not Player player)
            {
                return;
            }

            bool toggle = !player.IsInLevelBuilder;

            player.EnableLevelEditor(toggle);
            ServerToggleLevelEditor(toggle);
            BuildPanel.Instance.Toggle(toggle);
        }

        [ServerCmd]
        public static void ServerToggleLevelEditor(bool toggle)
        {
            if (ConsoleSystem.Caller?.Pawn is not Player player)
            {
                return;
            }

            player.EnableLevelEditor(toggle);
        }

        [ServerCmd]
        public static void ServerToggleMenu(bool toggle)
        {
            if (ConsoleSystem.Caller?.Pawn is not Player player)
            {
                return;
            }

            player.IsInMenu = toggle;
        }
    }
}

using System;
using System.Collections.Generic;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.Player;
using TerryBros.Player.Controller;
using TerryBros.LevelElements;
using TerryBros.Settings;
using TerryBros.Utils;

namespace TerryBros.Levels.Builder
{
    public static partial class Editor
    {
        // Just some testing, to create blocks dynamically
        [ServerCmd(Name = "stb_block", Help = "Spawns a block in front of the player")]
        public static void ServerCreateBlock(Vector3 position, string blockTypeName)
        {
            TerryBrosPlayer player = ConsoleSystem.Caller.Pawn as TerryBrosPlayer;
            MovementController movementController = player.Controller as MovementController;

            CreateBlock(position, blockTypeName);
            ClientCreateBlock(position, blockTypeName);
        }

        // Just some testing, to create blocks dynamically
        [ServerCmd(Name = "stb_block_delete", Help = "Removes a block in front of the player")]
        public static void ServerDeleteBlock(Vector3 position)
        {
            TerryBrosPlayer player = ConsoleSystem.Caller.Pawn as TerryBrosPlayer;
            MovementController movementController = player.Controller as MovementController;

            DeleteBlock(position);
            ClientDeleteBlock(position);
        }

        [ClientRpc]
        public static void ClientCreateBlock(Vector3 position, string blockTypeName)
        {
            CreateBlock(position, blockTypeName);
        }

        [ClientRpc]
        public static void ClientDeleteBlock(Vector3 position)
        {
            DeleteBlock(position);
        }

        public static ModelEntity CreateBlock(Vector3 position, string blockTypeName)
        {
            Level level = STBGame.CurrentLevel;
            IntVector3 intVector3 = GlobalSettings.GetGridCoordinatesForBlockPos(position);

            level.GridBlocks.TryGetValue(intVector3.x, out Dictionary<int, BlockEntity> dict);

            if (dict == null)
            {
                dict = new();

                level.GridBlocks.Add(intVector3.x, dict);
            }

            dict.TryGetValue(intVector3.y, out BlockEntity blockEntity);

            if (blockEntity == null)
            {
                Type type = BlockEntity.GetByName(blockTypeName);

                if (type != null)
                {
                    blockEntity = Library.Create<BlockEntity>(type);
                    blockEntity.Position = position;

                    dict[intVector3.y] = blockEntity;

                    return blockEntity;
                }
            }

            return blockEntity;
        }

        public static void DeleteBlock(Vector3 position)
        {
            Level level = STBGame.CurrentLevel;
            IntVector3 intVector3 = GlobalSettings.GetGridCoordinatesForBlockPos(position);

            level.GridBlocks.TryGetValue(intVector3.x, out Dictionary<int, BlockEntity> dict);

            if (dict == null)
            {
                dict = new();

                level.GridBlocks.Add(intVector3.x, dict);
            }

            dict.TryGetValue(intVector3.y, out BlockEntity blockEntity);

            if (blockEntity != null)
            {
                dict.Remove(intVector3.y);
                blockEntity.Delete();
            }
        }

        [ClientCmd("stb_editor")]
        public static void ClientToggleLevelEditor()
        {
            if (Local.Pawn is not TerryBrosPlayer player)
            {
                return;
            }

            player.IsInLevelBuilder = !player.IsInLevelBuilder;

            ServerToggleLevelEditor(player.IsInLevelBuilder);

            UI.LevelBuilder.Builder.Instance.Toggle(player.IsInLevelBuilder);
        }

        [ServerCmd]
        public static void ServerToggleLevelEditor(bool toggle)
        {
            if (ConsoleSystem.Caller?.Pawn is not TerryBrosPlayer player)
            {
                return;
            }

            player.IsInLevelBuilder = toggle;
        }

        [ServerCmd]
        public static void ServerToggleMenu(bool toggle)
        {
            if (ConsoleSystem.Caller?.Pawn is not TerryBrosPlayer player)
            {
                return;
            }

            player.IsInMenu = toggle;
        }
    }
}

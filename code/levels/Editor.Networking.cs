using System.Collections.Generic;

using Sandbox;

using TerryBros.UI.LevelBuilder;
using TerryBros.Utils;

namespace TerryBros.Levels
{
    public static partial class Editor
    {
        private static List<Vector3> Decompress(string positions) => Level.Decompress(Compression.Decompress<string>(positions.ByteArray()));

        [ServerCmd]
        public static void ServerCreateBlocks(string positions, string blockName)
        {
            foreach (Vector3 position in Decompress(positions))
            {
                CreateBlock(position, blockName);
            }

            ClientCreateBlocks(positions, blockName);
        }

        [ServerCmd]
        public static void ServerDeleteBlocks(string positions)
        {
            foreach (Vector3 position in Decompress(positions))
            {
                DeleteBlock(position);
            }

            ClientDeleteBlocks(positions);
        }

        [ClientRpc]
        public static void ClientCreateBlocks(string positions, string blockName)
        {
            foreach (Vector3 position in Decompress(positions))
            {
                CreateBlock(position, blockName);
            }
        }

        [ClientRpc]
        public static void ClientDeleteBlocks(string positions)
        {
            foreach (Vector3 position in Decompress(positions))
            {
                DeleteBlock(position);
            }
        }

        [ServerCmd]
        public static void ServerToggleLevelEditor(bool toggle, bool respawn)
        {
            if (ConsoleSystem.Caller == null)
            {
                return;
            }

            ConsoleSystem.Caller.SetValue("leveleditor", toggle);

            if (ConsoleSystem.Caller.Pawn is Player player)
            {
                player.EnableLevelEditor(toggle);

                if (respawn)
                {
                    player.Respawn();
                }
            }

            ClientToggleLevelEditor(To.Single(ConsoleSystem.Caller), toggle);
        }

        [ClientRpc]
        public static void ClientToggleLevelEditor(bool toggle)
        {
            BuildPanel.Instance?.Toggle(toggle);
            UI.Menu.Instance?.MenuContent?.OnClickHome();

            if (Local.Pawn is Player player)
            {
                player.SetCameraDimension();
            }
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

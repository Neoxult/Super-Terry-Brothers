using Sandbox;

using TerryBros.UI.LevelBuilder;

namespace TerryBros.Levels
{
    public static partial class Editor
    {
        [ServerCmd]
        public static void ServerCreateBlock(Vector3 position, string blockName)
        {
            CreateBlock(position, blockName);
            ClientCreateBlock(position, blockName);
        }

        [ServerCmd]
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
            UI.Menu.Menu.Instance?.MenuContent?.OnClickHome();

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

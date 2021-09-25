using Sandbox;

using TerryBros.Player;

namespace TerryBros.Gamemode
{
    public partial class STBGame
    {
        public override void Simulate(Client cl)
        {
            if (cl.Pawn is TerryBrosPlayer player)
            {
                player.Position = player.Position.WithY(0);
            }

            base.Simulate(cl);

            ClientOutOfBounds(cl);
        }

        // server-side only
        public static void LevelFinished(TerryBrosPlayer player)
        {
            if (Host.IsClient)
            {
                return;
            }

            CurrentLevel?.Restart();
            ClientRestartLevel();

            player.Respawn();
        }

        [ClientRpc]
        public static void ClientRestartLevel()
        {
            CurrentLevel?.Restart();
        }

        private void ClientOutOfBounds(Client cl)
        {
            if (Host.IsClient || cl.Pawn is not TerryBrosPlayer player)
            {
                return;
            }

            BBox bBox = STBGame.CurrentLevel.LevelBounds;

            if (player.Position.x < bBox.Mins.x
                || player.Position.x > bBox.Maxs.x
                || player.Position.y < bBox.Mins.y
                || player.Position.y > bBox.Maxs.y
                || player.Position.z < bBox.Mins.z
                || player.Position.z > bBox.Maxs.z
                )
            {
                player.Respawn();
            }
        }
    }
}

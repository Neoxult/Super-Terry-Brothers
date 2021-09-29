using Sandbox;

using TerryBros.Events;
using TerryBros.Player;
using TerryBros.Settings;

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

        [Event(TBEvent.Level.GoalReached)]
        private static void GoalReached(TerryBrosPlayer player)
        {
            //TODO: Maybe add Logic for more players that want to run to the goal
            // And a Timer, that runs down to put pressure on them
            Event.Run(TBEvent.Level.Finished);
        }

        // server-side only
        [Event(TBEvent.Level.Finished)]
        private static void LevelFinished()
        {
            if (Host.IsClient)
            {
                return;
            }

            //TODO: Add a real Level Change instead of a restart
            Event.Run(TBEvent.Level.Restart);

            ClientRestartLevel();

            foreach (Client client in Client.All)
            {

                if (client.Pawn is TerryBrosPlayer player)
                {
                    player.Respawn();
                }
            }
        }

        [ClientRpc]
        public static void ClientRestartLevel()
        {
            Event.Run(TBEvent.Level.Restart);
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
                || player.Position.z > bBox.Maxs.z + 5 * GlobalSettings.BlockSize
                )
            {
                player.Respawn();
            }
        }
    }
}

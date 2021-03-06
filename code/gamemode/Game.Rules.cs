using Sandbox;

using TerryBros.Events;
using TerryBros.Settings;

#pragma warning disable IDE0051, IDE0060

namespace TerryBros.Gamemode
{
    public partial class STBGame
    {
        private static void SimulateRules(Client cl)
        {
            if (cl.Pawn is Player player)
            {
                player.LocalPosition = player.LocalPosition.WithZ(0);
            }

            ClientOutOfBounds(cl);
        }

        [Event(TBEvent.Level.GOAL_REACHED)]
        private static void GoalReached(Player player)
        {
            // TODO: Maybe add Logic for more players that want to run to the goal
            // And a Timer, that runs down to put pressure on them
            Event.Run(TBEvent.Level.FINISHED, player);
        }

        // server-side only
        [Event(TBEvent.Level.FINISHED)]
        private static void LevelFinished(Player player)
        {
            if (Host.IsClient)
            {
                return;
            }

            // TODO: Add a real Level Change instead of a restart
            Event.Run(TBEvent.Level.RESTART);

            ClientRestartLevel();

            if (player.IsValid)
            {
                player.Respawn();
            }
        }

        [ClientRpc]
        public static void ClientRestartLevel()
        {
            Event.Run(TBEvent.Level.RESTART);
        }

        private static void ClientOutOfBounds(Client cl)
        {
            if (Host.IsClient || cl.Pawn is not Player player)
            {
                return;
            }

            BBox bBox = CurrentLevel.LevelBoundsLocal;
            Vector3 pos = player.LocalPosition;

            if (pos.x < bBox.Mins.x || pos.x > bBox.Maxs.x
                || pos.y < bBox.Mins.y || pos.y > bBox.Maxs.y + 5 * GlobalSettings.BlockSize
                || pos.z < bBox.Mins.z || pos.z > bBox.Maxs.z)
            {
                player.Respawn();
            }
        }
    }
}

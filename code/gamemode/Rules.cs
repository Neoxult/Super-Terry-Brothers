using Sandbox;

using TerryBros.Player;
using TerryBros.Settings;
using TerryBros.Levels;

namespace TerryBros.Gamemode
{
    public partial class STBGame
    {
        public override void Simulate(Client cl)
        {
            base.Simulate(cl);
            clientOutOfBounds(cl);
        }

        //TODO: Proper Winning with Endscreen
        public static void LevelFinished(TerryBrosPlayer player)
        {
            if (Host.IsClient)
            {
                return;
            }
            Level.currentLevel?.Restart();
            player.Respawn();
        }
        private void clientOutOfBounds(Client cl)
        {
            if (Host.IsClient)
            {
                return;
            }

            if(cl.Pawn is not TerryBrosPlayer player)
            {
                return;
            }

            if (player.Position.x <= globalSettings.worldBounds.Mins.x
                || player.Position.x >= globalSettings.worldBounds.Maxs.x
                || player.Position.y <= globalSettings.worldBounds.Mins.y
                || player.Position.y >= globalSettings.worldBounds.Maxs.y
                || player.Position.z <= globalSettings.worldBounds.Mins.z
                || player.Position.z >= globalSettings.worldBounds.Maxs.z
                )
            {
                player.Respawn();
            }
        }
    }
}

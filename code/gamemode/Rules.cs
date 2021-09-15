using Sandbox;

using TerryBros.Player;
using TerryBros.Settings;

namespace TerryBros.Gamemode
{
    public partial class Game
    {
        public override void Simulate(Client cl)
        {
            base.Simulate(cl);
            clientOutOfBounds(cl);
        }
        private void clientOutOfBounds(Client cl)
        {
            if (!Host.IsServer)
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

using Sandbox;

using TerryBros.Player;
using TerryBros.UI;
using TerryBros.Levels;

namespace TerryBros.Gamemode
{
    [Library("STB", Title = "Super Terry Brothers")]
    public partial class Game : Sandbox.Game
    {
        private Level currentLevel = null;
        public Game()
        {
            if (IsServer)
            {
                new Hud();
            }
        }
        [Event.Entity.PostSpawn]
        private void PostLevelSpawn()
        {
            if (currentLevel == null || !currentLevel.IsValid)
            {
                currentLevel = new DefaultLevel();
            }
        }
        public override void ClientJoined(Client client)
        {
            base.ClientJoined(client);

            var player = new TerryBrosPlayer(client);
            client.Pawn = player;

            player.Respawn();
        }
    }
}

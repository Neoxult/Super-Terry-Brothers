using Sandbox;

using TerryBros.Player;
using TerryBros.UI;
using TerryBros.Levels;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
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
                currentLevel = new DefaultLevel(new Vector3(0, 0, 0), Vector3.Backward, Vector3.Up);
            }
        }
        public override void ClientJoined(Client client)
        {
            base.ClientJoined(client);

            var player = new TerryBrosPlayer(client);
            client.Pawn = player;

            //TODO: Change to proper Level Creation and Spawn
            //Create a Level underneath the spawns
            player.Respawn();
        }
    }
}

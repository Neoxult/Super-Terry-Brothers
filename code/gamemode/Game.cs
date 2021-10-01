using Sandbox;

using TerryBros.Player;

namespace TerryBros.Gamemode
{
    [Library("STB", Title = "Super Terry Brothers")]
    public partial class STBGame : Sandbox.Game
    {
        public STBGame()
        {
        }

        public override void ClientJoined(Client client)
        {
            base.ClientJoined(client);

            TerryBrosPlayer player = new(client);
            client.Pawn = player;

            player.Respawn();
        }
    }
}

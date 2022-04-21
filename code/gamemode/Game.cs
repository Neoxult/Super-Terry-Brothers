using Sandbox;

#pragma warning disable IDE0051

namespace TerryBros.Gamemode
{
    public partial class STBGame : Game
    {
        public STBGame() : base()
        {
            if (IsClient)
            {
                _ = new UI.Hud();

                UI.StartScreen.StartScreen.Instance.Display = true;
            }
        }

        public override void Simulate(Client cl)
        {
            base.Simulate(cl);

            SimulateRules(cl);
            SimulateDebug(cl);
        }

        public override void ClientJoined(Client client)
        {
            base.ClientJoined(client);

            // TODO

            Player player = new();
            client.Pawn = player;

            player.Clothing.LoadFromClient(client);
            player.Spawn();
        }
    }
}

using System.Collections.Generic;

using Sandbox;

#pragma warning disable IDE0051

namespace TerryBros.Gamemode
{
    public partial class STBGame : Game
    {
        public List<Client> PlayingClients { get; set; }

        public bool IsPlaying
        {
            get => PlayingClients != null;
        }

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

            ClientOnClientJoined(client);

            Player player = new();
            client.Pawn = player;

            player.Clothing.LoadFromClient(client);
            player.Spawn();
        }

        [ClientRpc]
        public static void ClientOnClientJoined(Client client)
        {
            Event.Run("OnClientConnected", client);
        }

        public override void ClientDisconnect(Client client, NetworkDisconnectionReason reason)
        {
            ClientOnClientDisconnected(client, reason);

            base.ClientDisconnect(client, reason);
        }

        [ClientRpc]
        public static void ClientOnClientDisconnected(Client client, NetworkDisconnectionReason reason)
        {
            Event.Run("OnClientDisconnected", client, reason);
        }

        public static void Start()
        {
            (Current as STBGame).PlayingClients = new(Client.All);
        }

        public static void Finish()
        {
            (Current as STBGame).PlayingClients = null;
        }
    }
}

using System.Collections.Generic;

using Sandbox;

#pragma warning disable IDE0051

namespace TerryBros.Gamemode
{
    public partial class STBGame : Game
    {
        public static STBGame Instance
        {
            get => Current as STBGame;
        }

        public List<Client> PlayingClients { get; set; }

        public bool IsPlaying
        {
            get => PlayingClients != null;
        }

        public enum GameState
        {
            LevelEditor,
            Game,
            StartScreen
        }

        [Net]
        public GameState State { get; set; } = GameState.StartScreen;

        public STBGame() : base()
        {
            if (IsClient)
            {
                _ = new UI.Hud();
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
            if (State != GameState.StartScreen)
            {
                client.SetValue("kicked", true);
                client.Kick();

                return;
            }

            base.ClientJoined(client);

            ClientOnClientJoined(client);
        }

        [ClientRpc]
        public static void ClientOnClientJoined(Client client)
        {
            Event.Run(Events.TBEvent.Game.CLIENT_CONNECTED, client);
        }

        public override void ClientDisconnect(Client client, NetworkDisconnectionReason reason)
        {
            if (client.GetValue("kicked", false))
            {
                return;
            }

            ClientOnClientDisconnected(client, reason);

            base.ClientDisconnect(client, reason);
        }

        [ClientRpc]
        public static void ClientOnClientDisconnected(Client client, NetworkDisconnectionReason reason)
        {
            Event.Run(Events.TBEvent.Game.CLIENT_DISCONNECTED, client, reason);
        }
    }
}

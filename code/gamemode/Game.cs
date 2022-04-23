using System.Collections.Generic;

using Sandbox;

using TerryBros.Events;
using TerryBros.Levels;

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

        [Event(TBEvent.Level.LOADED)]
        public static void Start(Level _)
        {
            if (!Host.IsServer)
            {
                return;
            }

            STBGame game = Current as STBGame;

            game.PlayingClients = new(Client.All);

            foreach (Client client in game.PlayingClients)
            {
                Player player = new();
                client.Pawn = player;

                player.Clothing.LoadFromClient(client);
                player.Spawn();
            }
        }

        [Event(TBEvent.Level.CLEARED)]
        public static void Clear(Level _)
        {
            if (!Host.IsServer)
            {
                return;
            }

            STBGame game = Current as STBGame;

            foreach (Client client in game.PlayingClients)
            {
                if (client == null || !client.IsValid())
                {
                    continue;
                }

                if (client.Pawn is Player player)
                {
                    player.Delete();
                }

                client.Pawn = null;
            }

            game.PlayingClients = null;
        }
    }
}

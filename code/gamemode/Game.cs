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
            }
        }

        [Event.Hotload]
        public static void Reset()
        {
            CurrentLevel?.Clear();
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

        [ServerCmd]
        public static void StartGame(string levelPath)
        {
            foreach (Client client in Client.All)
            {
                client.SetValue("playing", true);
            }

            Levels.Loader.Local.Load(levelPath);
        }

        public static void StartEditor()
        {
            Level level = new();
            level.Build();

            Start(level);
        }

        [Event(TBEvent.Level.LOADED)]
        protected static void Start(Level level)
        {
            CurrentLevel = level;

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
        protected static void Clear(Level level)
        {
            if (CurrentLevel == level)
            {
                CurrentLevel = null;
            }

            if (!Host.IsServer)
            {
                UI.Menu.Menu.Instance?.Delete(true);

                return;
            }

            if (Current is not STBGame game)
            {
                return;
            }

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

                client.SetValue("playing", false);

                client.Pawn = null;
            }

            game.PlayingClients = null;
        }
    }
}

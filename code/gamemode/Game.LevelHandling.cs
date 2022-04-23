using Sandbox;

using TerryBros.Events;
using TerryBros.Levels;

#pragma warning disable IDE0051

namespace TerryBros.Gamemode
{
    public partial class STBGame
    {
        public static Level LastLevel { get; private set; }

        public static Level CurrentLevel
        {
            get => _currentLevel;
            protected set
            {
                LastLevel = _currentLevel;
                _currentLevel = value;
            }
        }

        private static Level _currentLevel;

        public override void MoveToSpawnpoint(Entity pawn)
        {
            LevelElements.SpawnPoint spawnPoint = CurrentLevel?.GetLastCheckPoint();

            if (spawnPoint == null)
            {
                Log.Warning($"Couldn't find spawnpoint for {pawn}!");

                return;
            }

            spawnPoint.MoveToSpawn(pawn);
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
            Levels.Editor.ClientToggleLevelEditor(true);

            ServerStartEditor();
            InitLevel();
        }

        [ServerCmd]
        private static void ServerStartEditor()
        {
            InitLevel();
        }

        protected static void InitLevel()
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

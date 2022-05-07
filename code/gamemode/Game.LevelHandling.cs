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
            if (pawn is not Player player)
            {
                return;
            }

            LevelElements.SpawnPoint spawnPoint = CurrentLevel?.GetLastCheckPoint(player);

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
            if (!ConsoleSystem.Caller?.HasPermission("startgame") ?? true)
            {
                return;
            }

            Instance.State = GameState.Game;

            Levels.Loader.Local.Load(levelPath);
        }

        [ServerCmd]
        public static void StartLevelEditor(string levelPath = null)
        {
            if (!ConsoleSystem.Caller?.HasPermission("startleveleditor") ?? false)
            {
                return;
            }

            Instance.State = GameState.LevelEditor;

            if (string.IsNullOrEmpty(levelPath))
            {
                Start(Levels.Loader.Local.Empty());
                ClientStartLevelEditor();
            }
            else
            {
                Levels.Loader.Local.Load(levelPath);
            }
        }

        [ClientRpc]
        public static void ClientStartLevelEditor()
        {
            Start(Levels.Loader.Local.Empty());
        }

        [ServerCmd]
        public static void ClearLevel()
        {
            if (!ConsoleSystem.Caller?.HasPermission("clear") ?? false)
            {
                return;
            }

            CurrentLevel.Clear();
            ClientClearLevel();
        }

        [ServerCmd]
        public static void QuitGame()
        {
            if (!ConsoleSystem.Caller?.HasPermission("quitgamestate") ?? true)
            {
                return;
            }

            Instance.State = GameState.StartScreen;

            Levels.Editor.ClientToggleLevelEditor(false);

            ClearLevel();
        }

        [ClientRpc]
        public static void ClientClearLevel()
        {
            CurrentLevel.Clear();
        }

        [Event(TBEvent.Level.LOADED)]
        protected static void Start(Level level)
        {
            CurrentLevel = level;

            if (!Host.IsServer)
            {
                return;
            }

            STBGame game = Instance;

            game.PlayingClients = new(Client.All);

            foreach (Client client in game.PlayingClients)
            {
                client.SetValue("leveleditor", game.State == GameState.LevelEditor);

                Player player = new();
                client.Pawn = player;

                player.Clothing.LoadFromClient(client);
                player.Spawn();
            }

            if (game.State == GameState.LevelEditor)
            {
                Levels.Editor.ClientToggleLevelEditor(true);
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
                UI.Menu.Instance?.Delete(true);

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

                client.Pawn = null;

                client.SetValue("leveleditor", false);
            }

            game.PlayingClients = null;
        }
    }
}

using Sandbox;

using TerryBros.LevelElements;
using TerryBros.Levels;
using TerryBros.Player;
using TerryBros.UI;

namespace TerryBros.Gamemode
{
    [Library("STB", Title = "Super Terry Brothers")]
    public partial class STBGame : Sandbox.Game
    {
        public static Level LastLevel { get; private set; }

        public static Level CurrentLevel
        {
            get => _currentLevel;
            set
            {
                LastLevel = _currentLevel;
                _currentLevel = value;
            }
        }
        private static Level _currentLevel;

        public STBGame()
        {
            if (IsServer)
            {
                new Hud();
            }
        }

        [Event.Entity.PostSpawn]
        private void PostLevelSpawn()
        {
            if (CurrentLevel != null)
            {
                return;
            }

            CurrentLevel = new DefaultLevel();
            CurrentLevel.Build();
        }

        public override void ClientJoined(Client client)
        {
            base.ClientJoined(client);

            TerryBrosPlayer player = new(client);

            // TODO: Find error, that sometimes the player doesnt fully spawn or gets rendered
            player.Respawn();

            client.Pawn = player;
        }

        public override void MoveToSpawnpoint(Entity pawn)
        {
            STBSpawn spawnPoint = CurrentLevel?.GetLastCheckPoint();

            if (spawnPoint == null)
            {
                Log.Warning($"Couldn't find spawnpoint for {pawn}!");

                return;
            }

            spawnPoint.MoveToSpawn(pawn);
        }
    }
}

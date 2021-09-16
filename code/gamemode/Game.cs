using Sandbox;

using TerryBros.Player;
using TerryBros.UI;
using TerryBros.Levels;
using TerryBros.LevelElements;

namespace TerryBros.Gamemode
{
    [Library("STB", Title = "Super Terry Brothers")]
    public partial class STBGame : Sandbox.Game
    {
        private Level currentLevel = null;
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
            if (currentLevel == null || !currentLevel.IsValid)
            {
                currentLevel = new DefaultLevel();
            }
        }
        public override void ClientJoined(Client client)
        {
            base.ClientJoined(client);

            var player = new TerryBrosPlayer(client);
            client.Pawn = player;

            //TODO: Find error, that sometimes the player doesnt fully spawn or gets rendered
            player.Respawn();
        }

		public override void MoveToSpawnpoint(Entity pawn)
        {
            STBSpawn spawnPoint = Level.currentLevel?.GetLastCheckPoint();

            if (spawnPoint == null)
            {
                Log.Warning($"Couldn't find spawnpoint for {pawn}!");
                return;
            }

            spawnPoint.MoveToSpawn(pawn);
        }
    }
}

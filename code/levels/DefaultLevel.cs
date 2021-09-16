using System.Linq;

using Sandbox;

using TerryBros.LevelElements;
using TerryBros.Player;
using TerryBros.Player.Controller;
using TerryBros.Settings;

namespace TerryBros.Levels
{
    public partial class DefaultLevel : Level
    {
        private intVector3 minBound = new intVector3(-30, -3, -1);
        private intVector3 maxBound = new intVector3(40, 10, 1);
        public DefaultLevel()
        {
            Transform newSpawn = new Transform(new Vector3(0, 0, 40));
            globalSettings.worldBoundsBlocks = new intBBox(minBound, maxBound);

            bool isNewSpawnSet = false;

            foreach (SpawnPoint spawn in All.OfType<SpawnPoint>())
            {
                if (isNewSpawnSet)
                {
                    spawn.Delete();
                }
                else
                {
                    spawn.Transform = newSpawn;
                    isNewSpawnSet = true;
                }
            }
            if (!isNewSpawnSet)
            {
                var spawn = Create<SpawnPoint>();
                spawn.Transform = newSpawn;
            }

            Create<DefaultSky>();

            var light = Create<EnvironmentLightEntity>();
            light.Rotation = Rotation.LookAt(new Vector3(-1, 1, -4), globalSettings.upwardDir);
            light.Brightness = 2f;

            light = Create<EnvironmentLightEntity>();
            light.Rotation = Rotation.LookAt(new Vector3(1, 0.5f, -1), globalSettings.upwardDir);
            light.Brightness = 2f;

            CreateWallFromTo<Brick>(globalSettings.worldBoundsBlocks.Mins.x, -globalSettings.visibleGroundBlocks + 1, -10, 0);
            CreateWallFromTo<Brick>(-6, -globalSettings.visibleGroundBlocks + 1, 20, 0);
            CreateWallFromTo<Brick>(24, -globalSettings.visibleGroundBlocks + 1, globalSettings.worldBoundsBlocks.Maxs.x, 0);
            CreateStair<Brick>(5, 1, 6, true);
            CreateStair<Brick>(11, 1, 5, false);
            CreateWall<Brick>(globalSettings.worldBoundsBlocks.Mins.x, 1, 2, 3);
            CreateWall<Brick>(globalSettings.worldBoundsBlocks.Mins.x, 9, 2, 4);
            CreateWall<Brick>(globalSettings.worldBoundsBlocks.Maxs.x - 1, 1, 2, 3);
            CreateWall<Brick>(globalSettings.worldBoundsBlocks.Maxs.x - 1, 9, 2, 4);
        }

        // Just some testing, to create blocks dynamically
        [ServerCmd(Name = "stb_block", Help = "Spawns a block in front of the player's")]
        public static void ServerCreateBlock()
        {
            TerryBrosPlayer player = ConsoleSystem.Caller.Pawn as TerryBrosPlayer;
            MovementController movementController = player.Controller as MovementController;
            Vector3 forwardVector = movementController.Forward ? globalSettings.forwardDir : -globalSettings.forwardDir;
            Vector3 position = player.Position + forwardVector * 100f + new Vector3(0, 0, 25f);

            CreateBlock(position);
            ClientCreateBlock(position);
        }

        [ClientRpc]
        public static void ClientCreateBlock(Vector3 position)
        {
            CreateBlock(position);
        }

        public static ModelEntity CreateBlock(Vector3 position)
        {
            return new Brick(position);
        }
    }
}

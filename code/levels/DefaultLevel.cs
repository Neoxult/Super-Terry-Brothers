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

            var sky = Create<DefaultSky>();

            var light = Entity.Create<EnvironmentLightEntity>();
            light.Rotation = Rotation.LookAt(new Vector3(-1, 1, -4), globalSettings.upwardDir);
            light.Brightness = 2f;

            light = Entity.Create<EnvironmentLightEntity>();
            light.Rotation = Rotation.LookAt(new Vector3(1, 0.5f, -1), globalSettings.upwardDir);
            light.Brightness = 2f;

            CreateBox(0, 0);
            /*
            CreateWallFromTo(globalSettings.worldBoundsBlocks.Mins.x, -globalSettings.visibleGroundBlocks + 1, -10, 0);
            CreateWallFromTo(-6, -globalSettings.visibleGroundBlocks + 1, 20, 0);
            CreateWallFromTo(24, -globalSettings.visibleGroundBlocks + 1, globalSettings.worldBoundsBlocks.Maxs.x, 0);
            CreateStair(5, 1, 6, true);
            CreateStair(11, 1, 5, false);
            CreateWall(globalSettings.worldBoundsBlocks.Mins.x, 1, 2, 3);
            CreateWall(globalSettings.worldBoundsBlocks.Mins.x, 9, 2, 4);
            CreateWall(globalSettings.worldBoundsBlocks.Maxs.x - 1, 1, 2, 3);
            CreateWall(globalSettings.worldBoundsBlocks.Maxs.x - 1, 9, 2, 4);
            */
        }

        private Brick CreateBox(int GridX, int GridY)
        {
            Log.Info($"IsClient: {Host.IsClient}, IsServer: {Host.IsServer}");
            return new Brick(globalSettings.GetBlockPosForGridCoordinates(GridX, GridY));
        }

        private void CreateStair(int GridX, int GridY, int height, bool upward = true)
        {
            for (int i = 0; i < height; i++)
            {
                int x = GridX + i;
                int maxHeight = upward ? i + 1 : height - i;

                for (int j = 0; j < maxHeight; j++)
                {
                    int y = GridY + j;

                    CreateBox(x, y);
                }
            }
        }
        private void CreateWallFromTo(int StartGridX, int StartGridY, int EndGridX, int EndGridY)
        {
            for (int x = StartGridX; x <= EndGridX; x++)
            {
                for (int y = StartGridY; y <= EndGridY; y++)
                {
                    CreateBox(x, y);
                }
            }
        }
        private void CreateWall(int GridX, int GridY, int width, int height)
        {
            for (int x = GridX; x < GridX + width; x++)
            {
                for (int y = GridY; y < GridY + height; y++)
                {
                    CreateBox(x, y);
                }
            }
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

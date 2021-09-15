using System.Linq;

using Sandbox;

using TerryBros.Objects;
using TerryBros.Player;
using TerryBros.Player.Controller;
using TerryBros.Settings;

namespace TerryBros.Levels
{
    public partial class DefaultLevel : Level
    {
        private Vector3 groundPos = Vector3.Zero;
        private Vector3 forward = Vector3.Forward;
        private Vector3 up = Vector3.Up;

        public DefaultLevel()
        {
            globalSettings.forwardDir = forward;
            globalSettings.upwardDir = up;
            globalSettings.lookDir = Vector3.Cross(up, forward);

            bool isNewSpawnSet = false;
            Transform newSpawn = new Transform(new Vector3(0, 0, 40));

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

            var sky = Create<defaultSky>();

            var light = Entity.Create<EnvironmentLightEntity>();
            light.Rotation = Rotation.LookAt(new Vector3(-1, 1, -4), up);
            light.Brightness = 2f;

            light = Entity.Create<EnvironmentLightEntity>();
            light.Rotation = Rotation.LookAt(new Vector3(1, 0.5f, -1), up);
            light.Brightness = 2f;

            CreateFloor();
            CreateStair(5, 1, 3, true);
            CreateStair(8, 1, 3, false);
        }

        private ModelEntity CreateBox(int GridX, int GridY)
        {
            return CreateBlock(groundPos + GridX * forward * 50f + GridY * up * 50f);
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

        private void CreateFloor()
        {
            for (int i = -100; i < 100; i++)
            {
                for (int j = 0; j > -3; j--)
                {
                    CreateBox(i, j);
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
            VertexBuffer vb = new VertexBuffer();
            vb.Init(true);
            vb.AddCube(Vector3.Zero, Vector3.One * 50.0f, Rotation.Identity);

            Mesh mesh = new Mesh(Material.Load("materials/blocks/stair_block.vmat"));
            mesh.CreateBuffers(vb);
            mesh.SetBounds(Vector3.One * -25, Vector3.One * 25);

            Model model = new ModelBuilder()
                .AddMesh(mesh)
                .AddCollisionBox(Vector3.One * 25.0f)
                .Create();

            ModelEntity entity = WorldEntity.Create<ModelEntity>();
            entity.SetModel(model);
            entity.Position = position;
            entity.Transmit = TransmitType.Always;

            entity.SetupPhysicsFromModel(PhysicsMotionType.Static);
            entity.CollisionGroup = CollisionGroup.Always;
            entity.EnableAllCollisions = true;
            entity.EnableHitboxes = true;
            
            entity.Spawn();

            if (Host.IsClient)
            {
                DebugOverlay.Box(position - 25f, position + 25f, Color.FromBytes(255, 0, 0), 2000f);
            }

            return entity;
        }
    }
}

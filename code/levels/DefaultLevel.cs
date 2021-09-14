using Sandbox;
using System.Linq;
using TerryBros.Player;
using TerryBros.Settings;
using TerryBros.Objects;

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
                } else
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
            ModelEntity box = new ModelEntity
            {
                Rotation = Rotation.LookAt(forward, up)
            };

            box.SetModel("models/citizen_props/crate01.vmdl");
            box.SetupPhysicsFromModel(PhysicsMotionType.Static);
            box.Position = groundPos + GridX * forward * box.CollisionBounds.Size + GridY * up * box.CollisionBounds.Size;

            return box;
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
        private static void CreateBlock()
        {
            TerryBrosPlayer player = ConsoleSystem.Caller.Pawn as TerryBrosPlayer;

            VertexBuffer vb = new VertexBuffer();
            vb.Init(true);
            vb.AddCube(Vector3.Zero, Vector3.One * 50.0f, Rotation.Identity);

            Mesh mesh = new Mesh(Material.Load("materials/default/vertex_color.vmat"));
            mesh.CreateBuffers(vb);
            mesh.SetBounds(Vector3.One * -25, Vector3.One * 25);

            Model model = new ModelBuilder()
                .AddMesh(mesh)
                .AddCollisionBox(Vector3.One * 25.0f)
                .Create();

            Vector3 position = player.Position + player.EyeRot.Forward * 100f;
            position += new Vector3(0, 0, 25f);

            AnimEntity entity = new AnimEntity();
            entity.SetModel(model);
            entity.SetupPhysicsFromModel(PhysicsMotionType.Static);
            entity.RenderColor = Color.Random;
            entity.Position = position;

            entity.RenderDirty();

            DebugOverlay.Box(position - 25f, position + 25f, Color.FromBytes(255, 0, 0), 2000f);
        }
    }
}

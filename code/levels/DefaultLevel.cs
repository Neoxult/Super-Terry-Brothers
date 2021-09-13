using Sandbox;

using TerryBros.Player;

namespace TerryBros.Levels
{
    partial class DefaultLevel : Level
    {
        private Vector3 groundPos;
        private Vector3 forward;
        private Vector3 up;

        public DefaultLevel(Vector3 groundPos, Vector3 forward, Vector3 up)
        {
            this.groundPos = groundPos;
            this.forward = forward;
            this.up = up;

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
            box.Position = groundPos + GridX * forward * box.CollisionBounds.Size + GridY * up * box.CollisionBounds.Size; // groundPos + i * forward * box.CollisionBounds.Size - j * up * box.CollisionBounds.Size;

            return box;
        }
        private void CreateStair(int GridX, int GridY, int height, bool upward = true)
        {
            int direction = upward ? 1 : -1;

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
                for (int j = 0; j > -4; j--)
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

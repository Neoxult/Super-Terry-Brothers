using Sandbox;

using TerryBros.Settings;

namespace TerryBros.LevelElements
{
    public abstract class BlockMaterialEntity : BlockEntity
    {
        public BlockMaterialEntity() : base()
        {
        }
        public override void Spawn()
        {
            VertexBuffer vb = new();
            vb.Init(true);
            vb.AddCube(Vector3.Zero, BlockSizeFloat * GlobalSettings.BlockSize, Rotation.LookAt(GlobalSettings.ForwardDir, -GlobalSettings.UpwardDir));

            Mesh mesh = new(Material.Load(MaterialName));
            mesh.CreateBuffers(vb);
            mesh.SetBounds(BlockSizeFloat * -GlobalSettings.BlockSize / 2, BlockSizeFloat * GlobalSettings.BlockSize / 2);

            Model model = new ModelBuilder()
                .AddMesh(mesh)
                .AddCollisionBox(BlockSizeFloat * GlobalSettings.BlockSize / 2)
                .Create();

            SetModel(model);
            SetupPhysicsFromModel(PhysicsMotionType);

            base.Spawn();
        }
    }
}
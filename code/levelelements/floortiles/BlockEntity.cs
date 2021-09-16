using System;
using Sandbox;

using TerryBros.Settings;

namespace TerryBros.LevelElements
{
    public partial class BlockEntity : ModelEntity
    {
        public BlockEntity() { }

        public BlockEntity(Vector3 position) : this()
        {
            Position = position;
        }

        //TODO: Check for a direct Facepunch fix
        // Currently a Workaround because SetModel cant be called in the constructor
        // See issue: https://github.com/Facepunch/sbox-issues/issues/219
        // Note: Spawn gets called before the constructor
        public override void Spawn()
        {
            Log.Info("Spawn Block.");
            VertexBuffer vb = new VertexBuffer();
            vb.Init(true);

            //TODO: Make an Issue to fix Cubes being orientated the right way.
            //For now its rotated around the forward axis for 180 degrees
            //Otherwise textures arent correct
            vb.AddCube(Vector3.Zero, myBlockSizeFloat * globalSettings.blockSize, Rotation.FromAxis(globalSettings.forwardDir, 180f));

            Mesh mesh = new Mesh(Material.Load(materialName));
            mesh.CreateBuffers(vb);
            mesh.SetBounds(myBlockSizeFloat * -globalSettings.blockSize / 2, myBlockSizeFloat * globalSettings.blockSize / 2);

            Model model = new ModelBuilder()
                .AddMesh(mesh)
                .AddCollisionBox(myBlockSizeFloat * globalSettings.blockSize / 2)
                .Create();

            SetModel(model);
            SetupPhysicsFromModel(physicsMotionType);

            base.Spawn();
        }
        public virtual string materialName {
            get { return _materialName; }
            set { _materialName = value; }
        }
        public virtual intVector3 myBlockSize {
            get { return _myBlockSize; }
            set { _myBlockSize = value; }
        }
        public Vector3 myBlockSizeFloat {
            get { return new Vector3(myBlockSize.x, myBlockSize.y, myBlockSize.z); }
            set { throw new InvalidOperationException("You cant set this value directly. Use myBlockSize for it."); }
        }
        public virtual PhysicsMotionType physicsMotionType
        {
            get { return _physicsMotionType; }
            set
            {
                _physicsMotionType = value;
                SetupPhysicsFromModel(value);
            }
        }

        private intVector3 _myBlockSize = new intVector3(1, 1, 1);
        private PhysicsMotionType _physicsMotionType = PhysicsMotionType.Static;
        private string _materialName = "materials/blocks/stair_block.vmat";
        public Model _model;
    }
}

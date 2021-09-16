using System;
using Sandbox;

using TerryBros.Settings;

namespace TerryBros.LevelElements
{
    public partial class BlockEntity : ModelEntity
    {
        public BlockEntity() { }

        public BlockEntity(Vector3 gridPosition) : this()
        {
            Log.Info("Set to Constructor 0,0,0");
            //Note: Setting the Position in a constructor as client leads to a Non-Authority error.
            //But if you dont set the position later, you can't see this entity,
            //which seems to correlate with the modelbuilder entities on the server not fully syncing to the client.
            if (Host.IsServer)
            {
                Position = gridPosition;
            }
        }

        //TODO: Check for a direct Facepunch fix
        // Currently a Workaround because SetModel cant be called in the constructor
        // See issue: https://github.com/Facepunch/sbox-issues/issues/219
        // Note: Spawn gets called before the constructor
        public override void Spawn()
        {
            VertexBuffer vb = new VertexBuffer();
            vb.Init(true);
            vb.AddCube(Vector3.Zero, myBlockSizeFloat * globalSettings.blockSize, Rotation.LookAt(globalSettings.forwardDir, -globalSettings.upwardDir));

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
            get { return globalSettings.ConvertLocalToGlobalCoordinates(myBlockSize); }
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
        /// <summary>
        /// Offsets the Position for Blockentities, so that their first block is directly on the grid.
        /// Normally this is the center, but we use the most nearest (z), lowest (y), left (x) corner.
        /// </summary>
        public override Vector3 Position
        {
            get
            {
                return base.Position - (myBlockSizeFloat - new Vector3(1, 1, 1)) * globalSettings.blockSize / 2;
            }
            set
            {
                base.Position = value + (myBlockSizeFloat - new Vector3(1, 1, 1)) * globalSettings.blockSize / 2;
            }
        }

        private intVector3 _myBlockSize = new intVector3(1, 1, 1);
        private PhysicsMotionType _physicsMotionType = PhysicsMotionType.Static;
        private string _materialName = "materials/blocks/stair_block.vmat";
        public Model _model;
    }
}

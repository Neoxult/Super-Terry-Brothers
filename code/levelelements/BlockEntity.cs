using System;
using System.Collections.Generic;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.Levels;
using TerryBros.Settings;
using TerryBros.Utils;

namespace TerryBros.LevelElements
{
    public partial class BlockData
    {
        public string Name { get; set; }
        public string MaterialName { get; set; }
        public Type Type { get; set; }
    }

    public abstract class BlockEntity : ModelEntity
    {
        public virtual string MaterialName
        {
            get => _materialName;
            set { _materialName = value; }
        }
        private string _materialName = "materials/blocks/stair_block.vmat";

        public virtual IntVector3 BlockSize
        {
            get => _blockSize;
            set { _blockSize = value; }
        }
        private IntVector3 _blockSize = new(1, 1, 1);

        public virtual PhysicsMotionType PhysicsMotionType
        {
            get => _physicsMotionType;
            set
            {
                _physicsMotionType = value;

                SetupPhysicsFromModel(value);
            }
        }
        private PhysicsMotionType _physicsMotionType = PhysicsMotionType.Static;

        public Vector3 BlockSizeFloat
        {
            get => GlobalSettings.ConvertLocalToGlobalCoordinates(BlockSize);
        }

        /// <summary>
        /// Offsets the Position for Blockentities, so that their first block is directly on the grid.
        /// Normally this is the center, but we use the most nearest (z), lowest (y), left (x) corner.
        /// </summary>
        public override Vector3 Position
        {
            get => base.Position - (BlockSizeFloat - new Vector3(1, 1, 1)) * GlobalSettings.BlockSize / 2;
            set
            {
                base.Position = value + (BlockSizeFloat - new Vector3(1, 1, 1)) * GlobalSettings.BlockSize / 2;

                Level level = STBGame.CurrentLevel;
                IntBBox intBBox = level.LevelBoundsBlocks;

                int x = GridX;
                int y = GridY;
                int z = GridZ;

                if (x < intBBox.Mins.x)
                {
                    intBBox.Mins.x = x;
                }

                if (x > intBBox.Maxs.x)
                {
                    intBBox.Maxs.x = x;
                }

                if (y < intBBox.Mins.y)
                {
                    intBBox.Mins.y = y;
                }

                if (y + 5 > intBBox.Maxs.y)
                {
                    intBBox.Maxs.y = y + 5;
                }

                if (z < intBBox.Mins.z)
                {
                    intBBox.Mins.z = z;
                }

                if (z > intBBox.Maxs.z)
                {
                    intBBox.Maxs.z = z;
                }

                level.LevelBoundsBlocks = intBBox;

                level.GridBlocks.TryGetValue(x, out Dictionary<int, BlockEntity> dict);

                if (dict == null)
                {
                    dict = new();

                    level.GridBlocks.Add(x, dict);
                }

                dict.TryGetValue(y, out BlockEntity blockEntity);

                if (blockEntity != null)
                {
                    dict.Remove(y);
                }

                dict[y] = this;
            }
        }

        public Vector3 Mins => Position - GlobalSettings.BlockSize / 2;
        public Vector3 Maxs => Mins + BlockSizeFloat * GlobalSettings.BlockSize;

        public int GridX => GlobalSettings.GetGridCoordinatesForBlockPos(Position).x;
        public int GridY => GlobalSettings.GetGridCoordinatesForBlockPos(Position).y;
        public int GridZ => GlobalSettings.GetGridCoordinatesForBlockPos(Position).z;

        public BlockEntity() : base()
        {

        }

        public BlockEntity(Vector3 gridPosition) : this()
        {
            //Note: Setting the Position in a constructor as client leads to a Non-Authority error.
            //But if you dont set the position later, you can't see this entity,
            //which seems to correlate with the modelbuilder entities on the server not fully syncing to the client.
            if (Host.IsServer)
            {
                Position = gridPosition;
            }
        }

        public BlockData GetBlockData()
        {
            Type type = GetType();

            return new BlockData
            {
                Type = type,
                Name = type.FullName.Replace(type.Namespace, "").TrimStart('.'),
                MaterialName = MaterialName
            };
        }

        //TODO: Check for a direct Facepunch fix
        // Currently a Workaround because SetModel cant be called in the constructor
        // See issue: https://github.com/Facepunch/sbox-issues/issues/219
        // Note: Spawn gets called before the constructor
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

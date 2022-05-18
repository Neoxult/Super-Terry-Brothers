using Sandbox;

using TerryBros.Settings;
using TerryBros.Utils;

namespace TerryBros.LevelElements
{
    public partial class BlockEntity : ModelEntity
    {
        public IntVector3 BlockSize { get; set; } = new(1, 1, 1);

        public virtual PhysicsMotionType PhysicsMotionType => PhysicsMotionType.Static;

        public Vector3 GlobalBlockSize
        {
            get => GlobalSettings.ConvertLocalToGlobalScale(BlockSize);
        }

        /// <summary>
        /// Its the position in the new local Coordinate system
        /// x,y being the screen horizontally, vertically and z being the depth
        /// </summary>
        public override Vector3 LocalPosition
        {
            get => GlobalSettings.ConvertGlobalToLocalCoordinates(Position);
            set => Position = GlobalSettings.ConvertLocalToGlobalCoordinates(value);
        }

        /// <summary>
        /// Offsets the Position for Blockentities, so that their first block is directly on the grid.
        /// Normally this is the center, but we use the most nearest (z), lowest (y), left (x) corner.
        /// </summary>
        public override Vector3 Position
        {
            get => base.Position - GlobalSettings.ConvertLocalToGlobalScale((BlockSize - new IntVector3(1, 1, 1)) * GlobalSettings.BlockSize * 0.5f);
            set
            {
                base.Position = value + GlobalSettings.ConvertLocalToGlobalScale((BlockSize - new IntVector3(1, 1, 1)) * GlobalSettings.BlockSize * 0.5f);
            }
        }

        public IntVector3 GridPosition
        {
            get => GlobalSettings.GetGridCoordinatesForBlockPos(Position);
            set => Position = GlobalSettings.GetBlockPosForGridCoordinates(value);
        }

        public BlockAsset Asset
        {
            get => _asset;
            set
            {
                _asset = value;

                if (_asset != null)
                {
                    BlockSize = (IntVector3) _asset.BlockSize;

                    SetModel(_asset.ModelPath);

                    if (Host.IsClient)
                    {
                        SetMaterialOverride(_asset.Material);
                    }
                }
            }
        }
        private BlockAsset _asset;

        public override void Spawn()
        {
            SetupPhysicsFromModel(PhysicsMotionType);

            Scale = GlobalSettings.BlockSize * 0.01f; // WorldSpaceBounds.Size.z;

            base.Spawn();
        }

        public static BlockEntity FromAsset(BlockAsset asset)
        {
            if (asset.Name == "__delete__")
            {
                Log.Error("Found restricted Block asset name '__delete__'");

                return null;
            }

            BlockEntity blockEntity = asset.Category switch
            {
                BlockAsset.Categories.Goal => new Goal(),
                BlockAsset.Categories.CheckPoint => new Checkpoint(),
                _ => new()
            };

            blockEntity.Asset = asset;

            return blockEntity;
        }

        public static BlockEntity FromPath(string assetPath) => FromAsset(Resource.FromPath<BlockAsset>(assetPath));

        public static BlockEntity FromName(string name)
        {
            foreach (BlockAsset asset in BlockAsset.All)
            {
                if (asset.Name == name)
                {
                    return FromAsset(asset);
                }
            }

            return null;
        }
    }
}

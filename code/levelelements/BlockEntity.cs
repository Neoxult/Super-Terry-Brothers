using System;
using System.Collections.Generic;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.Levels;
using TerryBros.Settings;
using TerryBros.Utils;

namespace TerryBros.LevelElements
{
    public class BlockData
    {
        public string Name { get; set; }
        public string MaterialName;
        public Type Type;
    }

    public abstract class BlockEntity : ModelEntity
    {
        public virtual string MaterialName => "materials/blocks/stair_block.vmat";
        public virtual IntVector3 BlockSize => new(1, 1, 1);

        public string TypeName
        {
            get => GetType().FullName.Replace(GetType().Namespace, "").TrimStart('.');
        }

        public virtual PhysicsMotionType PhysicsMotionType => PhysicsMotionType.Static;

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
                IntVector3 blockPosition = GlobalSettings.GetGridCoordinatesForBlockPos(Position);

                GridX = blockPosition.x;
                GridY = blockPosition.y;
                GridZ = blockPosition.z;

                if (GridX < intBBox.Mins.x)
                {
                    intBBox.Mins.x = GridX;
                }

                if (GridX > intBBox.Maxs.x)
                {
                    intBBox.Maxs.x = GridX;
                }

                if (GridY < intBBox.Mins.y)
                {
                    intBBox.Mins.y = GridY;
                }

                if (GridY + 5 > intBBox.Maxs.y)
                {
                    intBBox.Maxs.y = GridY + 5;
                }

                if (GridZ < intBBox.Mins.z)
                {
                    intBBox.Mins.z = GridZ;
                }

                if (GridZ > intBBox.Maxs.z)
                {
                    intBBox.Maxs.z = GridZ;
                }

                level.LevelBoundsBlocks = intBBox;

                level.GridBlocks.TryGetValue(GridX, out Dictionary<int, BlockEntity> dict);

                if (dict == null)
                {
                    dict = new();

                    level.GridBlocks.Add(GridX, dict);
                }

                dict.TryGetValue(GridY, out BlockEntity blockEntity);

                if (blockEntity != null)
                {
                    dict.Remove(GridY);
                }

                dict[GridY] = this;
            }
        }

        public Vector3 Mins => Position - GlobalSettings.BlockSize / 2;
        public Vector3 Maxs => Mins + BlockSizeFloat * GlobalSettings.BlockSize;

        public int GridX = 0;
        public int GridY = 0;
        public int GridZ = 0;

        public BlockEntity() : base()
        {

        }

        public BlockData GetBlockData()
        {
            Type type = GetType();

            return new BlockData
            {
                Type = type,
                Name = TypeName,
                MaterialName = MaterialName
            };
        }

        public static Type GetByName(string name)
        {
            foreach (Type t in Library.GetAll<BlockEntity>())
            {
                if (!t.IsAbstract && !t.ContainsGenericParameters)
                {
                    if (t.FullName.Replace(t.Namespace, "").TrimStart('.') == name)
                    {
                        return t;
                    }
                }
            }

            return null;
        }
    }
}

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

                _gridPosition = GlobalSettings.GetGridCoordinatesForBlockPos(Position);

                STBGame.CurrentLevel?.RegisterBlock(this);
            }
        }

        public Vector3 Mins => Position - GlobalSettings.BlockSize / 2;
        public Vector3 Maxs => Mins + BlockSizeFloat * GlobalSettings.BlockSize;

        public IntVector3 GridPosition
        {
            get => _gridPosition;
        }

        private IntVector3 _gridPosition = new IntVector3(0, 0, 0);
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

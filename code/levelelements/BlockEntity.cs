using System;

using Sandbox;

using TerryBros.Gamemode;
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
        /// <summary>
        /// Note: Translucent materials are not properly shown on the editor UI,
        /// so use pngs or non-translucent vmats for now on Models that dont need the MaterialName otherwise
        /// </summary>
        public virtual string MaterialName => "materials/blocks/stair_block.vmat";
        public virtual IntVector3 BlockSize => new(1, 1, 1);

        public string TypeName
        {
            get => GetType().FullName.Replace(GetType().Namespace, "").TrimStart('.');
        }

        public virtual PhysicsMotionType PhysicsMotionType => PhysicsMotionType.Static;

        public Vector3 GlobalBlockSize
        {
            get => GlobalSettings.ConvertLocalToGlobalScale(BlockSize);
        }

        /// <summary>
        /// Its the position in the new local Coordinate system
        /// x,y being the screen horizontally, vertically and z being the depth
        /// </summary>
        [Predicted]
        public override Vector3 LocalPosition
        {
            get => GlobalSettings.ConvertGlobalToLocalCoordinates(Position);
            set => Position = GlobalSettings.ConvertLocalToGlobalCoordinates(value);
        }

        /// <summary>
        /// Offsets the Position for Blockentities, so that their first block is directly on the grid.
        /// Normally this is the center, but we use the most nearest (z), lowest (y), left (x) corner.
        /// </summary>
        [Predicted]
        public override Vector3 Position
        {
            get => base.Position - GlobalSettings.ConvertLocalToGlobalScale((BlockSize - new IntVector3(1, 1, 1)) * GlobalSettings.BlockSize / 2);
            set
            {
                base.Position = value + GlobalSettings.ConvertLocalToGlobalScale((BlockSize - new IntVector3(1, 1, 1)) * GlobalSettings.BlockSize / 2);

                STBGame.CurrentLevel?.RegisterBlock(this);
            }
        }

        [Predicted]
        public IntVector3 GridPosition
        {
            get => GlobalSettings.GetGridCoordinatesForBlockPos(Position);
            set => Position = GlobalSettings.GetBlockPosForGridCoordinates(value);
        }

        public BlockEntity() : base()
        {
            STBGame.AddLateInitializeAction(LateInitialize);
        }

        /// <summary>
        /// Add work here, that cant be done in a constructor and needs a later initialize
        /// </summary>
        public virtual void LateInitialize() { }
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

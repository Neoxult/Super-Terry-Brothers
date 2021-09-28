using Sandbox;

using TerryBros.Utils;
using TerryBros.Settings;

namespace TerryBros.LevelElements
{
    public partial class BrickClayTop : BlockEntity
    {
        public override string MaterialName => "materials/blocks/brick_clay_top.vmat";
        public override string ModelName => "models/blocks/layered_block.vmdl";
        public override bool UseModel => true;
        public override IntVector3 BlockSize => new(1, 1, 1);
        public override Vector3 Position
        {
            get { return base.Position + GlobalSettings.BlockSize / 2 * GlobalSettings.UpwardDir; }
            set { base.Position = value - GlobalSettings.BlockSize / 2 * GlobalSettings.UpwardDir; }
        }
        public BrickClayTop() : base()
        {
            Scale = GlobalSettings.BlockSize / WorldSpaceBounds.Size.z;
        }

        public BrickClayTop(Vector3 position) : base(position)
        {
            Transmit = TransmitType.Always;
            CollisionGroup = CollisionGroup.Always;
            EnableAllCollisions = true;
            EnableHitboxes = true;
        }
    }
}

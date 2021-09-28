using Sandbox;

using TerryBros.Utils;
using TerryBros.Settings;

namespace TerryBros.LevelElements
{
    public partial class LeavesBlooming : BlockEntity
    {
        public override string MaterialName => "materials/blocks/leaves_blooming_4k.vmat";
        public override string ModelName => "models/blocks/leaves_blooming.vmdl";
        public override bool UseModel => true;
        public override bool OverrideMaterial => false;
        public override IntVector3 BlockSize => new(1, 1, 1);
        public override Vector3 Position
        {
            get { return base.Position + GlobalSettings.BlockSize / 2 * GlobalSettings.UpwardDir; }
            set { base.Position = value - GlobalSettings.BlockSize / 2 * GlobalSettings.UpwardDir; }
        }
        public LeavesBlooming() : base()
        {
            Scale = GlobalSettings.BlockSize / WorldSpaceBounds.Size.z;
        }

        public LeavesBlooming(Vector3 position) : base(position)
        {
            Transmit = TransmitType.Always;
            CollisionGroup = CollisionGroup.Always;
            EnableAllCollisions = true;
            EnableHitboxes = true;
        }
    }
}

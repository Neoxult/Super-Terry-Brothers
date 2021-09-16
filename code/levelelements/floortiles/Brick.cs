using Sandbox;

using TerryBros.Settings;

namespace TerryBros.LevelElements
{
    public partial class Brick : BlockEntity
    {
        public override string materialName => "materials/blocks/stair_block.vmat";
        public override intVector3 myBlockSize => new intVector3(1, 1, 1);
        public Brick() : base() { }
        public Brick(Vector3 position) : base(position)
        {
            Transmit = TransmitType.Always;
            CollisionGroup = CollisionGroup.Always;
            EnableAllCollisions = true;
            EnableHitboxes = true;
        }
    }
}

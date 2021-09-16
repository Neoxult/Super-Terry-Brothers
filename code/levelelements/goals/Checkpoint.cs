using Sandbox;

using TerryBros.Settings;

namespace TerryBros.LevelElements
{
    public partial class Checkpoint : BlockEntity
    {
        public Checkpoint() : this(globalSettings.GetBlockPosForGridCoordinates(0,0)) { }
        public Checkpoint(Vector3 gridPosition) : base(gridPosition)
        {
            Transmit = TransmitType.Always;
            CollisionGroup = CollisionGroup.Trigger;
            EnableAllCollisions = false;
            EnableHitboxes = true;
            RenderColor = Color.Blue;
        }
        public override string materialName => "materials/blocks/stair_block.vmat";
        public override intVector3 myBlockSize => new intVector3(1, 3, 1);
    }
}

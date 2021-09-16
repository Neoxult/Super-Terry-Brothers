using Sandbox;

using TerryBros.Settings;
using TerryBros.Gamemode;
using TerryBros.Player;

namespace TerryBros.LevelElements
{
    public partial class Goal : BlockEntity
    {
        public override string materialName => "materials/blocks/stair_block.vmat";
        public override intVector3 myBlockSize => new intVector3(1, 8, 1);
        public Goal() : this(globalSettings.GetBlockPosForGridCoordinates(0,0)) { }
        public Goal(Vector3 position) : base(position)
        {
            Transmit = TransmitType.Always;
            CollisionGroup = CollisionGroup.Trigger;
            EnableAllCollisions = true;
            EnableHitboxes = true;
            RenderColor = Color.Green;
        }
        public override void Touch(Entity other)
        {
            base.Touch(other);
            if (other is not TerryBrosPlayer player)
            {
                return;
            }
            STBGame.LevelFinished(player);
        }
    }
}

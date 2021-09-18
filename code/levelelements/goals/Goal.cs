using Sandbox;

using TerryBros.Gamemode;
using TerryBros.Player;
using TerryBros.Settings;
using TerryBros.Utils;

namespace TerryBros.LevelElements
{
    public partial class Goal : BlockEntity
    {
        public override string MaterialName => "materials/blocks/stair_block.vmat";
        public override IntVector3 BlockSize => new(1, 8, 1);

        public Goal() : this(GlobalSettings.GetBlockPosForGridCoordinates(0, 0))
        {

        }

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

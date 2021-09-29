using Sandbox;

using TerryBros.Events;
using TerryBros.Gamemode;
using TerryBros.Player;
using TerryBros.Utils;

namespace TerryBros.LevelElements
{
    public partial class Goal : BlockMaterialEntity
    {
        public override string MaterialName => "materials/blocks/stair_block.vmat";
        public override IntVector3 BlockSize => new(1, 8, 1);

        public Goal() : base()
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

            if (!Host.IsServer || other is not TerryBrosPlayer player)
            {
                return;
            }

            Event.Run(TBEvent.Level.GoalReached, player);
        }
    }
}

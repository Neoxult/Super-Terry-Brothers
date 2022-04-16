using Sandbox;

using TerryBros.Events;
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
            CollisionGroup = CollisionGroup.Trigger;
            RenderColor = Color.Green.WithAlpha(0.5f);
        }

        public override void Touch(Entity other)
        {
            base.Touch(other);

            if (other is not TerryBrosPlayer player)
            {
                return;
            }

            Event.Run(TBEvent.Level.GOAL_REACHED, player);
        }
    }
}

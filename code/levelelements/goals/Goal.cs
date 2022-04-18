using Sandbox;

using TerryBros.Events;
using TerryBros.Utils;

namespace TerryBros.LevelElements
{
    public partial class Goal : BlockEntity
    {
        public override IntVector3 BlockSize => new(1, 8, 1);

        public Goal() : base()
        {
            CollisionGroup = CollisionGroup.Trigger;
            RenderColor = Color.Green.WithAlpha(0.5f);
        }

        public override void Touch(Entity other)
        {
            base.Touch(other);

            if (other is not Player player)
            {
                return;
            }

            Event.Run(TBEvent.Level.GOAL_REACHED, player);
        }
    }
}

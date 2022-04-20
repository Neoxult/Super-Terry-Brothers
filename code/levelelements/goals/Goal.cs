using Sandbox;

using TerryBros.Events;

namespace TerryBros.LevelElements
{
    [Library("stb_goal"), Hammer.Skip]
    public partial class Goal : BlockEntity
    {
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

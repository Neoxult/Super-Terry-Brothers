using Sandbox;

using TerryBros.Settings;

namespace TerryBros.Player.Controller
{
    public partial class MovementController : WalkController
    {
        public override void Simulate()
        {
            if (Pawn is not TerryBrosPlayer player)
            {
                return;
            }

            //TODO: Do a proper implementation of the controller
            Input.Rotation = Rotation.LookAt(globalSettings.forwardDir, globalSettings.upwardDir);

            //TODO: Correct for current moveDirection
            Input.Forward = -1f * Input.Left;
            Input.Left = 0f;
            base.Simulate();
        }
    }
}

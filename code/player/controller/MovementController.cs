using System;

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

            if (Input.Left != 0f)
            {
                Input.Rotation = Rotation.LookAt(Input.Left > 0f ? -globalSettings.forwardDir : globalSettings.forwardDir, globalSettings.upwardDir);
                Input.Forward = Math.Abs(Input.Left);
                Input.Left = 0f;
            }

            base.Simulate();
        }
    }
}

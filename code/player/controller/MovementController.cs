using System;

using Sandbox;

using TerryBros.Settings;

namespace TerryBros.Player.Controller
{
    public partial class MovementController : WalkController
    {
        public Rotation CurrentRotation;
        public bool Forward = true;

        public override void Simulate()
        {
            if (Pawn is not TerryBrosPlayer player)
            {
                return;
            }

            if (Input.Left != 0f)
            {
                Forward = Input.Left <= 0f;
                Input.Rotation = Rotation.LookAt(Forward ? globalSettings.forwardDir : -globalSettings.forwardDir, globalSettings.upwardDir);
                Input.Forward = Math.Abs(Input.Left);
                Input.Left = 0f;

                CurrentRotation = Input.Rotation;
            }
            else
            {
                if (CurrentRotation == null)
                {
                    Input.Rotation = Rotation.LookAt(globalSettings.forwardDir, globalSettings.upwardDir);
                }

                Input.Rotation = CurrentRotation;
            }

            base.Simulate();
        }
    }
}

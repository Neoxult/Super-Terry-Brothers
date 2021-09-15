using System;

using Sandbox;

using TerryBros.Settings;

namespace TerryBros.Player.Controller
{
    public partial class MovementController : WalkController
    {
        public bool Forward = true;
        public bool IsJumpAttacking = false;
        public TimeSince JumpAttackStarted = 0f;
        public Vector3 JumpAttackPosition;

        public override void Simulate()
        {
            if (Pawn is not TerryBrosPlayer player)
            {
                return;
            }

            //TODO: Find out if the game is really lagging with sprinting
            //SprintSpeed = DefaultSpeed;

            if (Input.Left != 0f)
            {
                Forward = Input.Left <= 0f;
                Input.Forward = Math.Abs(Input.Left);
                Input.Left = 0f;
            }

            Input.Rotation = Rotation.LookAt(Forward ? globalSettings.forwardDir : -globalSettings.forwardDir, globalSettings.upwardDir);

            base.Simulate();

            if (Host.IsServer)
            {
                if (!IsJumpAttacking && player.GroundEntity == null && Input.Pressed(InputButton.Jump))
                {
                    IsJumpAttacking = true;
                    JumpAttackStarted = 0f;
                    JumpAttackPosition = player.Position;

                    TraceResult tr = Trace.Ray(player.Position, player.Position + Vector3.Down * 5000f)
                        .Ignore(player)
                        .HitLayer(CollisionLayer.All)
                        .Radius(0f)
                        .Run();

                    DebugOverlay.Line(tr.StartPos, tr.EndPos, Color.Red, 10f);
                }
                else if (player.GroundEntity != null)
                {
                    IsJumpAttacking = false;
                }
            }
        }
    }
}

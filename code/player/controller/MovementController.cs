using System;

using Sandbox;

using TerryBros.Settings;

namespace TerryBros
{
    public partial class MovementController : WalkController
    {
        public bool Forward
        {
            get => _forward;
            set
            {
                if (_forward != value)
                {
                    MovedirectionChanged = 0f;
                    _wasMovedirectionChanged = true;
                    _forward = value;
                }
            }
        }
        private bool _forward = true;

        [Net, Predicted]
        public bool IsFreeze { get; set; }

        private bool _wasMovedirectionChanged = false;

        public TimeSince MovedirectionChanged = 1f;

        public bool IsJumpAttacking = false;
        public TimeSince JumpAttackStarted = 0f;
        public Vector3 JumpAttackPosition;
        public bool IsJumpAttackTriggered = false;

        public bool IsJumping = false;

        public BBox GetBounds() => new(mins, maxs);

        public override void Simulate()
        {
            if (Pawn is not Player player || player.IsInMenu || IsFreeze)
            {
                return;
            }

            //TODO: Find out if the game is really lagging with sprinting
            //SprintSpeed = DefaultSpeed;

            if (!IsJumpAttacking)
            {
                if (Input.Left != 0f)
                {
                    Forward = Input.Left <= 0f;
                    Input.Forward = Math.Abs(Input.Left);
                    Input.Left = 0f;
                }

                Input.Rotation = Rotation.LookAt(Forward ? GlobalSettings.ForwardDir : -GlobalSettings.ForwardDir, GlobalSettings.UpwardDir);
            }

            CalculateSimulation();
        }

        private void CalculateSimulation()
        {
            EyeLocalPosition = Vector3.Up * (EyeHeight * Pawn.Scale);
            UpdateBBox();

            EyeLocalPosition += TraceOffset;
            EyeRotation = Input.Rotation;

            if (_wasMovedirectionChanged)
            {
                Rotation = Input.Rotation;

                _wasMovedirectionChanged = false;
            }

            if (Unstuck.TestAndFix())
            {
                return;
            }

            if (GroundEntity != null)
            {
                if (IsJumpAttacking)
                {
                    IsJumping = false;
                    IsJumpAttacking = false;
                    IsJumpAttackTriggered = false;
                }
            }
            else if (IsJumpAttacking)
            {
                if (JumpAttackStarted < 0.8f)
                {
                    Velocity = Vector3.Zero;

                    (Pawn as Player).Animator.SetAnimParameter("jumpattack", 0.8f);
                }
                else if (GroundEntity == null && !IsJumpAttackTriggered)
                {
                    IsJumpAttackTriggered = true;

                    Velocity = Vector3.Down * 800f;
                }
            }

            CheckLadder();

            Swimming = Pawn.WaterLevel > 0.6f;

            //
            // Start Gravity
            //
            if (!Swimming && !IsTouchingLadder)
            {
                if (!IsJumpAttacking)
                {
                    Velocity -= new Vector3(0, 0, Gravity * 0.5f) * Time.Delta;
                    Velocity += new Vector3(0, 0, BaseVelocity.z) * Time.Delta;
                }

                BaseVelocity = BaseVelocity.WithZ(0);
            }

            if (Input.Pressed(InputButton.Jump))
            {
                CheckJumpButton();
            }

            if (GroundEntity != null)
            {
                Velocity = Velocity.WithZ(0);

                ApplyFriction(GroundFriction * SurfaceFriction);
            }
            else if (Input.Pressed(InputButton.Duck))
            {
                if (!IsJumpAttacking && IsJumping)
                {
                    IsJumpAttacking = true;
                    JumpAttackStarted = 0f;

                    AddEvent("jumpattack");
                }
            }

            if (!IsJumpAttacking)
            {
                WishVelocity = new Vector3(Input.Forward, Input.Left, 0);

                float inSpeed = WishVelocity.Length.Clamp(0, 1);

                WishVelocity *= Input.Rotation;

                if (!Swimming && !IsTouchingLadder)
                {
                    WishVelocity = WishVelocity.WithZ(0);
                }

                WishVelocity = WishVelocity.Normal * inSpeed;
                WishVelocity *= GetWishSpeed();
            }

            Duck.PreTick();

            bool bStayOnGround = false;

            if (Swimming)
            {
                ApplyFriction(1);
                WaterMove();
            }
            else if (IsTouchingLadder)
            {
                LadderMove();
            }
            else if (GroundEntity != null)
            {
                bStayOnGround = true;

                WalkMove();
            }
            else
            {
                AirMove();
            }

            CategorizePosition(bStayOnGround);

            if (!Swimming && !IsTouchingLadder)
            {
                Velocity -= new Vector3(0, 0, Gravity * 0.5f) * Time.Delta;
            }

            if (GroundEntity != null)
            {
                Velocity = Velocity.WithZ(0);
            }

            DebugMovement();
        }

        private void DebugMovement()
        {
            if (Debug)
            {
                DebugOverlay.Box(Position + TraceOffset, mins, maxs, Color.Red);
                DebugOverlay.Box(Position, mins, maxs, Color.Blue);

                int lineOffset = 0;

                if (Host.IsServer)
                {
                    lineOffset = 10;
                }

                DebugOverlay.ScreenText(lineOffset + 0, $"        Position: {Position}");
                DebugOverlay.ScreenText(lineOffset + 1, $"        Velocity: {Velocity}");
                DebugOverlay.ScreenText(lineOffset + 2, $"    BaseVelocity: {BaseVelocity}");
                DebugOverlay.ScreenText(lineOffset + 3, $"    GroundEntity: {GroundEntity} [{GroundEntity?.Velocity}]");
                DebugOverlay.ScreenText(lineOffset + 4, $" SurfaceFriction: {SurfaceFriction}");
                DebugOverlay.ScreenText(lineOffset + 5, $"    WishVelocity: {WishVelocity}");
            }
        }

        public override void AirMove()
        {
            Vector3 wishdir = WishVelocity.Normal;
            float wishspeed = WishVelocity.Length;

            if (!IsJumpAttacking)
            {
                Accelerate(wishdir, wishspeed, AirControl, AirAcceleration);
            }

            Velocity += BaseVelocity;

            Move();

            Velocity -= BaseVelocity;
        }

        internal bool IsTouchingLadder { get; set; } = false;
        internal Vector3 LadderNormal { get; set; }

        public override void CheckLadder()
        {
            if (IsTouchingLadder && Input.Pressed(InputButton.Jump))
            {
                Velocity = LadderNormal * 100.0f;
                IsTouchingLadder = false;

                return;
            }

            const float LADDER_DISTANCE = 1.0f;
            Vector3 start = Position;
            Vector3 end = start + (IsTouchingLadder ? (LadderNormal * -1.0f) : WishVelocity.Normal) * LADDER_DISTANCE;

            TraceResult pm = Trace.Ray(start, end)
                .Size(mins, maxs)
                .HitLayer(CollisionLayer.All, false)
                .HitLayer(CollisionLayer.LADDER, true)
                .Ignore(Pawn)
                .Run();

            IsTouchingLadder = false;

            if (pm.Hit)
            {
                IsTouchingLadder = true;
                LadderNormal = pm.Normal;
            }
        }

        public override void WalkMove()
        {
            Vector3 wishdir = WishVelocity.Normal;
            float wishspeed = WishVelocity.Length;

            WishVelocity = WishVelocity.WithZ(0);
            WishVelocity = WishVelocity.Normal * wishspeed;

            Velocity = Velocity.WithZ(0);
            Accelerate(wishdir, wishspeed, 0, Acceleration);
            Velocity = Velocity.WithZ(0);

            // Add in any base velocity to the current velocity.
            Velocity += BaseVelocity;

            try
            {
                if (Velocity.Length < 1.0f)
                {
                    Velocity = Vector3.Zero;

                    return;
                }

                Vector3 dest = (Position + Velocity * Time.Delta).WithZ(Position.z);
                TraceResult pm = TraceBBox(Position, dest);

                if (pm.Fraction == 1)
                {
                    Position = pm.EndPosition;

                    StayOnGround();

                    return;
                }

                Move();
            }
            finally
            {
                // Now pull the base velocity back out.
                // Base velocity is set if you are on a moving object
                Velocity -= BaseVelocity;
            }

            StayOnGround();
        }

		/// <summary>
		/// Traces the bbox and returns the trace result.
		/// LiftFeet will move the start position up by this amount, while keeping the top of the bbox at the same
		/// position. This is good when tracing down because you won't be tracing through the ceiling above.
		/// </summary>
		public override TraceResult TraceBBox(Vector3 start, Vector3 end, Vector3 mins, Vector3 maxs, float liftFeet = 0.0f)
		{
			if (liftFeet > 0)
			{
				start += Vector3.Up * liftFeet;
				maxs = maxs.WithZ(maxs.z - liftFeet);
			}

			TraceResult tr = Trace.Ray(start + TraceOffset, end + TraceOffset)
                .Size(mins, maxs)
                .HitLayer(CollisionLayer.All, false)
                .HitLayer(CollisionLayer.Solid, true)
                .HitLayer(CollisionLayer.GRATE, true)
                .HitLayer(CollisionLayer.PLAYER_CLIP, true)
                .HitLayer(CollisionLayer.WINDOW, true)
                .HitLayer(CollisionLayer.NPC, true)
                .Ignore(Pawn)
                .WithoutTags("player")
                .Run();

			tr.EndPosition -= TraceOffset;

			return tr;
		}

		public override void Move()
		{
			MoveHelper mover = new(Position, Velocity);
			mover.Trace = mover.Trace.Size(mins, maxs).Ignore(Pawn).WithoutTags("player");
			mover.MaxStandableAngle = GroundAngle;

			mover.TryMove(Time.Delta);

			Position = mover.Position;
			Velocity = mover.Velocity;
		}

        public override void CheckJumpButton()
        {
            // If we are in the water most of the way...
            if (Swimming)
            {
                // swimming, not jumping
                ClearGroundEntity();

                Velocity = Velocity.WithZ(100);

                return;
            }

            if (GroundEntity == null)
            {
                return;
            }

            IsJumping = true;

            ClearGroundEntity();

            float flGroundFactor = 1.0f;
            float flMul = 268.3281572999747f * 1.2f;
            float startz = Velocity.z;

            if (Duck.IsActive)
            {
                flMul *= 0.8f;
            }

            Velocity = Velocity.WithZ(startz + flMul * flGroundFactor);
            Velocity -= new Vector3(0, 0, Gravity * 0.5f) * Time.Delta;

            AddEvent("jump");
        }
    }
}

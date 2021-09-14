using Sandbox;

namespace TerryBros.Player
{
    using Camera;

    using Controller;

    partial class TerryBrosPlayer : Sandbox.Player
    {
        /// <summary>
        /// The clothing container is what dresses the citizen
        /// </summary>
        public Clothing.Container Clothing = new();

        public TerryBrosPlayer()
        {

        }

        public TerryBrosPlayer(Client cl) : this()
        {
            Clothing.LoadFromClient(cl);
        }

        public override void Respawn()
        {
            SetModel("models/citizen/citizen.vmdl");

            Controller = new MovementController();
            Animator = new StandardPlayerAnimator();
            Camera = new SideScrollerCamera();

            EnableAllCollisions = true;
            EnableDrawing = true;
            EnableHideInFirstPerson = true;
            EnableShadowInFirstPerson = true;

            Clothing.DressEntity(this);

            base.Respawn();
        }

        /// <summary>
        /// Called every tick, clientside and serverside.
        /// </summary>
        public override void Simulate(Client cl)
        {
            base.Simulate(cl);

            SimulateActiveChild(cl, ActiveChild);
        }

        [Event.Physics.PostStep]
        public void OnPhysicsPostStep()
        {
            if (!Host.IsServer || Controller is not MovementController movementController || !movementController.IsJumpAttacking)
            {
                return;
            }

            if (movementController.JumpAttackStarted < 0.8f)
            {
                Velocity = Vector3.Zero;
                Position = movementController.JumpAttackPosition;

                return;
            }

            Velocity = Vector3.Down * 800f;
        }

        public override void OnKilled()
        {
            base.OnKilled();
        }
    }
}

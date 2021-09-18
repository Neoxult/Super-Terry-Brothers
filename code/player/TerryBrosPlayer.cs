using Sandbox;

namespace TerryBros.Player
{
    using Camera;

    using Controller;

    public partial class TerryBrosPlayer : Sandbox.Player
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
            base.Respawn();

            SetModel("models/citizen/citizen.vmdl");

            Controller = new MovementController();
            Animator = new StandardPlayerAnimator();
            Camera = new SideScrollerCamera();

            EnableAllCollisions = true;
            EnableDrawing = true;
            EnableHideInFirstPerson = true;
            EnableShadowInFirstPerson = true;

            Clothing.DressEntity(this);

            Scale = 0.34f;
            PhysicsGroup?.RebuildMass();
            PhysicsGroup?.Wake();
        }

        /// <summary>
        /// Called every tick, clientside and serverside.
        /// </summary>
        public override void Simulate(Client cl)
        {
            base.Simulate(cl);

            SimulateActiveChild(cl, ActiveChild);
        }

        public override void OnKilled()
        {
            base.OnKilled();
        }
    }
}

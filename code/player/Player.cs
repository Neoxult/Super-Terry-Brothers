using Sandbox;

using TerryBros.Settings;

namespace TerryBros
{
    public partial class Player : Sandbox.Player
    {
        /// <summary>
        /// The clothing container is what dresses the citizen
        /// </summary>
        public Clothing.Container Clothing = new();

        public static Model PlayerModel { get; } = Model.Load("models/citizen/citizen.vmdl");

        public LevelElements.SpawnPoint CheckPointSpawn { get; internal set; }

        [ClientVar]
        public static bool Camera2D
        {
            get => _camera2D;
            set
            {
                _camera2D = value;

                if (Local.Client != null && Local.Client.Pawn is Player terryBrosPlayer)
                {
                    terryBrosPlayer.SetCameraDimension();
                }
            }
        }
        private static bool _camera2D = false;

        public bool IsInMenu = false;

        /// <summary>
        /// Its the position in the new local Coordinate system
        /// x -> horizontally, y -> vertically and z -> depth
        /// </summary>
        public override Vector3 LocalPosition
        {
            get => GlobalSettings.ConvertGlobalToLocalCoordinates(Position);
            set => Position = GlobalSettings.ConvertLocalToGlobalCoordinates(value);
        }

        public void SetCameraDimension()
        {
            CameraMode cameraMode = Client.GetValue("leveleditor", false) ? new LevelEditorCamera() : Camera2D ? new SideScroller2DCamera() : new SideScroller3DCamera();

            if (CameraMode?.GetType() != cameraMode.GetType())
            {
                CameraMode = cameraMode;
            }
        }

        public override void Spawn()
        {
            base.Spawn();

            Model = PlayerModel;

            Clothing.DressEntity(this);

            Scale = 0.34f;
            PhysicsGroup?.RebuildMass();

            Controller = new MovementController();
            Animator = new StandardPlayerAnimator();

            EnableAllCollisions = true;
            EnableSelfCollisions = true;

            ClearCollisionLayers();
            AddCollisionLayer(CollisionLayer.Player);

            Tags.Add("player");

            CollisionGroup = CollisionGroup.Player;

            if (Gamemode.STBGame.Instance.State == Gamemode.STBGame.GameState.LevelEditor)
            {
                EnableLevelEditor(true);
            }

            Respawn();
        }

        public override void Respawn()
        {
            base.Respawn();

            ClientRespawn(To.Single(this));
        }

        [ClientRpc]
        public void ClientRespawn()
        {
            SetCameraDimension();
        }

        public override void ClientSpawn()
        {
            base.ClientSpawn();

            UI.Hud.Instance?.RootPanel.AddChild<UI.Menu>();
        }

        /// <summary>
        /// Called every tick, clientside and serverside.
        /// </summary>
        public override void Simulate(Client cl)
        {
            base.Simulate(cl);

            SimulateActiveChild(cl, ActiveChild);
            SimulateLevelNetworking(cl);
        }

        public override void FrameSimulate(Client cl)
        {
            base.FrameSimulate(cl);

            SimulateLevelEditing();
        }

        public override void OnKilled()
        {
            base.OnKilled();
        }
    }
}

using Sandbox;

using TerryBros.Settings;
using TerryBros.UI;
using TerryBros.Utils;

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

        [ClientVar]
        public static bool stb_2D { get; set; } = false;

        public bool IsInLevelBuilder = false;

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

            if (!IsClient || !IsInLevelBuilder)
            {
                return;
            }

            SideScrollerCamera sideScrollerCamera = Camera as SideScrollerCamera;

            Vector3 cameraPos = sideScrollerCamera.Pos;
            cameraPos.x -= Screen.Width * (sideScrollerCamera.OrthoSize / 2f);
            cameraPos.z += Screen.Height * (sideScrollerCamera.OrthoSize / 2f);

            Vector2 mousePosition = Mouse.Position * sideScrollerCamera.OrthoSize;
            Vector3 mousePos = new Vector3(mousePosition.x + cameraPos.x, 0, cameraPos.z - mousePosition.y);

            Vector3 BlockSize = GlobalSettings.BlockSize / 2f;
            IntVector3 intVector3 = GlobalSettings.GetGridCoordinatesForBlockPos(mousePos);
            Vector3 vector3 = GlobalSettings.GetBlockPosForGridCoordinates(intVector3);

            DebugOverlay.Box(vector3 - BlockSize, vector3 + BlockSize, Color.Red, 0.1f);
            DebugOverlay.Box(mousePos - BlockSize, mousePos + BlockSize, Color.Orange, 0.1f);
            DebugOverlay.Box(mousePos - new Vector3(2f, 10f, 2f), mousePos + new Vector3(2f, 10f, 2f), Color.Blue, 0.1f);

            DebugOverlay.Box(cameraPos.WithY(0f) + new Vector3(0, -10, 0), cameraPos.WithY(0f) + new Vector3(Screen.Width * sideScrollerCamera.OrthoSize, 10, -Screen.Height * sideScrollerCamera.OrthoSize), Color.Green, 0.1f);

            if (Input.Pressed(InputButton.Menu))
            {
                ServerCreateBlock(vector3);
            }
        }

        public override void OnKilled()
        {
            base.OnKilled();
        }

        // Just some testing, to create blocks dynamically
        [ServerCmd(Name = "stb_block", Help = "Spawns a block in front of the player's")]
        public static void ServerCreateBlock(Vector3 position)
        {
            TerryBrosPlayer player = ConsoleSystem.Caller.Pawn as TerryBrosPlayer;
            MovementController movementController = player.Controller as MovementController;

            CreateBlock(position);
            ClientCreateBlock(position);
        }

        [ClientRpc]
        public static void ClientCreateBlock(Vector3 position)
        {
            CreateBlock(position);
        }

        public static ModelEntity CreateBlock(Vector3 position)
        {
            ModelEntity modelEntity = new LevelElements.Brick();
            modelEntity.Position = position;

            return modelEntity;
        }

        [ClientCmd("stb_editor")]
        public static void ToggleLevelEditor()
        {
            if (Local.Pawn is not TerryBrosPlayer player)
            {
                return;
            }

            player.IsInLevelBuilder = !player.IsInLevelBuilder;

            LevelBuilder.Instance.Toggle(player.IsInLevelBuilder);
        }
    }
}

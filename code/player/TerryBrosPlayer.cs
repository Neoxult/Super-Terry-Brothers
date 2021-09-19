using System.Collections.Generic;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.LevelElements;
using TerryBros.Levels;
using TerryBros.Settings;
using TerryBros.UI.LevelBuilder;
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

        private IntVector3 _oldGrid = IntVector3.Zero;
        private bool _isDrawing = false;

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

            if (Builder.Instance.IsMouseDown && !_oldGrid.Equals(intVector3))
            {
                _oldGrid = intVector3;
                _isDrawing = true;

                if (Builder.Instance.IsLeftMouseButtonDown)
                {
                    ServerCreateBlock(vector3);
                }
                else if (Builder.Instance.IsRightMouseButtonDown)
                {
                    ServerDeleteBlock(vector3);
                }
            }
            else if (_isDrawing && !Builder.Instance.IsMouseDown)
            {
                _oldGrid = IntVector3.Zero;
                _isDrawing = false;
            }
        }

        public override void OnKilled()
        {
            base.OnKilled();
        }

        // Just some testing, to create blocks dynamically
        [ServerCmd(Name = "stb_block", Help = "Spawns a block in front of the player")]
        public static void ServerCreateBlock(Vector3 position)
        {
            TerryBrosPlayer player = ConsoleSystem.Caller.Pawn as TerryBrosPlayer;
            MovementController movementController = player.Controller as MovementController;

            CreateBlock(position);
            ClientCreateBlock(position);
        }

        // Just some testing, to create blocks dynamically
        [ServerCmd(Name = "stb_block_delete", Help = "Removes a block in front of the player")]
        public static void ServerDeleteBlock(Vector3 position)
        {
            TerryBrosPlayer player = ConsoleSystem.Caller.Pawn as TerryBrosPlayer;
            MovementController movementController = player.Controller as MovementController;

            DeleteBlock(position);
            ClientDeleteBlock(position);
        }

        [ClientRpc]
        public static void ClientCreateBlock(Vector3 position)
        {
            CreateBlock(position);
        }

        [ClientRpc]
        public static void ClientDeleteBlock(Vector3 position)
        {
            DeleteBlock(position);
        }

        public static ModelEntity CreateBlock(Vector3 position)
        {
            Level level = STBGame.CurrentLevel;
            IntVector3 intVector3 = GlobalSettings.GetGridCoordinatesForBlockPos(position);

            level.GridBlocks.TryGetValue(intVector3.x, out Dictionary<int, BlockEntity> dict);

            if (dict == null)
            {
                dict = new();

                level.GridBlocks.Add(intVector3.x, dict);
            }

            dict.TryGetValue(intVector3.y, out BlockEntity blockEntity);

            if (blockEntity == null)
            {
                blockEntity = new LevelElements.Brick();
                blockEntity.Position = position;

                dict[intVector3.y] = blockEntity;

                return blockEntity;
            }

            return blockEntity;
        }

        public static void DeleteBlock(Vector3 position)
        {
            Level level = STBGame.CurrentLevel;
            IntVector3 intVector3 = GlobalSettings.GetGridCoordinatesForBlockPos(position);

            level.GridBlocks.TryGetValue(intVector3.x, out Dictionary<int, BlockEntity> dict);

            if (dict == null)
            {
                dict = new();

                level.GridBlocks.Add(intVector3.x, dict);
            }

            dict.TryGetValue(intVector3.y, out BlockEntity blockEntity);

            if (blockEntity != null)
            {
                dict.Remove(intVector3.y);
                blockEntity.Delete();
            }
        }

        [ClientCmd("stb_editor")]
        public static void ToggleLevelEditor()
        {
            if (Local.Pawn is not TerryBrosPlayer player)
            {
                return;
            }

            player.IsInLevelBuilder = !player.IsInLevelBuilder;

            Builder.Instance.Toggle(player.IsInLevelBuilder);
        }
    }
}

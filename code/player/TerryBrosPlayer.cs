using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

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

        [ClientVar, Change("SetCameraDimension")]
        public static bool stb_2D { get; set; } = false;

        public bool IsInLevelBuilder = false;
        public bool IsInMenu = false;

        /// <summary>
        /// Its the position in the new local Coordinate system
        /// x,y being the screen horizontally, vertically and z being the depth
        /// </summary>
        public override Vector3 LocalPosition
        {
            get => GlobalSettings.ConvertGlobalToLocalCoordinates(Position);
            set => Position = GlobalSettings.ConvertLocalToGlobalCoordinates(value);
        }

        private IntVector3 _oldGrid = IntVector3.Zero;
        private bool _isDrawing = false;

        public TerryBrosPlayer()
        {
        }

        public TerryBrosPlayer(Client cl) : this()
        {
            Clothing.LoadFromClient(cl);
        }
        public void SetCameraDimension()
        {
            //TODO: Separate LevelEditor Camera from SidescrollerCamera
            //Note: Tried here to experiment with own cameras, which makes the leveleditor unusable
            //Camera = stb_2D ? new SideScroller2DCamera() : new SideScrollerCamera();
            Camera = new SideScroller2DCamera();
        }
        public override void Respawn()
        {
            SetModel("models/citizen/citizen.vmdl");

            Controller = new MovementController();
            Animator = new StandardPlayerAnimator();
            SetCameraDimension();

            EnableAllCollisions = true;
            EnableDrawing = true;
            EnableHideInFirstPerson = true;
            EnableShadowInFirstPerson = true;

            Clothing.DressEntity(this);

            Scale = 0.34f;
            PhysicsGroup?.RebuildMass();
            PhysicsGroup?.Wake();

            ClientRespawn(this);

            base.Respawn();
        }

        [ClientRpc]
        public static void ClientRespawn(TerryBrosPlayer player)
        {
            if (player == null || !player.IsValid)
            {
                return;
            }

            // sbox weirdness, controller is reset on serverside, but stays with same vars on clientside
            if (player.Controller is MovementController movementController)
            {
                movementController.Forward = true;
            }
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

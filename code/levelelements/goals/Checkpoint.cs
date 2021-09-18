using System;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.Player;
using TerryBros.Settings;
using TerryBros.Utils;

namespace TerryBros.LevelElements
{
    public partial class Checkpoint : BlockEntity
    {
        public STBSpawn spawnPoint { get; private set; }
        public override string MaterialName => "materials/blocks/stair_block.vmat";
        public override IntVector3 BlockSize => new(1, 3, 1);
        public override Vector3 Position
        {
            get => base.Position;
            set
            {
                base.Position = value;

                if (spawnPoint != null)
                {
                    spawnPoint.Position = value;
                }
            }
        }
        private bool wasTouched = false;

        public Checkpoint() : this(GlobalSettings.GetBlockPosForGridCoordinates(0, 0))
        {

        }

        public Checkpoint(Vector3 position) : base(position)
        {
            Transmit = TransmitType.Always;
            CollisionGroup = CollisionGroup.Trigger;
            EnableAllCollisions = true;
            EnableHitboxes = true;
            RenderColor = Color.Blue;

            spawnPoint = new STBSpawn(position + GlobalSettings.UpwardDir * (GlobalSettings.FigureHeight - GlobalSettings.BlockSize / 2) / 2);
        }

        public void RegisterReset(Action resetDelegate)
        {
            if (resetDelegate != null)
            {
                resetDelegate += ResetCheckPoint;
            }
            else
            {
                resetDelegate = ResetCheckPoint;
            }
        }

        public override void Touch(Entity other)
        {
            base.Touch(other);

            if (wasTouched || other is not TerryBrosPlayer)
            {
                return;
            }

            if (STBGame.CurrentLevel != null)
            {
                wasTouched = true;

                STBGame.CurrentLevel.SetCheckPoint(this);
            }

        }

        private void ResetCheckPoint()
        {
            wasTouched = false;
        }
    }
}

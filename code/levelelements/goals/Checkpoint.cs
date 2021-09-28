using System;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.Player;
using TerryBros.Settings;
using TerryBros.Utils;

namespace TerryBros.LevelElements
{
    public partial class Checkpoint : BlockTextureEntity
    {
        public STBSpawn SpawnPoint { get; private set; }
        public override string MaterialName => "materials/blocks/stair_block.vmat";
        public override IntVector3 BlockSize => new(1, 3, 1);
        public override Vector3 Position
        {
            get => base.Position;
            set
            {
                base.Position = value;

                if (SpawnPoint != null)
                {
                    SpawnPoint.Position = value;
                }
            }
        }
        private bool _wasTouched = false;

        public Checkpoint() : this(GlobalSettings.GetBlockPosForGridCoordinates(0, 0))
        {

        }

        public Checkpoint(Vector3 position) : base()
        {
            Transmit = TransmitType.Always;
            CollisionGroup = CollisionGroup.Trigger;
            EnableAllCollisions = true;
            EnableHitboxes = true;
            RenderColor = Color.Blue;

            SpawnPoint = new STBSpawn(position + GlobalSettings.UpwardDir * (GlobalSettings.FigureHeight - GlobalSettings.BlockSize / 2) / 2);
        }

        public void RegisterReset(ref Action resetDelegate)
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

            if (_wasTouched || other is not TerryBrosPlayer)
            {
                return;
            }

            if (STBGame.CurrentLevel != null)
            {
                _wasTouched = true;

                STBGame.CurrentLevel.SetCheckPoint(this);
            }
        }

        public void ResetCheckPoint()
        {
            _wasTouched = false;
        }
    }
}

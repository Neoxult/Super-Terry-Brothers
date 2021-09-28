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

        public Checkpoint() : base()
        {
            CollisionGroup = CollisionGroup.Trigger;
            RenderColor = Color.Blue;

            SpawnPoint = new STBSpawn();
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
                Log.Info("Set Checkpoint");
                STBGame.CurrentLevel.SetCheckPoint(this);
            }
        }

        public void ResetCheckPoint()
        {
            _wasTouched = false;
        }
    }
}

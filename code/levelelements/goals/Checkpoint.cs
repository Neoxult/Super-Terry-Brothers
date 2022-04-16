using Sandbox;

using TerryBros.Events;
using TerryBros.Gamemode;
using TerryBros.Player;
using TerryBros.Utils;

namespace TerryBros.LevelElements
{
    public partial class Checkpoint : BlockMaterialEntity
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
            RenderColor = Color.Blue.WithAlpha(0.5f);
        }
        public override void Spawn()
        {
            SpawnPoint = new STBSpawn();

            base.Spawn();
        }
        public override void Touch(Entity other)
        {
            base.Touch(other);

            if (_wasTouched || other is not TerryBrosPlayer player)
            {
                return;
            }

            if (STBGame.CurrentLevel != null)
            {
                _wasTouched = true;

                STBGame.CurrentLevel.CheckPointReached(player, this);
            }
        }

        [Event(TBEvent.Level.RESTART)]
        public void ResetCheckPoint()
        {
            _wasTouched = false;
        }
    }
}

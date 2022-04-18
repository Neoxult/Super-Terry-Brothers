using Sandbox;

using TerryBros.Events;
using TerryBros.Gamemode;
using TerryBros.Utils;

namespace TerryBros.LevelElements
{
    public partial class Checkpoint : BlockEntity
    {
        public SpawnPoint SpawnPoint { get; private set; }
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
            SpawnPoint = new();

            base.Spawn();
        }

        public override void Touch(Entity other)
        {
            base.Touch(other);

            if (_wasTouched || other is not Player player)
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

using Sandbox;

using TerryBros.Events;
using TerryBros.Gamemode;

namespace TerryBros.LevelElements
{
    [Library("stb_checkpoint"), Hammer.Skip]
    public partial class Checkpoint : BlockEntity
    {
        public SpawnPoint SpawnPoint { get; private set; }

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

            AddCollisionLayer(CollisionLayer.All);
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

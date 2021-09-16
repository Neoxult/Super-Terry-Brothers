using Sandbox;

using TerryBros.Player;
using TerryBros.Settings;
using TerryBros.Levels;

namespace TerryBros.LevelElements
{
    public partial class Checkpoint : BlockEntity
    {
        public STBSpawn spawnPoint { get; private set; }
        public override string materialName => "materials/blocks/stair_block.vmat";
        public override intVector3 myBlockSize => new intVector3(1, 3, 1);
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

        private bool wasTouched;

        public Checkpoint() : this(globalSettings.GetBlockPosForGridCoordinates(0, 0)) { }
        public Checkpoint(Vector3 position) : base(position)
        {
            Transmit = TransmitType.Always;
            CollisionGroup = CollisionGroup.Trigger;
            EnableAllCollisions = true;
            EnableHitboxes = true;
            RenderColor = Color.Blue;

            spawnPoint = new STBSpawn(position + globalSettings.upwardDir * (globalSettings.figureHeight - globalSettings.blockSize / 2) / 2);
        }

        public void RegisterReset(ref resetCheckPoints resetDelegate)
        {
            resetDelegate += resetCheckPoint;
        }

        public override void Touch(Entity other)
        {
            base.Touch(other);
            if (wasTouched || other is not TerryBrosPlayer)
            {
                return;
            }
            if(Level.currentLevel != null)
            {
                wasTouched = true;
                Level.currentLevel.SetCheckPoint(this);
            }

        }
        private void resetCheckPoint()
        {
            wasTouched = false;
        }
    }
}

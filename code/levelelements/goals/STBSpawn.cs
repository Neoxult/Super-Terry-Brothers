using Sandbox;

using TerryBros.Settings;

namespace TerryBros.LevelElements
{
    public partial class STBSpawn : Entity
    {
        public STBSpawn() : base()
        {
        }
        /// <summary>
        /// Make sure the Player can spawn here properly.
        /// TODO: Check for exact figureHeight
        /// </summary>
        public override Vector3 Position {
            get => base.Position;
            set => base.Position = value + GlobalSettings.UpwardDir * (GlobalSettings.FigureHeight - GlobalSettings.BlockSize / 2) / 2;
        }
        public void MoveToSpawn(Entity pawn)
        {
            pawn.Transform = Transform;
        }
    }
}

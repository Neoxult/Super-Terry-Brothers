using Sandbox;

using TerryBros.Settings;

namespace TerryBros.LevelElements
{
    public partial class STBSpawn : Entity
    {
        public STBSpawn() : this(globalSettings.GetBlockPosForGridCoordinates(0, 4)) { }
        public STBSpawn(Vector3 position)
        {
            Transform = new Transform(position);
        }
        public void MoveToSpawn(Entity pawn)
        {
            pawn.Transform = Transform;
        }
    }
}

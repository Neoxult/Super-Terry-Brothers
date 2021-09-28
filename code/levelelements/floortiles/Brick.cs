using Sandbox;

using TerryBros.Utils;

namespace TerryBros.LevelElements
{
    public partial class Brick : BlockTextureEntity
    {
        public override string MaterialName => "materials/blocks/stair_block.vmat";
        public override IntVector3 BlockSize => new(1, 1, 1);
        public Brick() : base()
        {
        }
    }
}

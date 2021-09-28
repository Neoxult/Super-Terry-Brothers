using Sandbox;

using TerryBros.Utils;
using TerryBros.Settings;

namespace TerryBros.LevelElements
{
    public partial class BrickClayTop : BlockModelEntity
    {
        public override string MaterialName => "materials/blocks/brick_clay_top.vmat";
        public override string ModelName => "models/blocks/layered_block.vmdl";
        public override bool UseMaterial => true;
        public BrickClayTop() : base()
        {
        }
    }
}

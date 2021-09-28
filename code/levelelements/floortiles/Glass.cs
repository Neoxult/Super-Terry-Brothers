using Sandbox;

using TerryBros.Utils;
using TerryBros.Settings;

namespace TerryBros.LevelElements
{
    public partial class Glass : BlockModelEntity
    {
        public override string MaterialName => "materials/blocks/glass.vmat";
        public override string ModelName => "models/blocks/layered_block.vmdl";
        public override bool UseMaterial => true;
        public Glass() : base()
        {
        }
    }
}

namespace TerryBros.LevelElements
{
    public partial class GlassTiled : BlockModelEntity
    {
        public override string MaterialName => "materials/blocks/glass_tiled.vmat";
        public override string ModelName => "models/blocks/layered_block.vmdl";
        public override bool UseMaterial => true;
        public GlassTiled() : base()
        {
        }
    }
}

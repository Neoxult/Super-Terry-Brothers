namespace TerryBros.LevelElements
{
    public partial class SheetMetal : BlockModelEntity
    {
        public override string MaterialName => "materials/blocks/sheet_metal.vmat";
        public override string ModelName => "models/blocks/layered_block.vmdl";
        public override bool UseMaterial => true;
    }
}

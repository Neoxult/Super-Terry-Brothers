using Sandbox.UI;
using Sandbox.UI.Construct;

using TerryBros.LevelElements;

namespace TerryBros.UI.LevelBuilder
{
    public class Block : Panel
    {
        public Label TextLabel;

        public BlockData BlockData { get; set; }

        public Block(Panel parent = null, BlockData blockData = null) : base()
        {
            Parent = parent ?? Parent;

            TextLabel = Add.Label(blockData.Name, "name");
            BlockData = blockData;

            AddEventListener("onclick", (e) =>
            {
                Builder.Instance.Editor.Select(BlockData.Type);
            });
        }
    }
}

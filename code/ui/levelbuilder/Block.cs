using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TerryBros.LevelElements;

namespace TerryBros.UI.LevelBuilder
{
    public class Block : Panel
    {
        public Label TextLabel;
        public Image Image;

        public BlockData BlockData { get; set; }

        public Block(Panel parent = null, BlockData blockData = null) : base()
        {
            Parent = parent ?? Parent;

            BlockData = blockData;

            Image = Add.Image(blockData.MaterialName.Replace(".vmat", ".png"), "image");
            TextLabel = Add.Label(blockData.Name, "name");

            AddEventListener("onclick", (e) =>
            {
                BuildPanel.Instance.BlockSelection.Select(BlockData.Type);
            });
        }
    }
}

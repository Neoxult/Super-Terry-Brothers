using Sandbox.UI;
using Sandbox.UI.Construct;

using TerryBros.LevelElements;

namespace TerryBros.UI.LevelBuilder
{
    public class Block : Panel
    {
        public Label TextLabel;
        public Image Image;

        public BlockAsset Asset { get; set; }

        public Block(Panel parent = null, BlockAsset asset = null) : base(parent)
        {
            Asset = asset;

            Image = Add.Image(Asset.IconName, "image");
            TextLabel = Add.Label(Asset.Name, "name");

            AddEventListener("onclick", (e) =>
            {
                BuildPanel.Instance.BlockSelection.Select(Asset.Name);
            });
        }
    }
}

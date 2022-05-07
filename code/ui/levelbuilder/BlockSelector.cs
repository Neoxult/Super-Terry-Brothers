using System.Collections.Generic;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryBros.UI.LevelBuilder
{
    public class BlockSelector : Panel
    {
        public bool IsOpened
        {
            get => _isOpened;
            set
            {
                SetClass("opened", value);

                _isOpened = value;

                OpenLabel.Text = value ? "Close" : "Open";
            }
        }
        private bool _isOpened = false;

        public Label OpenLabel;

        public List<Block> BlockList = new();

        public BlockSelector(Panel parent) : base(parent)
        {
            StyleSheet.Load("/ui/levelbuilder/BlockSelector.scss");

            OpenLabel = Add.Label("Open", "openlabel");
            OpenLabel.AddEventListener("onclick", (e) =>
            {
                IsOpened = !IsOpened;
            });

            IsOpened = true;

            AddBlocksData(Add.Panel("blocks"));
        }

        private void AddBlocksData(Panel parent)
        {
            int count = 0;

            foreach (LevelElements.BlockAsset asset in LevelElements.BlockAsset.All)
            {
                count++;

                BlockList.Add(new(parent, asset));

                if (count == 1)
                {
                    Select(asset.Name);
                }
            }
        }

        public void Select(string name)
        {
            foreach (Block block in BlockList)
            {
                bool selected = block.Asset.Name == name;

                block.SetClass("selected", selected);

                if (selected)
                {
                    BuildPanel.Instance.SelectedAsset = block.Asset;
                }
            }
        }
    }
}

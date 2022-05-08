using System.Collections.Generic;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryBros.UI.LevelBuilder.Tools
{
    [UseTemplate]
    public class BlockSelector : Panel
    {
        public static BlockSelector Instance { get; set; }

        public Panel Header { get; set; }
        public Panel Wrapper { get; set; }
        public Panel Footer { get; set; }

        public Panel Blocks { get; set; }

        public bool IsOpened
        {
            get => _isOpened;
            set
            {
                SetClass("opened", value);

                _isOpened = value;
            }
        }
        private bool _isOpened = false;

        public List<Block> BlockList = new();

        public BlockSelector() : base()
        {
            Instance = this;

            AddBlocksData(Blocks);

            Footer.Add.Button("Close", "close", () => {
                IsOpened = !IsOpened;
            });
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

            IsOpened = false;
        }
    }
}

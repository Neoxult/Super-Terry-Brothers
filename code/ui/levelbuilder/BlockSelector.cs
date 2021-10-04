using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TerryBros.Gamemode;
using TerryBros.LevelElements;

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

        private Panel blocksParentPanel;

        public BlockSelector(Panel parent = null) : base()
        {
            Parent = parent ?? Parent;

            StyleSheet.Load("/ui/levelbuilder/BlockSelector.scss");

            OpenLabel = Add.Label("Open", "openlabel");
            OpenLabel.AddEventListener("onclick", (e) =>
            {
                IsOpened = !IsOpened;
            });

            IsOpened = true;

            //Shift Block-Creation to a late Initialization
            blocksParentPanel = Add.Panel("blocks");
            STBGame.AddLateInitializeAction(AddBlocksData);
        }
        
        private void AddBlocksData()
        {
            int count = 0;

            //Outside of a constructor we can create Entities
            STBGame.CreateBlockData();

            foreach (BlockData blockData in STBGame.BlockDataList)
            {
                count++;

                BlockList.Add(new Block(blocksParentPanel, blockData));

                if (count == 1)
                {
                    Select(blockData.Type);
                }
            }
        }

        public void Select(Type type)
        {
            foreach (Block block in BlockList)
            {
                bool selected = block.BlockData.Type == type;

                block.SetClass("selected", selected);

                if (selected)
                {
                    BuildPanel.Instance.SelectedBlockData = block.BlockData;
                }
            }
        }
    }
}

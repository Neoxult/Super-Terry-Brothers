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


            //Shift to LateInitialize. Outside of a constructor we can create Entities
            STBGame.AddLateInitializeAction(
                () => { AddBlocksData(Add.Panel("blocks"));
            });
        }
        
        private void AddBlocksData(Panel parent)
        {
            int count = 0;

            List<BlockData> blockDataList = CreateBlockData();

            foreach (BlockData blockData in blockDataList)
            {
                count++;

                BlockList.Add(new Block(parent, blockData));

                if (count == 1)
                {
                    Select(blockData.Type);
                }
            }
        }
        private List<BlockData> CreateBlockData()
        {
            List<BlockData> blockDataList = new();

            foreach (Type type in Library.GetAll<BlockEntity>())
            {
                if (!type.IsAbstract && !type.ContainsGenericParameters)
                {
                    BlockEntity blockEntity = Library.Create<BlockEntity>(type);
                    BlockData blockData = blockEntity.GetBlockData();

                    blockEntity.Delete();
                    blockDataList.Add(blockData);
                }
            }

            return blockDataList;
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

using System;
using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TerryBros.LevelElements;

namespace TerryBros.UI.LevelBuilder
{
    public class Editor : Panel
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

        public Editor(Panel parent = null) : base()
        {
            Parent = parent ?? Parent;

            StyleSheet.Load("/ui/levelbuilder/Editor.scss");

            OpenLabel = Add.Label("Open", "openlabel");
            OpenLabel.AddEventListener("onclick", (e) =>
            {
                IsOpened = !IsOpened;
            });

            //TODO: Initialize Blocks outside of constructor
            //AddBlocks(Add.Panel("blocks"));

            IsOpened = true;
        }

        private void AddBlocks(Panel parent)
        {
            int count = 0;

            foreach (Type type in Library.GetAll<BlockEntity>())
            {
                if (!type.IsAbstract && !type.ContainsGenericParameters)
                {
                    count++;

                    BlockEntity blockEntity = Library.Create<BlockEntity>(type);
                    BlockData blockData = blockEntity.GetBlockData();

                    blockEntity.Delete();
                    BlockList.Add(new Block(parent, blockData));

                    if (count == 1)
                    {
                        Select(type);
                    }
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
                    Builder.Instance.SelectedBlockData = block.BlockData;
                }
            }
        }
    }
}

using System;

using Sandbox.UI;

namespace TerryBros.UI.LevelBuilder.Tools
{
    public class BlockBuildTool : BuildTool
    {
        public override int Priority { get; set; } = 0;
        public override Action<MousePanelEvent> OnClickTool { get; set; } = OnClickBuildTool;

        public override string IconPath { get; set; } = "";

        public static BlockBuildTool Instance { get; set; }

        public static void OnClickBuildTool(MousePanelEvent e)
        {
            Instance.BlockSelector.IsOpened = !Instance.BlockSelector.IsOpened;
        }

        public BlockSelector BlockSelector { get; set; }

        public BlockBuildTool() : base()
        {
            Instance = this;

            BlockSelector = new();

            AddChild(BlockSelector);
        }
    }
}

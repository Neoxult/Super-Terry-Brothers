using System;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryBros.UI.LevelBuilder.Tools
{
    public class BlockSizeTool : BuildTool
    {
        public override int Priority { get; set; } = 0;
        public override Action<MousePanelEvent> OnClickTool { get; set; } = OnClickBuildTool;
        public override string IconPath { get; set; } = "";

        public static BlockSizeTool Instance { get; set; }

        public bool IsOpened
        {
            get => _isOpened;
            set
            {
                _isOpened = value;

                SetClass("opened", _isOpened);
            }
        }
        private bool _isOpened = false;

        public static void OnClickBuildTool(MousePanelEvent e)
        {
            Instance.IsOpened = !Instance.IsOpened;
        }

        public BlockSizeTool() : base()
        {
            Instance = this;

            Panel wrapper = Add.Panel("wrapper");
            wrapper.Add.Button("1x1");
            wrapper.Add.Button("2x2");
            wrapper.Add.Button("3x3");
        }
    }
}

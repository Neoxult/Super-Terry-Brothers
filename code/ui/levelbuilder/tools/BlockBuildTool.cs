using System;

using Sandbox.UI;

namespace TerryBros.UI.LevelBuilder.Tools
{
    public class BlockBuildTool : BuildTool
    {
        public override int Priority { get; set; } = 0;
        public override Action<MousePanelEvent> OnClickTool { get; set; } = OnClickBuildTool;

        public override string IconPath
        {
            get => BuildPanel.Instance?.SelectedAsset?.IconPath;
            set { }
        }
        public static BlockBuildTool Instance { get; set; }

        public static void OnClickBuildTool(MousePanelEvent e)
        {
            BlockSelector.Instance.IsOpened = !BlockSelector.Instance.IsOpened;
        }

        public BlockBuildTool() : base()
        {
            Instance = this;

            Hud.Instance.RootPanel.AddChild(new BlockSelector());
        }

        [Sandbox.Event.Hotload]
        public static void OnHotload()
        {
            BlockSelector.Instance?.Delete(true);

            Hud.Instance?.RootPanel.AddChild(new BlockSelector());
        }
    }
}

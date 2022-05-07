using System;

using Sandbox.UI;

namespace TerryBros.UI.LevelBuilder.Tools
{
    public class BlockBuildTool : BuildTool
    {
        public override int Priority { get; set; } = 0;
        public override Action<MousePanelEvent> OnClickTool { get; set; } = OnClickBuildTool;

        public override string IconPath { get; set; } = "";

        public static void OnClickBuildTool(MousePanelEvent e)
        {
            Log.Info("Pressed BlockBuildTool");
        }
    }
}

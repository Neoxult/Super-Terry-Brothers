using System;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryBros.UI.LevelBuilder.Tools
{
    public abstract class BuildTool : Button
    {
        public abstract int Priority { get; set; }

        public abstract Action<MousePanelEvent> OnClickTool { get; set; }

        public abstract string IconPath { get; set; }

        public BuildTool() : base()
        {
            AddClass("build-tool");

            Add.Image(IconPath, "icon");
        }

        protected override void OnClick(MousePanelEvent e)
        {
            base.OnClick(e);

            OnClickTool?.Invoke(e);
        }
    }
}

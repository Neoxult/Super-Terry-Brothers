using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryBros.UI.Menu
{
    public partial class MenuContent : Panel
    {
        public Panel OptionsPanel;

        public MenuContent(Panel parent = null) : base()
        {
            Parent = parent ?? Parent;

            Add.Label("Menu", "title");

            OptionsPanel = Add.Panel("options");

            OptionsPanel.Add.Label("Load Level");
            OptionsPanel.Add.Label("Settings");
            OptionsPanel.Add.Label("About");
        }
    }
}

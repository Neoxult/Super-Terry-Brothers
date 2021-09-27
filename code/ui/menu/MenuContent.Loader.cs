using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TerryBros.Player;

namespace TerryBros.UI.Menu
{
    public partial class MenuContent
    {
        public void ShowLevels(Panel wrapperPanel)
        {
            foreach (string level in TerryBrosPlayer.GetLevels())
            {
                wrapperPanel.Add.Button(level.Split('.')[0], "entry", () =>
                {
                    TerryBros.Player.TerryBrosPlayer.LoadLevel(level);

                    OnClickHome();

                    Menu.Instance.Display = false;
                });
            }
        }
    }
}

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryBros.UI.Menu
{
    public partial class MenuContent
    {
        public void ShowLevels(Panel wrapperPanel)
        {
            foreach (string level in Levels.Builder.Loader.GetLevels())
            {
                wrapperPanel.Add.Button(level.Split('.')[0], "entry", () =>
                {
                    Levels.Builder.Loader.LoadLevel(level);

                    OnClickHome();

                    Menu.Instance.Display = false;
                });
            }
        }
    }
}

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

        public void ShowLevelInput(Panel wrapperPanel)
        {
            TextEntry textEntry = wrapperPanel.Add.TextEntry("");
            textEntry.Focus();
            textEntry.Placeholder = "filename";

            wrapperPanel.Add.Button("Save", "entry", () =>
            {
                string text = textEntry.Text;

                if (string.IsNullOrWhiteSpace(text))
                {
                    return;
                }

                text = text.ToLower();

                bool found = false;

                foreach (string level in Levels.Builder.Loader.GetLevels())
                {
                    if (level.ToLower() == text)
                    {
                        found = true;

                        break;
                    }
                }

                if (!found)
                {
                    SaveLevel(text);

                    return;
                }

                // TODO popup to override the files
            });
        }

        private void SaveLevel(string text)
        {
            Levels.Builder.Loader.SaveLevel(text);

            OnClickHome();

            Menu.Instance.Display = false;
        }
    }
}

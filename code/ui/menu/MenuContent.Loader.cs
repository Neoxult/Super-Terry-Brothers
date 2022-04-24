using Sandbox.UI;
using Sandbox.UI.Construct;

using TerryBros.Gamemode;

namespace TerryBros.UI
{
    public partial class MenuContent
    {
        public void ShowLevels(Panel wrapperPanel)
        {
            foreach (string level in Levels.Loader.Local.Get())
            {
                Panel panel = wrapperPanel.Add.Panel("buttons");

                panel.Add.Button(level.Split('.')[0], "entry", () =>
                {
                    STBGame.ClearLevel();
                    STBGame.StartLevelEditor(level);

                    OnClickHome();

                    Menu.Instance.Display = false;
                });

                panel.Add.Button("X", "entry delete", () =>
                {
                    Levels.Loader.Local.Delete(level);

                    panel.Delete();
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

                foreach (string level in Levels.Loader.Local.Get())
                {
                    if (level.Split('.')[0].ToLower() == text)
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
            Levels.Loader.Local.Save(text);

            OnClickHome();

            Menu.Instance.Display = false;
        }
    }
}

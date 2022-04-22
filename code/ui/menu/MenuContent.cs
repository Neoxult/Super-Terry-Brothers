using System;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryBros.UI.Menu
{
    public partial class MenuContent : Panel
    {
        public Label TitleLabel { get; set; }
        public Panel WrapperPanel { get; set; }
        public string CurrentView { get; set; }

        public Menu Menu { get; set; }

        private readonly Button _backButton;

        public MenuContent(Menu menu) : base(menu)
        {
            Menu = menu;

            TitleLabel = Add.Label("Menu", "title");
            _backButton = Add.Button("keyboard_backspace", "back", () =>
            {
                OnClickBack(CurrentView);
            });

            WrapperPanel = Add.Panel("wrapper");

            OnClickHome();
        }

        public void SetContent(string title, Action<Panel> onSetContent = null, string view = null)
        {
            if (CurrentView != null)
            {
                SetClass(CurrentView, false);
            }

            if (view != null)
            {
                SetClass(view, true);
            }

            CurrentView = view;

            TitleLabel.Text = title;

            WrapperPanel.DeleteChildren(true);

            onSetContent?.Invoke(WrapperPanel);

            _backButton.SetClass("display", !view.Equals("home"));
        }

        public void ShowDefaultMenu(Panel wrapperPanel)
        {
            wrapperPanel.Add.Button("Load Level", "entry", () =>
            {
                SetContent("Load Level", ShowLevels, "levels");
            });

            wrapperPanel.Add.Button("Level Editor / Test", "entry", () =>
            {
                Levels.Builder.Editor.ClientToggleLevelEditor();

                Menu.Display = false;
            });

            wrapperPanel.Add.Button("Save", "entry", () =>
            {
                SetContent("Save Level", ShowLevelInput, "saving");
            });

            wrapperPanel.Add.Button("Quit", "entry", () =>
            {
                if (Sandbox.Local.Client.Pawn is Player player && player.IsInLevelBuilder)
                {
                    Levels.Builder.Editor.ClientToggleLevelEditor();
                }

                Menu.Display = false;
                StartScreen.StartScreen.Instance.Display = true;
            });
        }

        public void OnClickBack(string currentView)
        {
            if (currentView.Equals("home"))
            {
                return;
            }

            OnClickHome();
        }

        public void OnClickHome()
        {
            SetContent("Menu", ShowDefaultMenu, "home");
        }
    }
}

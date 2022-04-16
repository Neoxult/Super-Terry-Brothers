using System;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryBros.UI.Menu
{
    public partial class MenuContent : Panel
    {
        public Label TitleLabel;
        public Panel WrapperPanel;
        public string CurrentView;

        private readonly Button _backButton;

        public MenuContent(Panel parent = null) : base()
        {
            Parent = parent ?? Parent;

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
                SetContent("Load level", ShowLevels, "levels");
            });
            wrapperPanel.Add.Button("Level Editor", "entry", () => Levels.Builder.Editor.ClientToggleLevelEditor());

            wrapperPanel.Add.Button("Settings", "entry disabled");
            wrapperPanel.Add.Button("About", "entry disabled");
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

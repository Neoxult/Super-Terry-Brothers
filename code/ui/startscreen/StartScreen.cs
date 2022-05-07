using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TerryBros.Gamemode;

namespace TerryBros.UI
{
    [UseTemplate]
    public partial class StartScreen : Panel
    {
        public static StartScreen Instance { get; set; }

        public bool Display
        {
            get => _display;
            set
            {
                _display = value;

                SetClass("display", _display);

                if (Local.Pawn is Player player)
                {
                    player.IsInMenu = _display;

                    Levels.Editor.ServerToggleMenu(_display);
                }

                if (!_display)
                {
                    OnClickHome();
                }
            }
        }
        private bool _display = false;


        public string Title { get; set; }
        public Panel Wrapper { get; set; }
        public string CurrentView { get; set; }
        public Button BackButton { get; set; }
        public Image Logo { get; set; }

        public PlayerList PlayerList { get; set; }

        public StartScreen() : base()
        {
            Instance = this;
            Title = "Menu";
            BackButton.Text = "Back";
            Logo.Texture = Texture.Load(FileSystem.Mounted, "assets/logo.png", false);

            BackButton.AddEventListener("onclick", () =>
            {
                OnClickBack(CurrentView);
            });

            OnClickHome();

            Display = true;
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
            Title = title;

            Wrapper.DeleteChildren(true);
            onSetContent?.Invoke(Wrapper);

            BackButton.SetClass("display", !view.Equals("home"));
        }

        public void ShowDefaultMenu(Panel wrapperPanel)
        {
            wrapperPanel.SetClass("scroll", false);

            Button startButton = wrapperPanel.Add.Button("Start Game", "entry", () =>
            {
                if (!Local.Client?.HasPermission("startgame") ?? true)
                {
                    return;
                }

                SetContent("Choose Level", ShowLevels, "levels");
            });

            if (!Local.Client?.HasPermission("startgame") ?? false)
            {
                startButton.AddClass("disabled");
            }

            Button leveleditorButton = wrapperPanel.Add.Button("Level Editor", "entry", () =>
            {
                if (!Local.Client?.HasPermission("startleveleditor") ?? true)
                {
                    return;
                }

                STBGame.StartLevelEditor();

                Display = false;
            });

            if (!Local.Client?.HasPermission("startleveleditor") ?? false)
            {
                leveleditorButton.AddClass("disabled");
            }

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
            SetContent("Main Menu", ShowDefaultMenu, "home");
        }

        public void ShowLevels(Panel wrapperPanel)
        {
            wrapperPanel.SetClass("scroll", true);

            foreach (string level in Levels.Loader.Local.Get())
            {
                Panel panel = wrapperPanel.Add.Panel("buttons");

                panel.Add.Button(level.Split('.')[0], "entry", () =>
                {
                    STBGame.StartGame(level);

                    OnClickHome();

                    Display = false;
                });

                panel.Add.Button("X", "entry delete", () =>
                {
                    Levels.Loader.Local.Delete(level);

                    panel.Delete();
                });
            }
        }
    }
}

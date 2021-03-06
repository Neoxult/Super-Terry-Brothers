using Sandbox;
using Sandbox.UI;

namespace TerryBros.UI
{
    public partial class Menu : Panel
    {
        public static Menu Instance { get; set; }
        public MenuContent MenuContent { get; set; }

        private bool _pressed = false;

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
                    MenuContent.OnClickHome();
                }
            }
        }
        private bool _display = false;

        public Menu() : base()
        {
            if (Instance != null && !Instance.IsDeleting)
            {
                Instance.Delete(true);
            }

            Instance = this;

            StyleSheet.Load("/ui/menu/Menu.scss");

            MenuContent = new(this);

            Display = false;
        }

        public override void Tick()
        {
            base.Tick();

            // own pressing detection, as Input.Pressed is unreliable
            if (Input.Down(InputButton.Menu))
            {
                if (!_pressed && !StartScreen.Instance.Display)
                {
                    _pressed = true;

                    Display = !Display;
                }
            }
            else
            {
                _pressed = false;
            }
        }
    }
}

using Sandbox;
using Sandbox.UI;

namespace TerryBros.UI.Menu
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

                    Levels.Builder.Editor.ServerToggleMenu(_display);
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
                if (!_pressed)
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

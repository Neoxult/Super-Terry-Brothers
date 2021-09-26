using Sandbox;
using Sandbox.UI;

using TerryBros.Player;

namespace TerryBros.UI.Menu
{
    public partial class Menu : Panel
    {
        public static Menu Instance;
        public MenuContent MenuContent;

        private bool _pressed = false;

        public bool Display
        {
            get => _display;
            set
            {
                _display = value;

                SetClass("display", _display);

                if (Local.Pawn is TerryBrosPlayer player)
                {
                    player.IsInMenu = _display;

                    TerryBrosPlayer.ServerToggleMenu(_display);
                }
            }
        }
        private bool _display = false;

        public Menu() : base()
        {
            Instance = this;

            StyleSheet.Load("/ui/menu/Menu.scss");

            MenuContent = new(this);

            Display = true;
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

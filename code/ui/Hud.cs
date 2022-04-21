using Sandbox;
using Sandbox.UI;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace TerryBros.UI
{
    /// <summary>
    /// This is the HUD entity. It creates a RootPanel clientside, which can be accessed
    /// via RootPanel on this entity, or Local.Hud.
    /// </summary>
    public partial class Hud : HudEntity<RootPanel>
    {
        public static Hud Instance { get; set; }

        public Hud()
        {
            Instance = this;

            RootPanel.AddChild<StartScreen.StartScreen>();
            RootPanel.AddChild<LevelBuilder.BuildPanel>();
            RootPanel.AddChild<Menu.Menu>();
        }

        [Event.Hotload]
        public static void OnHotReloaded()
        {
            if (!Host.IsClient)
            {
                return;
            }

            Local.Hud?.Delete();

            _ = new Hud();
        }
    }
}

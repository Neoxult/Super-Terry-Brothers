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
    public partial class Hud : Sandbox.HudEntity<RootPanel>
    {
        public static Hud Instance { get; set; }

        public Hud()
        {
            if (!IsClient)
            {
                return;
            }

            Instance = this;

            RootPanel.AddChild<LevelBuilder.Builder>();
            RootPanel.AddChild<LevelLoader.Loader>();
        }

        [Event.Hotload]
        public static void OnHotReloaded()
        {
            if (Host.IsClient)
            {
                Local.Hud?.Delete();

                Hud hud = new();
            }
        }
    }
}

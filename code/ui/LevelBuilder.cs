using Sandbox.UI;

namespace TerryBros.UI
{
    public class LevelBuilder : Panel
    {
        public static LevelBuilder Instance;

        public LevelBuilder() : base()
        {
            Instance = this;

            StyleSheet.Load("/ui/LevelBuilder.scss");
        }

        public void Toggle(bool toggle)
        {
            Style.Display = toggle ? DisplayMode.Flex : DisplayMode.None;
            Style.Dirty();
        }
    }
}

using System.Collections.Generic;

using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryBros.UI.LevelLoader
{
    public class Loader : Panel
    {
        public static Loader Instance;

        public List<string> LevelList = new();
        public List<Label> LevelLabelList = new();

        public bool Display
        {
            get => _display;
            set
            {
                _display = value;

                Style.Display = _display ? DisplayMode.Flex : DisplayMode.None;
                Style.Dirty();
            }
        }
        private bool _display = false;

        public Loader() : base()
        {
            Instance = this;

            StyleSheet.Load("/ui/levelloader/Loader.scss");

            Display = false;
        }

        public void SetLevels(List<string> levelList)
        {
            LevelList = levelList;

            foreach (Label label in LevelLabelList)
            {
                label.Delete(true);
            }

            LevelLabelList.Clear();

            foreach (string level in levelList)
            {
                Label label = Add.Label(level, "level");

                label.AddEventListener("onclick", (e) =>
                {
                    TerryBros.Player.TerryBrosPlayer.LoadLevel(level);

                    Display = false;
                });

                LevelLabelList.Add(label);
            }
        }
    }
}

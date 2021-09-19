using Sandbox.UI;

namespace TerryBros.UI.LevelBuilder
{
    public class Builder : Panel
    {
        public static Builder Instance;
        public bool IsMouseDown { get; private set; } = false;
        public bool IsLeftMouseButtonDown { get; private set; } = false;
        public bool IsRightMouseButtonDown { get; private set; } = false;

        public Editor LevelEditor;

        public Builder() : base()
        {
            Instance = this;

            StyleSheet.Load("/ui/levelbuider/Builder.scss");

            LevelEditor = new Editor(this);

            Toggle(false);
        }

        public void Toggle(bool toggle)
        {
            Style.Display = toggle ? DisplayMode.Flex : DisplayMode.None;
            Style.Dirty();

            if (!toggle)
            {
                IsMouseDown = false;
                IsLeftMouseButtonDown = false;
                IsRightMouseButtonDown = false;
            }
        }

        protected override void OnMouseDown(MousePanelEvent e)
        {
            IsMouseDown = true;

            if (e.Button.Equals("mouseleft"))
            {
                IsLeftMouseButtonDown = true;
            }
            else if (e.Button.Equals("mouseright"))
            {
                IsRightMouseButtonDown = true;
            }
        }

        protected override void OnMouseUp(MousePanelEvent e)
        {
            IsMouseDown = false;
            IsLeftMouseButtonDown = false;
            IsRightMouseButtonDown = false;
        }
    }
}

using Sandbox.UI;

using TerryBros.LevelElements;

namespace TerryBros.UI.LevelBuilder
{
    public class Builder : Panel
    {
        public static Builder Instance;
        public bool IsMouseDown { get; private set; } = false;
        public bool IsLeftMouseButtonDown { get; private set; } = false;
        public bool IsRightMouseButtonDown { get; private set; } = false;
        public BlockData SelectedBlockData;

        public Editor Editor;

        public Builder() : base()
        {
            Instance = this;

            StyleSheet.Load("/ui/levelbuider/Builder.scss");

            Editor = new Editor(this);

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
            base.OnMouseDown(e);

            if (e.Target is not Builder)
            {
                return;
            }

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
            base.OnMouseUp(e);

            IsMouseDown = false;
            IsLeftMouseButtonDown = false;
            IsRightMouseButtonDown = false;
        }
    }
}

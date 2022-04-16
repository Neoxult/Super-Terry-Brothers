using Sandbox.UI;

using TerryBros.LevelElements;

namespace TerryBros.UI.LevelBuilder
{
    public class BuildPanel : Panel
    {
        public static BuildPanel Instance { get; set; }
        public bool IsMouseDown { get; private set; } = false;
        public bool IsLeftMouseButtonDown { get; private set; } = false;
        public bool IsRightMouseButtonDown { get; private set; } = false;
        public float MouseWheel { get; private set; } = 0f;
        private float _mouseWheel = 0f;
        public BlockData SelectedBlockData;

        public BlockSelector BlockSelection;

        public BuildPanel() : base()
        {
            Instance = this;

            StyleSheet.Load("/ui/levelbuilder/BuildPanel.scss");

            BlockSelection = new BlockSelector(this);

            Toggle(false);
        }

        public void Toggle(bool toggle)
        {
            Style.Display = toggle ? DisplayMode.Flex : DisplayMode.None;
            Style.Dirty();

            SetClass("display", toggle);

            if (!toggle)
            {
                IsMouseDown = false;
                IsLeftMouseButtonDown = false;
                IsRightMouseButtonDown = false;
            }
        }
        public override void Tick()
        {
            base.Tick();

            MouseWheel = _mouseWheel;
            _mouseWheel = 0f;
        }

        //TODO: Stop Scrolling when over BuildPanel for later use of scroll in blockList
        /*protected override void OnMouseOver(MousePanelEvent e)
        {
            base.OnMouseOver(e);

            if (e.Target is not BuildPanel) return;

            MouseWheel = 0f;
            _mouseWheel = 0f;
        }*/

        public override void OnMouseWheel(float value)
        {
            base.OnMouseWheel(value);

            _mouseWheel = value;
        }
        protected override void OnMouseDown(MousePanelEvent e)
        {
            base.OnMouseDown(e);

            if (e.Target is not BuildPanel)
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

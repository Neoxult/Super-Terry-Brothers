using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryBros.UI.LevelBuilder
{
    public class Editor : Panel
    {
        public bool IsOpened
        {
            get => _isOpened;
            set
            {
                SetClass("opened", value);

                _isOpened = value;

                OpenLabel.Text = value ? "Close" : "Open";
            }
        }
        private bool _isOpened = false;

        public Label OpenLabel;

        public Editor(Panel parent = null) : base()
        {
            Parent = parent ?? Parent;

            StyleSheet.Load("/ui/levelbuider/Editor.scss");

            OpenLabel = Add.Label("Open", "openlabel");
            OpenLabel.AddEventListener("onclick", (e) =>
            {
                IsOpened = !IsOpened;
            });

            AddBlocks(Add.Panel("blocks"));

            IsOpened = true;
        }

        private void AddBlocks(Panel parent)
        {
            Block block = new Block(parent);
            block.TextLabel.Text = "Brick";
            block.SetClass("selected", true);
        }
    }
}

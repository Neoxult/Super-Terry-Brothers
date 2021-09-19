using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryBros.UI.LevelBuilder
{
    public class Block : Panel
    {
        public Label TextLabel;

        public Block(Panel parent = null) : base()
        {
            Parent = parent ?? Parent;

            TextLabel = Add.Label("", "name");
        }
    }
}

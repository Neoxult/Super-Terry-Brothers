using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryBros.UI.LevelBuilder.Tools
{
    public abstract class BuildTool : Button
    {
        public abstract int Priority { get; set; }

        public abstract Action<MousePanelEvent> OnClickTool { get; set; }

        public abstract string IconPath { get; set; }

        private string _iconPath = null;
        private readonly Image _image;

        public BuildTool() : base()
        {
            AddClass("build-tool");

            _image = Add.Image(IconPath, "icon");
            _iconPath = IconPath;
        }

        protected override void OnClick(MousePanelEvent e)
        {
            base.OnClick(e);

            OnClickTool?.Invoke(e);
        }

        public override void Tick()
        {
            base.Tick();

            if (IconPath != _iconPath)
            {
                _iconPath = IconPath;

                if (IconPath != null)
                {
                    _image.Texture = Texture.Load(IconPath, false);
                }
            }
        }
    }
}

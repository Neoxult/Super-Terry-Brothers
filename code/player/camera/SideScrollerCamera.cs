using Sandbox;

using TerryBros.Settings;

namespace TerryBros.Player.Camera
{
    public partial class SideScrollerCamera : Sandbox.Camera
    {
        private float distance = 1500;
        private float viewHeightScale = 0.3f;
        private float orthoSize = 0.3f;

        public override void Update()
        {
            var player = Local.Pawn as TerryBrosPlayer;

            if (player == null)
            {
                return;
            }

            Pos = player.Position;
            Pos += Vector3.Up * Screen.Height * orthoSize * viewHeightScale;
            Pos -= globalSettings.lookDir * distance;

            Rot = Rotation.LookAt(globalSettings.lookDir, globalSettings.upwardDir);

            Ortho = true;
            OrthoSize = orthoSize;

            Viewer = null;
        }
    }
}

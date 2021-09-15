using Sandbox;

using TerryBros.Settings;

namespace TerryBros.Player.Camera
{
    public partial class SideScrollerCamera : Sandbox.Camera
    {
        private float distance = 1500;
        private float orthoSize = 0.3f;
        private int visibleGroundBlocks = 3;

        public override void Update()
        {
            var player = Local.Pawn as TerryBrosPlayer;

            if (player == null)
            {
                return;
            }

            Pos = new Vector3(player.Position.x, globalSettings.groundPos.y, globalSettings.groundPos.z);
            Pos -= globalSettings.upwardDir * globalSettings.blockSize * visibleGroundBlocks;
            Pos += globalSettings.upwardDir * Screen.Height / 2 * orthoSize;
            Pos -= globalSettings.lookDir * distance;

            Rot = Rotation.LookAt(globalSettings.lookDir, globalSettings.upwardDir);

            Ortho = true;
            OrthoSize = orthoSize;

            Viewer = null;
        }
    }
}

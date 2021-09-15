using Sandbox;

using TerryBros.Settings;

namespace TerryBros.Player.Camera
{
    public partial class SideScrollerCamera : Sandbox.Camera
    {
        private float distance = 1500;
        private float viewHeightScale = 0.3f;
        private float orthoSize = 0.3f;
        private int visibleGroundBlocks = 4;

        public override void Update()
        {
            var player = Local.Pawn as TerryBrosPlayer;

            if (player == null)
            {
                return;
            }

            orthoSize = 0.3f;
            visibleGroundBlocks = 3;
            Pos = globalSettings.groundPos;
            Pos -= globalSettings.upwardDir * globalSettings.blockSize * visibleGroundBlocks;
            Pos += globalSettings.upwardDir * Screen.Height / 2 * orthoSize;
            Pos -= globalSettings.lookDir * distance;
            Pos = new Vector3(player.Position.x, Pos.y, Pos.z);

            Rot = Rotation.LookAt(globalSettings.lookDir, globalSettings.upwardDir);

            Ortho = true;
            OrthoSize = orthoSize;

            Viewer = null;
        }
    }
}

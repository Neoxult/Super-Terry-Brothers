using Sandbox;

using TerryBros.Settings;

namespace TerryBros.Player.Camera
{
    public partial class SideScrollerCamera : Sandbox.Camera
    {
        private int distanceInBlocks = 10;
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
            Pos -= globalSettings.lookDir * globalSettings.blockSize * distanceInBlocks;

            if (Pos.x < globalSettings.worldBounds.Mins.x + Screen.Width/2 * orthoSize)
            {
                Pos = new Vector3(globalSettings.worldBounds.Mins.x + Screen.Width / 2 * orthoSize, Pos.y, Pos.z);
            } else if (Pos.x > globalSettings.worldBounds.Maxs.x - Screen.Width / 2 * orthoSize)
            {
                Pos = new Vector3(globalSettings.worldBounds.Maxs.x - Screen.Width / 2 * orthoSize, Pos.y, Pos.z);
            }

            Rot = Rotation.LookAt(globalSettings.lookDir, globalSettings.upwardDir);

            //TODO: Experiment with other perspectives
            //Rot = Rot.RotateAroundAxis(globalSettings.upwardDir, -30);

            //TODO: Check weird behaviour of RotateAroundAxis
            //Why is a rotation around the lookDir a rotation around the forwardDir?
            //Rot = Rot.RotateAroundAxis(globalSettings.lookDir, 30);

            Ortho = true;
            OrthoSize = orthoSize;

            Viewer = null;
        }
    }
}

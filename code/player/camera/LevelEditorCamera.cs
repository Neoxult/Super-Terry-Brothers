using System;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.Settings;
using TerryBros.Utils;
using TerryBros.Levels.Builder;
using TerryBros.UI.LevelBuilder;

namespace TerryBros.Player.Camera
{
    public partial class LevelEditorCamera : Sandbox.Camera
    {
        public float FreeCameraSpeed = 500f;
        public Vector3 LocalPosition
        {
            get => GlobalSettings.ConvertGlobalToLocalCoordinates(Pos);
            set { Pos = GlobalSettings.ConvertLocalToGlobalCoordinates(value); }
        }

        private int DistanceInBlocks = 10;
        private float _orthoSize = 0.3f;
        private float _zoomDistanceSteps = 0.01f;

        public LevelEditorCamera()
        {
            LocalPosition = new Vector3(0, 0, -GlobalSettings.BlockSize * DistanceInBlocks);
            Rot = Rotation.LookAt(GlobalSettings.LookDir, GlobalSettings.UpwardDir);

            Ortho = true;
            OrthoSize = _orthoSize;

            Viewer = null;
        }

        public override void Update()
        {
            Vector3 newPos = LocalPosition;

            newPos.x -= Input.Left * FreeCameraSpeed * Time.Delta;
            newPos.y += Input.Forward * FreeCameraSpeed * Time.Delta;

            LocalPosition = newPos;
            
            OrthoSize = Math.Clamp(OrthoSize + BuildPanel.Instance.MouseWheel * _zoomDistanceSteps, 0.1f, 2f);
        }
    }
}

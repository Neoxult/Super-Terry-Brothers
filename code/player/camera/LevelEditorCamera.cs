using System;

using Sandbox;

using TerryBros.Settings;
using TerryBros.UI.LevelBuilder;

namespace TerryBros
{
    public partial class LevelEditorCamera : CameraMode
    {
        public float FreeCameraSpeed = 500f;
        public Vector3 LocalPosition
        {
            get => GlobalSettings.ConvertGlobalToLocalCoordinates(Position);
            set
            {
                Position = GlobalSettings.ConvertLocalToGlobalCoordinates(value);
            }
        }

        private readonly int DistanceInBlocks = 10;
        private readonly float _orthoSize = 0.3f;
        private readonly float _zoomDistanceSteps = 0.01f;

        public LevelEditorCamera()
        {
            LocalPosition = new Vector3(0, 0, -GlobalSettings.BlockSize * DistanceInBlocks);
            Rotation = Rotation.LookAt(GlobalSettings.LookDir, GlobalSettings.UpwardDir);

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

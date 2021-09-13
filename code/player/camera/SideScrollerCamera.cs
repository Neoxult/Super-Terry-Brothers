using Sandbox;

namespace TerryBros.Player.Camera
{
	public partial class SideScrollerCamera : Sandbox.Camera
	{
		private float distance = 1500;
		private float viewHeightScale = 0.2f;
		private float orthoSize = 0.3f;

		//TODO: Make that globally available
		private Vector3 lookDirection = Vector3.Right;

		public override void Update()
		{
			var player = Local.Pawn as TerryBrosPlayer;

			if ( player == null )
				return;

			Pos = player.Position;
			Pos += Vector3.Up * Screen.Height * orthoSize * viewHeightScale;
			Pos -= player.viewDirection * distance;

			Rot = Rotation.LookAt( player.viewDirection, Vector3.Up );

			Ortho = true;
			OrthoSize = orthoSize;

			Viewer = null;
		}
	}
}

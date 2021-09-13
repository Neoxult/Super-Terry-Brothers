using Sandbox;

namespace TerryBros.Player.Controller
{
	public partial class MovementController : WalkController
	{
		public override void Simulate()
		{
			if ( Pawn is not TerryBrosPlayer player )
				return;

			//TODO: Do a proper implementation of the controller
			Input.Rotation = Rotation.LookAt( player.moveDirection, Vector3.Up );

			//TODO: Correct for current moveDirection
			Input.Forward = -1f * Input.Left;
			Input.Left = 0f;
			base.Simulate();
		}
	}
}

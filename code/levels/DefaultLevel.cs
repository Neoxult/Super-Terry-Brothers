using Sandbox;

namespace TerryBros.Levels
{
	partial class DefaultLevel : Level
	{
		private Vector3 groundPos;
		private Vector3 forward;
		private Vector3 up;

		public DefaultLevel( Vector3 groundPos, Vector3 forward, Vector3 up )
		{
			this.groundPos = groundPos;
			this.forward = forward;
			this.up = up;

			createFloor();
		}
		private ModelEntity createBox( Vector3 pos, int boxOffsetX, int boxOffsetY)
		{
			ModelEntity box = new ModelEntity
			{
				Rotation = Rotation.LookAt( forward, up )
			};

			box.SetModel( "models/citizen_props/crate01.vmdl" );
			box.SetupPhysicsFromModel( PhysicsMotionType.Static );
			box.Position = pos + boxOffsetX * forward * box.CollisionBounds.Size + boxOffsetX * up * box.CollisionBounds.Size;//  groundPos + i * forward * box.CollisionBounds.Size - j * up * box.CollisionBounds.Size;

			return box;
		}
		private void createFloor()
		{
			
			for (int i=0; i<100; i++ )
			{
				for (int j=0; j>-4; j-- )
				{
					createBox(groundPos, i ,j);
				}
			}

		}
	}
}

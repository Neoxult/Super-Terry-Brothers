using Sandbox;
using System;

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

			CreateFloor();
			CreateStair( 5, 1, 3, true );
			CreateStair( 8, 1, 3, false );
		}
		private ModelEntity CreateBox(int GridX, int GridY)
		{
			ModelEntity box = new ModelEntity
			{
				Rotation = Rotation.LookAt( forward, up )
			};

			box.SetModel( "models/citizen_props/crate01.vmdl" );
			box.SetupPhysicsFromModel( PhysicsMotionType.Static );
			box.Position = groundPos + GridX * forward * box.CollisionBounds.Size + GridY * up * box.CollisionBounds.Size;//  groundPos + i * forward * box.CollisionBounds.Size - j * up * box.CollisionBounds.Size;

			return box;
		}
		private void CreateStair(int GridX, int GridY, int height, bool upward = true)
		{
			int direction = upward ? 1 : -1;

			for ( int i = 0; i < height; i++  )
			{
				int x = GridX + i;

				int maxHeight = upward ? i + 1 : height - i;
				for ( int j = 0; j < maxHeight; j++ )
				{
					int y = GridY + j;
					CreateBox( x, y );
				}
			}
		}
		private void CreateFloor()
		{
			
			for (int i=-100; i<100; i++ )
			{
				for (int j=0; j>-4; j-- )
				{
					CreateBox(i, j);
				}
			}

		}
	}
}

using Sandbox;
using System;

namespace TerryBros.Player
{
	using Camera;
	using Controller;

	partial class TerryBrosPlayer : Sandbox.Player
	{
		/// <summary>
		/// The clothing container is what dresses the citizen
		/// </summary>
		public Clothing.Container Clothing = new();

		public Vector3 moveDirection { get; private set; }
		public Vector3 viewDirection { get; private set; }

		public TerryBrosPlayer()
		{
			//TODO: Use Forward and left for easier understanding
			//NOTE: Currently set for Construct map
			moveDirection = Vector3.Backward;
			viewDirection = Vector3.Right;
		}

		public TerryBrosPlayer( Client cl) : this()
		{
			Clothing.LoadFromClient( cl );
		}
		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			Controller = new MovementController();

			//
			// Use StandardPlayerAnimator  (you can make your own PlayerAnimator for 100% control)
			//
			Animator = new StandardPlayerAnimator();

			Camera = new SideScrollerCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			Clothing.DressEntity( this );

			base.Respawn();
		}

		/// <summary>
		/// Called every tick, clientside and serverside.
		/// </summary>
		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			//
			// If you have active children (like a weapon etc) you should call this to 
			// simulate those too.
			//
			SimulateActiveChild( cl, ActiveChild );
		}

		public override void OnKilled()
		{
			base.OnKilled();
		}
	}
}

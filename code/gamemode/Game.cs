using System;
using System.Collections.Generic;

using Sandbox;

using TerryBros.Player;

namespace TerryBros.Gamemode
{
    [Library("STB", Title = "Super Terry Brothers")]
    public partial class STBGame : Sandbox.Game
    {
        private static Queue<Action> _lateInitializers = new();
        public STBGame()
        {
        }
        public override void Simulate(Client cl)
        {
            base.Simulate(cl);

            SimulateRules(cl);
            SimulateDebug(cl);
        }
        public override void ClientJoined(Client client)
        {
            base.ClientJoined(client);

            TerryBrosPlayer player = new(client);
            client.Pawn = player;

            player.Respawn();
        }
        public static void AddLateInitializeAction(Action action)
        {
            _lateInitializers.Enqueue(action);
        }

        //TODO: Choose better event to have sooner Initialization,
        // i.e. you can see the blocks changing material.
        // Best would be an event that is called just before a new frame is drawn.
        [Event.Physics.PostStep]
        private void DoLateInitializations()
        {
            for (int i = 0; i < _lateInitializers.Count; i++)
            {
                _lateInitializers.Dequeue().Invoke();
            }
        }
    }
}

using System;
using System.Collections.Generic;

using Sandbox;

using TerryBros.Player;

#pragma warning disable IDE0051

namespace TerryBros.Gamemode
{
    public partial class STBGame : Game
    {
        private static Queue<Action> LateInitializers { get; set; } = new();

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
            LateInitializers.Enqueue(action);
        }

        //TODO: Choose better event to have sooner Initialization,
        // i.e. you can see the blocks changing material.
        // Best would be an event that is called just before a new frame is drawn.
        [Event.Physics.PostStep]
        private static void DoLateInitializations()
        {
            for (int i = 0; i < LateInitializers.Count; i++)
            {
                LateInitializers.Dequeue().Invoke();
            }
        }
    }
}

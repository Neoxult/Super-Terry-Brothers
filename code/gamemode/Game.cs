﻿
using Sandbox;

using TerryBros.Player;
using TerryBros.UI;
using TerryBros.Levels;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace TerryBros.Gamemode
{
    [Library("STB", Title = "Super Terry Brothers")]
    public partial class Game : Sandbox.Game
    {
        private Level currentLevel = null;
        public Game()
        {
            if (IsServer)
            {
                new Hud();
            }
        }

        public override void ClientJoined(Client client)
        {
            base.ClientJoined(client);

            var player = new TerryBrosPlayer(client);
            client.Pawn = player;

            //TODO: Change to proper Level Creation and Spawn
            //Create a Level underneath the spawns
            player.Respawn();
            player.Position += Vector3.Up * 200f;

            if (currentLevel != null)
            {
                return;
            }

            if (player.Controller is not WalkController controller)
            {
                return;
            }

            currentLevel = new DefaultLevel(player.Position - Vector3.Up * 100f, player.moveDirection, Vector3.Up);
        }
    }
}

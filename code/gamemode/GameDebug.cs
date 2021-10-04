using Sandbox;

using TerryBros.LevelElements;
using TerryBros.Player;
using TerryBros.Player.Controller;
using TerryBros.Utils;

namespace TerryBros.Gamemode
{
    public partial class STBGame
    {
        private bool ShouldDebug = true;

        private bool DebugSpawnPoint = false;
        private bool DebugPlayerCollisionBox = true;

        private void SimulateDebug(Client cl)
        {
            if (!ShouldDebug)
            {
                return;
            }

            if (Host.IsServer)
            {
                TerryBrosPlayer player = cl.Pawn as TerryBrosPlayer;

                if (DebugPlayerCollisionBox && player != null && player.Controller is MovementController controller)
                {
                    ShowPlayerCollisionBox(controller.GetBounds() + player.Position);
                }

                if (DebugSpawnPoint)
                {
                    STBSpawn spawnpoint = CurrentLevel?.GetLastCheckPoint();

                    if (spawnpoint != null)
                    {
                        ShowSpawnPointPosition(spawnpoint.Position);
                    }
                }
            }

        }

        [ClientRpc]
        private void ShowPlayerCollisionBox(BBox bBox)
        {
            DebugOverlay.Box(bBox.Mins, bBox.Maxs, Color.Black);
        }

        [ClientRpc]
        private void ShowSpawnPointPosition(Vector3 position)
        {
            float boxExtents = 5;
            Vector3 spawnMins = new Vector3(position);
            spawnMins.x -= boxExtents;
            spawnMins.y -= boxExtents;
            spawnMins.z -= boxExtents;

            boxExtents *= 2;
            DebugOverlay.Box(spawnMins, spawnMins + new Vector3(boxExtents, boxExtents, boxExtents), Color.Green);
        }

    }

}

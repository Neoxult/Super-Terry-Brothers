using Sandbox;

#pragma warning disable CA1822

namespace TerryBros.Gamemode
{
    public partial class STBGame
    {
        //TODO: Make this toggleable via console or similar
        private readonly bool ShouldDebug = true;

        private readonly bool DebugSpawnPoint = false;
        private readonly bool DebugPlayerCollisionBox = true;

        private void SimulateDebug(Client cl)
        {
            if (!ShouldDebug)
            {
                return;
            }

            if (Host.IsServer && cl.Pawn is Player player)
            {
                if (DebugPlayerCollisionBox && player.Controller is MovementController controller)
                {
                    ShowPlayerCollisionBox(controller.GetBounds() + player.Position);
                }

                if (DebugSpawnPoint)
                {
                    LevelElements.SpawnPoint spawnpoint = CurrentLevel?.GetLastCheckPoint(player);

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
            Vector3 spawnMins = new(position);
            spawnMins.x -= boxExtents;
            spawnMins.y -= boxExtents;
            spawnMins.z -= boxExtents;

            boxExtents *= 2;
            DebugOverlay.Box(spawnMins, spawnMins + new Vector3(boxExtents, boxExtents, boxExtents), Color.Green);
        }
    }
}

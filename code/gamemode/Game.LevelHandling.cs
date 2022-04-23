using Sandbox;

using TerryBros.Levels;

#pragma warning disable IDE0051

namespace TerryBros.Gamemode
{
    public partial class STBGame
    {
        public static Level LastLevel { get; private set; }

        public static Level CurrentLevel
        {
            get => _currentLevel;
            protected set
            {
                LastLevel = _currentLevel;
                _currentLevel = value;
            }
        }

        private static Level _currentLevel;

        public override void MoveToSpawnpoint(Entity pawn)
        {
            LevelElements.SpawnPoint spawnPoint = CurrentLevel?.GetLastCheckPoint();

            if (spawnPoint == null)
            {
                Log.Warning($"Couldn't find spawnpoint for {pawn}!");

                return;
            }

            spawnPoint.MoveToSpawn(pawn);
        }
    }
}

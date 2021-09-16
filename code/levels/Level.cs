using Sandbox;

using TerryBros.Settings;
using TerryBros.LevelElements;

namespace TerryBros.Levels
{
    public delegate void resetCheckPoints();

    public partial class Level : Entity
    {
        public static Level lastLevel { get; private set; }
        public static Level currentLevel { get; private set; }

        protected STBSpawn restartSpawn;
        protected STBSpawn checkPointSpawn;

        /// <summary>
        /// As a delegate cant be instantiated without a function, use this dummy here.
        /// </summary>
        private void dummyReset() { }
        private resetCheckPoints onResetCheckPoints;

        public Level()
        {
            lastLevel = currentLevel;
            currentLevel = this;
            onResetCheckPoints = dummyReset;
        }
        public STBSpawn GetRestartPoint()
        {
            return restartSpawn;
        }
        public STBSpawn GetLastCheckPoint()
        {
            if(checkPointSpawn != null)
            {
                return checkPointSpawn;
            } else
            {
                return GetRestartPoint();
            }
        }
        public void SetCheckPoint(Checkpoint checkPoint)
        {
            checkPointSpawn = checkPoint.spawnPoint;
            checkPoint.RegisterReset(ref onResetCheckPoints);
        }
        public void Restart()
        {
            checkPointSpawn = null;
            onResetCheckPoints();
            onResetCheckPoints = dummyReset;
        }
    
        //Note: Can't use parameters in generic constraints
        protected T CreateBox<T>(int GridX, int GridY) where T : BlockEntity, new()
        {
            T block = new T();
            block.Position = globalSettings.GetBlockPosForGridCoordinates(GridX, GridY);
            return block;
        }

        protected void CreateStair<T>(int GridX, int GridY, int height, bool upward = true) where T : BlockEntity, new()
        {
            for (int i = 0; i < height; i++)
            {
                int x = GridX + i;
                int maxHeight = upward ? i + 1 : height - i;

                for (int j = 0; j < maxHeight; j++)
                {
                    int y = GridY + j;

                    CreateBox<T>(x, y);
                }
            }
        }
        protected void CreateWallFromTo<T>(int StartGridX, int StartGridY, int EndGridX, int EndGridY) where T : BlockEntity, new()
        {
            for (int x = StartGridX; x <= EndGridX; x++)
            {
                for (int y = StartGridY; y <= EndGridY; y++)
                {
                    CreateBox<T>(x, y);
                }
            }
        }
        protected void CreateWall<T>(int GridX, int GridY, int width, int height) where T : BlockEntity, new()
        {
            for (int x = GridX; x < GridX + width; x++)
            {
                for (int y = GridY; y < GridY + height; y++)
                {
                    CreateBox<T>(x, y);
                }
            }
        }

        protected void CreateCheckPoint(int GridX, int GridY)
        {
            var point = CreateBox<Checkpoint>(GridX, GridY);
        }
        protected void CreateGoal(int GridX, int GridY)
        {
            var point = CreateBox<Goal>(GridX, GridY);
        }

    }
}

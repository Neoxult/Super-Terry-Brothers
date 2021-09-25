using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.LevelElements;
using TerryBros.Settings;
using TerryBros.Utils;

namespace TerryBros.Levels
{
    public abstract partial class Level : Entity
    {
        public BBox LevelBounds { get; private set; }

        /// <summary>
        /// World Bound given in number of Blocks
        /// </summary>
        public IntBBox LevelBoundsBlocks
        {
            get => _levelBoundsBlocks;
            set
            {
                _levelBoundsBlocks = value;

                LevelBounds = new BBox(
                    GlobalSettings.GetBlockPosForGridCoordinates(value.Mins) - (GlobalSettings.ForwardDir + 2 * GlobalSettings.UpwardDir + GlobalSettings.LookDir) * GlobalSettings.BlockSize / 2,
                    GlobalSettings.GetBlockPosForGridCoordinates(value.Maxs) + (GlobalSettings.ForwardDir + GlobalSettings.LookDir) * GlobalSettings.BlockSize / 2
                );
            }
        }
        private static IntBBox _levelBoundsBlocks = IntBBox.Zero;

        public Dictionary<int, Dictionary<int, BlockEntity>> GridBlocks = new();

        protected STBSpawn RestartSpawn;
        protected STBSpawn CheckPointSpawn;

        private Action _onResetCheckPoints = null;

        public Level()
        {

        }

        public abstract void Build();

        public STBSpawn GetRestartPoint()
        {
            return RestartSpawn;
        }

        public STBSpawn GetLastCheckPoint()
        {
            return CheckPointSpawn ?? GetRestartPoint();
        }

        public void SetCheckPoint(Checkpoint checkPoint)
        {
            CheckPointSpawn = checkPoint.SpawnPoint;

            checkPoint.RegisterReset(ref _onResetCheckPoints);
        }

        public void Restart()
        {
            CheckPointSpawn = null;

            _onResetCheckPoints?.Invoke();
        }

        protected T CreateBox<T>(int GridX, int GridY) where T : BlockEntity, new()
        {
            return new T()
            {
                Position = GlobalSettings.GetBlockPosForGridCoordinates(GridX, GridY)
            };
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

        protected Checkpoint CreateCheckPoint(int GridX, int GridY)
        {
            return CreateBox<Checkpoint>(GridX, GridY);
        }

        protected Goal CreateGoal(int GridX, int GridY)
        {
            return CreateBox<Goal>(GridX, GridY);
        }

        public string Export()
        {
            Dictionary<string, List<Vector2>> dict = new();

            foreach (KeyValuePair<int, Dictionary<int, BlockEntity>> x in GridBlocks)
            {
                foreach (KeyValuePair<int, BlockEntity> y in x.Value)
                {
                    dict.TryGetValue(y.Value.TypeName, out List<Vector2> blockList);

                    if (blockList == null)
                    {
                        blockList = new();

                        dict.Add(y.Value.TypeName, blockList);
                    }

                    blockList.Add(new Vector2(x.Key, y.Key));
                }
            }

            return JsonSerializer.Serialize(dict);
        }

        public static void Import(Dictionary<string, List<Vector2>> dict)
        {
            foreach (KeyValuePair<string, List<Vector2>> blockList in dict)
            {
                Type blockType = BlockEntity.GetByName(blockList.Key);

                if (blockType != null)
                {
                    foreach (Vector2 position in blockList.Value)
                    {
                        BlockEntity blockEntity = Library.Create<BlockEntity>(blockType);
                        blockEntity.Position = GlobalSettings.GetBlockPosForGridCoordinates((int) position.x, (int) position.y);
                    }
                }
            }
        }

        public static void Clear()
        {
            foreach (Entity entity in Entity.All)
            {
                if (entity is BlockEntity blockEntity)
                {
                    try
                    {
                        blockEntity.Delete();
                    }
                    catch (Exception) { }
                }
            }

            STBGame.CurrentLevel.GridBlocks = new();
        }
    }
}

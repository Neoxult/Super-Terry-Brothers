using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

using TerryBros.Events;
using TerryBros.LevelElements;
using TerryBros.Settings;
using TerryBros.Utils;

namespace TerryBros.Levels
{
    public partial class Level
    {
        public BBox LevelBounds { get; private set; }
        public BBox LevelBoundsLocal { get; private set; }

        /// <summary>
        /// World Bound given in number of Blocks
        /// </summary>
        public IntBBox LevelBoundsBlocks
        {
            get => _levelBoundsBlocks;
            set
            {
                _levelBoundsBlocks = value;

                LevelBoundsLocal = new BBox(
                    (value.Mins * 2 - new Vector3(1, 1, 1)) * GlobalSettings.BlockSize / 2,
                    (value.Maxs * 2 + new Vector3(1, 1, 1)) * GlobalSettings.BlockSize / 2
                );

                LevelBounds = new BBox(
                    GlobalSettings.ConvertLocalToGlobalCoordinates(LevelBoundsLocal.Mins),
                    GlobalSettings.ConvertLocalToGlobalCoordinates(LevelBoundsLocal.Maxs)
                );
            }
        }
        private static IntBBox _levelBoundsBlocks = IntBBox.Zero;

        public Dictionary<int, Dictionary<int, BlockEntity>> GridBlocks = new();

        public Sky Sky { get; set; }
        public List<EnvironmentLightEntity> Lights { get; set; } = new();

        protected LevelElements.SpawnPoint RestartSpawn;
        protected LevelElements.SpawnPoint CheckPointSpawn;

        public void RegisterBlock(BlockEntity block)
        {
            IntBBox intBBox = LevelBoundsBlocks;

            int GridX = block.GridPosition.X;
            int GridY = block.GridPosition.Y;
            int GridZ = block.GridPosition.Z;

            if (GridX < intBBox.Mins.X)
            {
                intBBox.Mins.X = GridX;
            }

            if (GridX > intBBox.Maxs.X)
            {
                intBBox.Maxs.X = GridX;
            }

            if (GridY < intBBox.Mins.Y)
            {
                intBBox.Mins.Y = GridY;
            }

            if (GridY > intBBox.Maxs.Y)
            {
                intBBox.Maxs.Y = GridY;
            }

            if (GridZ < intBBox.Mins.Z)
            {
                intBBox.Mins.Z = GridZ;
            }

            if (GridZ > intBBox.Maxs.Z)
            {
                intBBox.Maxs.Z = GridZ;
            }

            LevelBoundsBlocks = intBBox;

            GridBlocks.TryGetValue(GridX, out Dictionary<int, BlockEntity> dict);

            if (dict == null)
            {
                dict = new();

                GridBlocks.Add(GridX, dict);
            }

            dict.TryGetValue(GridY, out BlockEntity blockEntity);

            if (blockEntity != null)
            {
                dict.Remove(GridY);
            }

            dict[GridY] = block;
        }

        public LevelElements.SpawnPoint GetRestartPoint() => RestartSpawn;

        public LevelElements.SpawnPoint GetLastCheckPoint() => CheckPointSpawn ?? GetRestartPoint();

        public void CheckPointReached(Player _, Checkpoint checkPoint)
        {
            //TODO: Allow different players to have different checkpoints
            CheckPointSpawn = checkPoint.SpawnPoint;
        }

        [Event(TBEvent.Level.RESTART)]
        public void Restart()
        {
            CheckPointSpawn = null;
        }

        protected static T CreateBox<T>(int GridX, int GridY, BlockAsset asset) where T : BlockEntity, new() => new()
        {
            Asset = asset,
            Position = GlobalSettings.GetBlockPosForGridCoordinates(GridX, GridY)
        };

        protected static void CreateStair<T>(int GridX, int GridY, int height, bool upward = true) where T : BlockEntity, new()
        {
            for (int i = 0; i < height; i++)
            {
                int x = GridX + i;
                int maxHeight = upward ? i + 1 : height - i;

                for (int j = 0; j < maxHeight; j++)
                {
                    int y = GridY + j;

                    CreateBox<T>(x, y, BlockAsset.GetByName("blooming"));
                }
            }
        }

        protected static void CreateWallFromTo<T>(int StartGridX, int StartGridY, int EndGridX, int EndGridY) where T : BlockEntity, new()
        {
            for (int x = StartGridX; x <= EndGridX; x++)
            {
                for (int y = StartGridY; y <= EndGridY; y++)
                {
                    CreateBox<T>(x, y, BlockAsset.GetByName("blooming"));
                }
            }
        }

        protected static void CreateWall<T>(int GridX, int GridY, int width, int height) where T : BlockEntity, new()
        {
            for (int x = GridX; x < GridX + width; x++)
            {
                for (int y = GridY; y < GridY + height; y++)
                {
                    CreateBox<T>(x, y, BlockAsset.GetByName("blooming"));
                }
            }
        }

        protected static Checkpoint CreateCheckPoint(int GridX, int GridY) => CreateBox<Checkpoint>(GridX, GridY, BlockAsset.GetByName("checkpoint"));

        protected static Goal CreateGoal(int GridX, int GridY) => CreateBox<Goal>(GridX, GridY, BlockAsset.GetByName("goal"));

        public void Build()
        {
            RestartSpawn = new()
            {
                Position = GlobalSettings.GetBlockPosForGridCoordinates(0, 1)
            };

            Sky = new DefaultSky();

            //TODO: Properly set lights up in local space
            Lights.Add(new()
            {
                //light.Position = GlobalSettings.ConvertLocalToGlobalCoordinates(new Vector3(1, 4, -1));
                //light.Rotation = Rotation.LookAt(GlobalSettings.GroundPos - light.Position, GlobalSettings.UpwardDir);
                Rotation = Rotation.LookAt(new Vector3(-1, 1, -4), GlobalSettings.UpwardDir),
                Brightness = 2f
            });

            Lights.Add(new()
            {
                //light.Position = GlobalSettings.ConvertLocalToGlobalCoordinates(new Vector3(1, 4, -1));
                //light.Rotation = Rotation.LookAt(GlobalSettings.GroundPos - light.Position, GlobalSettings.UpwardDir);
                Rotation = Rotation.LookAt(new Vector3(-1, 1, 4), GlobalSettings.UpwardDir),
                Brightness = 2f
            });

            Lights.Add(new()
            {
                //light.Position = GlobalSettings.ConvertLocalToGlobalCoordinates(new Vector3(-1, 1, -0.5f));
                //light.Rotation = Rotation.LookAt(GlobalSettings.GroundPos - light.Position, GlobalSettings.UpwardDir);
                Rotation = Rotation.LookAt(new Vector3(1, 0.5f, -1), GlobalSettings.UpwardDir),
                Brightness = 2f
            });

            Lights.Add(new()
            {
                //light.Position = GlobalSettings.ConvertLocalToGlobalCoordinates(new Vector3(-1, 1, -0.5f));
                //light.Rotation = Rotation.LookAt(GlobalSettings.GroundPos - light.Position, GlobalSettings.UpwardDir);
                Rotation = Rotation.LookAt(new Vector3(1, 0.5f, 1), GlobalSettings.UpwardDir),
                Brightness = 2f
            });
        }

        public string Export()
        {
            Dictionary<string, List<Vector2>> dict = new();

            foreach (KeyValuePair<int, Dictionary<int, BlockEntity>> x in GridBlocks)
            {
                foreach (KeyValuePair<int, BlockEntity> y in x.Value)
                {
                    dict.TryGetValue(y.Value.Asset.Name, out List<Vector2> blockList);

                    if (blockList == null)
                    {
                        blockList = new();

                        dict.Add(y.Value.Asset.Name, blockList);
                    }

                    blockList.Add(new Vector2(x.Key, y.Key));
                }
            }

            return JsonSerializer.Serialize(dict);
        }

        public void Import(Dictionary<string, List<Vector2>> dict)
        {
            Build();

            foreach (KeyValuePair<string, List<Vector2>> blockList in dict)
            {
                BlockAsset asset = BlockAsset.GetByName(blockList.Key);

                if (asset != null)
                {
                    foreach (Vector2 position in blockList.Value)
                    {
                        BlockEntity blockEntity = BlockEntity.FromAsset(asset);
                        blockEntity.Position = GlobalSettings.GetBlockPosForGridCoordinates((int) position.x, (int) position.y);

                        RegisterBlock(blockEntity);
                    }
                }
            }

            Event.Run(TBEvent.Level.LOADED, this);
        }

        public void Clear()
        {
            foreach (Dictionary<int, BlockEntity> dict in GridBlocks.Values)
            {
                foreach (BlockEntity blockEntity in dict.Values)
                {
                    try
                    {
                        blockEntity.Delete();
                    }
                    catch (Exception) { }
                }
            }

            Sky.Delete();
            Sky = null;

            foreach (EnvironmentLightEntity environmentLightEntity in Lights)
            {
                environmentLightEntity.Delete();
            }

            Lights.Clear();
            GridBlocks.Clear();

            RestartSpawn?.Delete();
            RestartSpawn = null;

            LevelBoundsBlocks = IntBBox.Zero;

            Event.Run(TBEvent.Level.CLEARED, this);
        }
    }
}

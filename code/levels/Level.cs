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

        public string Data { get; set; }

        protected LevelElements.SpawnPoint RestartSpawn;

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

        public LevelElements.SpawnPoint GetLastCheckPoint(Player player) => player.CheckPointSpawn ?? GetRestartPoint();

        public void CheckPointReached(Player player, Checkpoint checkPoint)
        {
            if (Host.IsClient && player != Local.Pawn)
            {
                return;
            }

            player.CheckPointSpawn = checkPoint.SpawnPoint;
        }

        [Event(TBEvent.Level.RESTART)]
        public static void Restart()
        {
            if (Host.IsClient)
            {
                if (Local.Pawn is Player player)
                {
                    player.CheckPointSpawn = null;
                }
            }
            else
            {
                foreach (Client client in Gamemode.STBGame.Instance.PlayingClients)
                {
                    if (client.Pawn is Player player)
                    {
                        player.CheckPointSpawn = null;
                    }
                }
            }
        }

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

            return JsonSerializer.Serialize(Compress(dict));
        }

        public void Import(string data)
        {
            Data = data;

            Dictionary<string, List<Vector3>> dict = Decompress(Compression.Decompress<Dictionary<string, string>>(data.ByteArray()));

            Build();

            foreach (KeyValuePair<string, List<Vector3>> blockList in dict)
            {
                BlockAsset asset = BlockAsset.GetByName(blockList.Key);

                if (asset != null)
                {
                    foreach (Vector3 position in blockList.Value)
                    {
                        BlockEntity blockEntity = BlockEntity.FromAsset(asset);
                        blockEntity.Position = GlobalSettings.GetBlockPosForGridCoordinates((int) position.x, (int) position.z);

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
                    blockEntity.Delete();
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

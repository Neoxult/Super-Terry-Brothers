using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

using TerryBros.LevelElements;
using TerryBros.Player;
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
            CheckPointSpawn = checkPoint.spawnPoint;

            checkPoint.RegisterReset(_onResetCheckPoints);
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

            foreach (KeyValuePair<int, Dictionary<int, BlockEntity>> y in GridBlocks)
            {
                foreach (KeyValuePair<int, BlockEntity> x in y.Value)
                {
                    dict.TryGetValue(x.Value.TypeName, out List<Vector2> blockList);

                    if (blockList == null)
                    {
                        blockList = new();

                        dict.Add(x.Value.TypeName, blockList);
                    }

                    blockList.Add(new Vector2(x.Key, y.Key));
                }
            }

            return JsonSerializer.Serialize(dict);
        }

        public static void Import(string data)
        {
            Clear();

            Dictionary<string, List<Vector2>> dict = null;

            try
            {
                dict = JsonSerializer.Deserialize<Dictionary<string, List<Vector2>>>(data);
            }
            catch (Exception) { }

            if (dict == null)
            {
                return;
            }

            foreach (KeyValuePair<string, List<Vector2>> blockVectorList in dict)
            {
                Type blockType = BlockEntity.GetByName(blockVectorList.Key);

                if (blockType != null)
                {
                    foreach (Vector2 vector2 in blockVectorList.Value)
                    {
                        BlockEntity blockEntity = Library.Create<BlockEntity>(blockType);
                        blockEntity.Position = new Vector3(vector2.x, 0, vector2.y);
                    }
                }
            }
        }

        [ServerCmd(Name = "stb_import_serveronly")]
        public static void ServerImportData(string data)
        {
            if ((!ConsoleSystem.Caller?.HasPermission("import") ?? false))
            {
                return;
            }

            Import(data);
            ClientImport(data);

            foreach (Client client in Client.All)
            {
                client.Pawn.Spawn();
            }
        }

        [ClientRpc]
        public static void ClientImport(string data)
        {
            Import(data);
        }

        public static void Clear()
        {
            foreach (BlockEntity entity in BlockEntity.List)
            {
                entity.Delete();
            }

            BlockEntity.List.Clear();
        }
    }
}

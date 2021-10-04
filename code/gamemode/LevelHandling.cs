using System;
using System.Collections.Generic;

using Sandbox;

using TerryBros.Levels;
using TerryBros.LevelElements;
using TerryBros.UI;
using TerryBros.Settings;

namespace TerryBros.Gamemode
{
    public partial class STBGame
    {
        public static Level LastLevel { get; private set; }

        public static Level CurrentLevel
        {
            get => _currentLevel;
            set
            {
                LastLevel = _currentLevel;
                _currentLevel = value;
            }
        }

        public static List<BlockData> BlockDataList = new();

        private static Level _currentLevel;
        public override void MoveToSpawnpoint(Entity pawn)
        {
            STBSpawn spawnPoint = CurrentLevel?.GetLastCheckPoint();

            if (spawnPoint == null)
            {
                Log.Warning($"Couldn't find spawnpoint for {pawn}!");

                return;
            }

            spawnPoint.MoveToSpawn(pawn);
        }

        [Event.Entity.PostSpawn]
        private void PostLevelSpawn()
        {
            if (CurrentLevel != null)
            {
                return;
            }

            CurrentLevel = new DefaultLevel();
            CurrentLevel.Build();
        }

        public static void CreateBlockData()
        {
            BlockDataList = new();
            foreach (Type type in Library.GetAll<BlockEntity>())
            {
                if (!type.IsAbstract && !type.ContainsGenericParameters)
                {
                    BlockEntity blockEntity = Library.Create<BlockEntity>(type);
                    BlockData blockData = blockEntity.GetBlockData();

                    blockEntity.Delete();
                    BlockDataList.Add(blockData);
                }
            }
        }
    }
}

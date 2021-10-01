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
            CreateBlockData();

            if (IsClient)
            {
                //Note: Normally created on server and then networked,
                // but event is called later and blockdatas are missing
                //TODO: Discuss with Alf, if this is okay like that
                new Hud();
            }

            if (CurrentLevel != null)
            {
                return;
            }

            CurrentLevel = new DefaultLevel();
            CurrentLevel.Build();
        }

        private void CreateBlockData()
        {
            foreach (Type type in Library.GetAll<BlockEntity>())
            {
                if (!type.IsAbstract && !type.ContainsGenericParameters)
                {
                    BlockEntity blockEntity = Library.Create<BlockEntity>(type);
                    BlockData blockData = blockEntity.GetBlockData();

                    blockEntity.Delete();
                    BlockDataList.Add(blockData);
                    Log.Info(blockData.Name);
                }
            }
        }
    }
}

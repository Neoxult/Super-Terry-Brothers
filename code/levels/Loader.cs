using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

using Sandbox;

using TerryBros.Gamemode;

namespace TerryBros.Levels.Loader
{
    public static partial class Local
    {
        public const string CUSTOM_LEVEL_FOLDER = "custom_levels";

        [ClientRpc]
        public static void Save(string fileName)
        {
            if (!FileSystem.Data.DirectoryExists(CUSTOM_LEVEL_FOLDER))
            {
                FileSystem.Data.CreateDirectory(CUSTOM_LEVEL_FOLDER);
            }

            FileSystem.Data.WriteAllText($"{CUSTOM_LEVEL_FOLDER}/{fileName.Split('.')[0]}.json", STBGame.CurrentLevel.Export());
        }

        [ClientRpc]
        public static void Delete(string fileName)
        {
            if (!FileSystem.Data.DirectoryExists(CUSTOM_LEVEL_FOLDER))
            {
                return;
            }

            string filePath = $"{CUSTOM_LEVEL_FOLDER}/{fileName.Split('.')[0]}.json";

            if (!FileSystem.Data.FileExists(filePath))
            {
                return;
            }

            FileSystem.Data.DeleteFile(filePath);
        }

        [ClientRpc]
        public static void Load(string fileName)
        {
            string file = $"{CUSTOM_LEVEL_FOLDER}/{fileName.Split('.')[0]}.json";

            if (!FileSystem.Data.FileExists(file))
            {
                return;
            }

            try
            {
                Level.SyncData(JsonSerializer.Deserialize<Dictionary<string, string>>(FileSystem.Data.ReadAllText(file)));
            }
            catch (Exception) { }
        }

        public static List<string> Get()
        {
            Assert.True(Host.IsClient);

            if (!FileSystem.Data.DirectoryExists(CUSTOM_LEVEL_FOLDER))
            {
                return new();
            }

            return FileSystem.Data.FindFile(CUSTOM_LEVEL_FOLDER, "*.json").ToList();
        }

        public static Level Empty()
        {
            Level level = new();
            level.Build();

            return level;
        }
    }
}

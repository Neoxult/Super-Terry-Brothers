using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.Levels;
using TerryBros.UI.LevelLoader;

namespace TerryBros.Player
{
    public partial class TerryBrosPlayer
    {
        private static int _currentPacketHash = -1;
        private static int _packetCount;
        private static string[] _packetData;

        [ClientCmd(Name = "stb_save")]
        public static void SaveLevel(string fileName)
        {
            if (!FileSystem.Data.DirectoryExists("custom_levels"))
            {
                FileSystem.Data.CreateDirectory("custom_levels");
            }

            FileSystem.Data.WriteAllText($"custom_levels/{fileName.Split('.')[0]}.json", STBGame.CurrentLevel.Export());
        }

        [ClientCmd(Name = "stb_load")]
        public static void LoadLevel(string fileName = null)
        {
            if (fileName == null)
            {
                Loader.Instance.SetLevels(GetLevels());
                Loader.Instance.Display = true;
            }
            else
            {
                string file = $"custom_levels/{fileName.Split('.')[0]}.json";

                if (!FileSystem.Data.FileExists(file))
                {
                    return;
                }

                try
                {
                    ServerSendLevelData(JsonSerializer.Deserialize<Dictionary<string, List<Vector2>>>(FileSystem.Data.ReadAllText(file)));
                }
                catch (Exception) { }
            }
        }

        public static List<string> GetLevels()
        {
            Assert.True(Host.IsClient);

            if (!FileSystem.Data.DirectoryExists("custom_levels"))
            {
                return new();
            }

            return FileSystem.Data.FindFile("custom_levels", "*.json").ToList();
        }

        public static void ServerSendLevelData(Dictionary<string, List<Vector2>> dict)
        {
            string levelDataJson = JsonSerializer.Serialize(dict);
            int splitLength = 150;
            int splitCount = (int) MathF.Ceiling((float) levelDataJson.Length / splitLength);

            for (int i = 0; i < splitCount; i++)
            {
                ServerSendPartialLevelData(levelDataJson.GetHashCode(), i, splitCount, levelDataJson.Substring(splitLength * i, splitLength + Math.Min(0, levelDataJson.Length - splitLength * (i + 1))));
            }
        }

        [ServerCmd(Name = "stb_send_partialleveldata")]
        public static void ServerSendPartialLevelData(int packetHash, int packetNum, int maxPackets, string partialLevelData)
        {
            if (!ConsoleSystem.Caller?.HasPermission("import") ?? true)
            {
                return;
            }

            ProceedPartialLevelData(packetHash, packetNum, maxPackets, partialLevelData);
            ClientSendPartialLevelData(packetHash, packetNum, maxPackets, partialLevelData);
        }

        [ClientRpc]
        public static void ClientSendPartialLevelData(int packetHash, int packetNum, int maxPackets, string partialLevelData)
        {
            ProceedPartialLevelData(packetHash, packetNum, maxPackets, partialLevelData);
        }

        public static void ProceedPartialLevelData(int packetHash, int packetNum, int maxPackets, string partialLevelData)
        {
            if (_currentPacketHash != packetHash)
            {
                _packetCount = 0;
                _packetData = new string[maxPackets];

                _currentPacketHash = packetHash;
            }

            _packetData[packetNum] = partialLevelData;
            _packetCount++;

            if (_packetCount == maxPackets)
            {
                _currentPacketHash = -1;

                Level.Clear();
                Level.Import(JsonSerializer.Deserialize<Dictionary<string, List<Vector2>>>(string.Join("", _packetData)));

                if (Host.IsServer)
                {
                    STBGame.CurrentLevel?.Restart();
                    STBGame.ClientRestartLevel();

                    foreach (Client client in Client.All)
                    {
                        if (client.Pawn is TerryBrosPlayer player)
                        {
                            player.Respawn();
                        }
                    }
                }
            }
        }

        [ServerCmd(Name = "stb_clear")]
        public static void ServerClear()
        {
            Level.Clear();
            ClientClear();
        }

        [ClientRpc]
        public static void ClientClear()
        {
            Level.Clear();
        }
    }
}

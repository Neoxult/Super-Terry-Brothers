using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

using TerryBros.Gamemode;
using TerryBros.Levels;

namespace TerryBros.Player
{
    public partial class TerryBrosPlayer
    {
        private static int _currentPacketHash = -1;
        private static int _packetCount;
        private static Dictionary<int, string> _packetData = new();

        [ClientCmd(Name = "stb_save")]
        public static void SaveLevel(string fileName)
        {
            FileSystem.Data.WriteAllText(fileName.Split('.')[0] + ".json", STBGame.CurrentLevel.Export());
        }

        [ClientCmd(Name = "stb_load")]
        public static void LoadLevel(string fileName)
        {
            string file = fileName.Split('.')[0] + ".json";

            if (!FileSystem.Data.FileExists(file))
            {
                return;
            }

            Dictionary<string, List<Vector2>> dict = null;

            try
            {
                dict = JsonSerializer.Deserialize<Dictionary<string, List<Vector2>>>(FileSystem.Data.ReadAllText(file));
            }
            catch (Exception) { }

            if (dict == null)
            {
                return;
            }

            int count = 0;

            string[] blockTypes = new string[dict.Count];
            Vector2[][] positions = new Vector2[dict.Count][];

            foreach (KeyValuePair<string, List<Vector2>> keyValuePair in dict)
            {
                blockTypes[count] = keyValuePair.Key;
                positions[count] = keyValuePair.Value.ToArray();

                count++;
            }

            ServerSendLevelData(new LevelData(blockTypes, positions));
        }

        public static void ServerSendLevelData(LevelData levelData)
        {
            string levelDataJson = JsonSerializer.Serialize(levelData);
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
                _packetData.Clear();

                _currentPacketHash = packetHash;
            }

            _packetData.Add(packetNum, partialLevelData);
            _packetCount++;

            if (_packetCount == maxPackets)
            {
                _currentPacketHash = -1;

                string fullData = "";

                for (int i = 0; i < maxPackets; i++)
                {
                    _packetData.TryGetValue(i, out string partialData);

                    fullData += partialData;
                }

                Level.Clear();
                Level.Import(JsonSerializer.Deserialize<LevelData>(fullData));

                if (Host.IsServer)
                {
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
